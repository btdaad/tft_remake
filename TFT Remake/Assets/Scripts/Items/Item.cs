using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] BaseItemSO baseItemSO;

    public void Dematerialize()
    {
        gameObject.SetActive(false);
        // this.GetComponent<MeshRenderer>().enabled = false;
        // this.GetComponent<Collider>().enabled = false;
    }
}
