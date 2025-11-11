using UnityEngine;
using System.Collections.Generic;
using System;

public class ShopManager : MonoBehaviour
{
    private int SHOP_SIZE = 5;
    private int DIFF_COST = 3; // 1-cost, 3-cost, 5-cost
    [SerializeField] public int refreshCost = 2;
    private int firstThreeCostIndex = (int)UnitType.Cone;
    private int firstFiveCostIndex = (int)UnitType.Icosahedron;
    private int outOfBoundIndex = (int)UnitType.TargetDummy;

    // list of dictionaries of the nb of unit available for each UnitType, separated by unit cost
    private List<Dictionary<UnitType, int>> _unitsPool; // [{1-cost units}, {3-cost units}, {5-cost units}]
    [SerializeField] private int[] _poolSizes = { 30, 18, 9 }; /* 1 Coût 30; 2 Coût 25; 3 Coût 18; 4 Coût 10; 5 Coût 9 */

    // https://www.metatft.com/tables/shop-odds
    [SerializeField] private float[,] _poolPercentage = {
        { 0f, 0f, 0f}, // filler zone to ease access as it is 0-indexed
        { 0f, 0f, 0f}, // filler zone for level 1
        // Lvl 2	100%	0%	0%	0%	0%
        { 1f,    0f,      0f},
        // Lvl 3	75%	    25%	0%	0%	0%
        { 1f,    0f,      0f},
        // Lvl 4	55%	    30%	15%	0%	0%
        { 0.85f, 0.15f,   0f},
        // Lvl 5	45%	    33%	20%	2%	0%
        { 0.78f, 0.2f,    0.02f},
        // Lvl 6	30%	    40%	25%	5%	0%
        { 0.7f,  0.25f, 0.05f},
        // Lvl 7	19%	    30%	40%	10%	1%
        { 0.49f, 0.4f, 0.11f},
        // Lvl 8	17%	    24%	32%	24%	3%
        { 0.41f, 0.32f, 0.27f},
        // Lvl 9	15%	    18%	25%	30%	12%
        { 0.33f, 0.25f, 0.42f},
        // Lvl 10	5%	    10%	20%	40%	25%
        { 0.15f, 0.2f, 0.65f}
    }; // Lvl 11	1%	    2%	12%	50%	35%

    private UnitType[][] _shop;
    [SerializeField] public GameObject[] _unitPrefabs;
    public int GetUnitCostIndexFromUnitType(UnitType unitType)
    {
        int index = (int)unitType;
        if (index < firstThreeCostIndex) // unit is a 1-cost
            return 0;
        else if (index < firstFiveCostIndex) // unit is a 3-cost
            return 1;
        else if (index < outOfBoundIndex) // unit is a 5-cost
            return 2;
        return -1; // unit is a target dummy
    }

    public void Init()
    {
        UnityEngine.Random.InitState(300600);

        _unitsPool = new List<Dictionary<UnitType, int>>();
        for (int i = 0; i < DIFF_COST; i++)
            _unitsPool.Add(new Dictionary<UnitType, int>());

        foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
        {
            int costIndex = GetUnitCostIndexFromUnitType(unitType);
            if (costIndex != -1) // unit is not a target dummy
                _unitsPool[costIndex].Add(unitType, _poolSizes[costIndex]);
        }

        _shop = JaggedArrayUtil.InitJaggedArray<UnitType>(2, SHOP_SIZE, () => UnitType.TargetDummy);
    }

    public int GetShopSize()
    {
        return SHOP_SIZE;
    }

    public int GetRefreshCost()
    {
        return refreshCost;
    }

    public UnitType[] GetShop(bool isPlayer)
    {
        int shopSide = isPlayer ? 0 : 1;
        return _shop[shopSide];
    }

    public void BuyUnit(bool isPlayer, int i)
    {
        int shopSide = isPlayer ? 0 : 1;
        UnitType unitType = _shop[shopSide][i];

        Vector3 position = Vector3.zero;
        GameObject unitGO = Instantiate(_unitPrefabs[(int)unitType], position, Quaternion.identity);
    }

