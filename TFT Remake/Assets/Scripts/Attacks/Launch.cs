using UnityEngine;

public class Launch : MonoBehaviour
{
    private Transform _target = null;
    [SerializeField] float speed;

    void Start()
    { }

    void Update()
    {
        if (_target != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target.position, step);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER");
        if (_target != null && _target.GetComponent<Collider>() == other)
            Destroy(this.gameObject);
    }
}
