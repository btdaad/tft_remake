using UnityEngine;
using System.Collections.Generic;
using System;

public class PoolManager : MonoBehaviour
{
    private Dictionary<UnitType, int> _unitsPool;
    [SerializeField] private int[] _poolSizes = { 30, 18, 9 }; /* 1 Coût 30; 2 Coût 25; 3 Coût 18; 4 Coût 10; 5 Coût 9 */

    // https://www.metatft.com/tables/shop-odds
    [SerializeField]
    private float[,] _poolPercentage = {
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

    public void Init()
    {
        _unitsPool = new Dictionary<UnitType, int>();

        foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
        {
            int index = (int)unitType;
            if (index < (int)UnitType.Cone)
                _unitsPool.Add(unitType, _poolSizes[0]);
            else if (index < (int)UnitType.Icosahedron)
                _unitsPool.Add(unitType, _poolSizes[1]);
            else if (index < (int)UnitType.TargetDummy)
                _unitsPool.Add(unitType, _poolSizes[2]);
        }
    }

    public (float, float, float) GetPoolPercentage(int level)
    {
        return (_poolPercentage[level,0], _poolPercentage[level,1], _poolPercentage[level,2]);
    }

    private void Dump()
    {
        foreach (var unitType in _unitsPool.Keys)
            Debug.Log($"{unitType} : {_unitsPool[unitType]}");
    }
}