    public GameObject GetUnitFromUnitType(UnitType unitType)
    {
        return _unitPrefabs[(int)unitType];
    }

    // Returns the index corresponding to the dictionary contaning the unit in the _unitsPool list
    private int GetUnitCostIndexFromPercentage(float unitCost, float oneCostPercentage, float threeCostPercentage)
    {
        if (unitCost <= oneCostPercentage)
            return 0; // 1-cost unit
        else
        {
            unitCost -= oneCostPercentage;
            if (unitCost <= threeCostPercentage)
                return 1; // 3-cost unit
            else return 2; // 5-cost unit
        }
    }

    private int GetTotalUnitAvailable(int costIndex)
    {
        int count = 0;
        foreach (UnitType unitType in _unitsPool[costIndex].Keys)
            count += _unitsPool[costIndex][unitType];
        return count;
    }

    // Goes through units one by one until it reaches a random threshold set according to the number of x-cost units available
    private UnitType RandomSelectUnit(int costIndex, int totalUnitAvailable)
    {
        int count = 0;
        int round = 1;
        int unitIndex = UnityEngine.Random.Range(0, totalUnitAvailable);
        while (round <= _poolSizes[costIndex]) // cap the number of loop to the max nb of unit available for x-cost units
        {
            foreach (UnitType unitType in _unitsPool[costIndex].Keys)
            {
                if (_unitsPool[costIndex][unitType] >= round) // pass UnitTypes that does not have enough units available
                {
                    if (count == unitIndex)
                        return unitType;
                    count++;
                }
            }
            round++;
        }
        Debug.LogError("Could not select a unit randomly");
        return UnitType.TargetDummy;
    }
    public void RefreshShop(bool isPlayer)
    {
        int shopSide = isPlayer ? 0 : 1;
        (float oneCostPercentage, float threeCostPercentage, _) = GetPoolPercentage(GameManager.Instance.GetPlayer(isPlayer).GetLevel());

        // put units back in the pool
        for (int i = 0; i < SHOP_SIZE; i++)
        {
            if (_shop[shopSide][i] != UnitType.TargetDummy)
                UpdatePool(_shop[shopSide][i], 1);
        }

        for (int i = 0; i < SHOP_SIZE; i++)
        {
            int costIndex;
            int totalUnitAvailable = 0;
            int counter = 0;
            do
            {
                // find what cost the unit will be
                float unitCost = UnityEngine.Random.Range(0f, 1f);
                costIndex = GetUnitCostIndexFromPercentage(unitCost, oneCostPercentage, threeCostPercentage);
                totalUnitAvailable = GetTotalUnitAvailable(costIndex);
                counter++;
            } while (totalUnitAvailable == 0 && counter < 10);

            if (totalUnitAvailable != 0)
            {
                // pick specific unit type
                UnitType unitType = RandomSelectUnit(costIndex, totalUnitAvailable);
                _shop[shopSide][i] = unitType;
                UpdatePool(unitType, -1);
            }
            else
            {
                Debug.LogError("Could not find a unit in less than 10 iterations.");
                _shop[shopSide][i] = UnitType.TargetDummy;
            }
        }
    }

    private void UpdatePool(UnitType unitType, int delta)
    {
        foreach (Dictionary<UnitType, int> pool in _unitsPool)
        {
            foreach (UnitType type in pool.Keys)
            {
                if (unitType == type)
                {
                    int nbUnitAvailable = pool[type];
                    pool[type] = nbUnitAvailable + delta;
                    return;
                }
            }
        }
    }

    public (float, float, float) GetPoolPercentage(int level)
    {
        return (_poolPercentage[level, 0], _poolPercentage[level, 1], _poolPercentage[level, 2]);
    }

    private void Dump()
    {
        foreach (Dictionary<UnitType, int> pool in _unitsPool)
        {
            foreach (var unitType in pool.Keys)
                Debug.Log($"{unitType} : {pool[unitType]}");
        }
    }
}
