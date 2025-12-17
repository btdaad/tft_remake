using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    [SerializeField] public ConsumableItemSO consumableItemSO;

    public void Consume()
    {
        Destroy(this.gameObject);
    }

    public void ApplyEffects(Unit unit)
    {
        Item[] items = unit.GetItems();
        switch (consumableItemSO.type)
        {
            case ConsumableItemSO.ConsumableType.REFORGER:
                Debug.Log("Reforge");
                break;
            case ConsumableItemSO.ConsumableType.REMOVER:
                Debug.Log("Remove");
                break;
            default:
                Debug.Log("Consumable type not handles");
                break;
        }
    }
}
