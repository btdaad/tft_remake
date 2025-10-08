using UnityEngine;

public class PvPManager : MonoBehaviour
{
    public void Init()
    {

    }

    public void Fight(Transform[][] battlefield)
    {
        Debug.Log(battlefield.Length);
        Debug.Log(battlefield[0].Length);
    }
}
