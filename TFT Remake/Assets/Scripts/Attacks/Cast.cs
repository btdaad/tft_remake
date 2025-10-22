using UnityEngine;

public class Cast : MonoBehaviour
{
    private Unit _targetUnit = null;
    private Transform _target = null;
    private float _damage = 0.0f;
    private bool _isPhysicalDamage = true;
    [SerializeField] float speed;

    void Start()
    { }

    void Update()
    {
        if (_target != null)
        {
            if (_target.gameObject.activeSelf)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _target.position, step);
            }
            else
                Destroy(this.gameObject);
        }
    }

    public void SetTarget(Unit targetUnit, float damage, bool isPhysicalDamage)
    {
        _targetUnit = targetUnit;
        _target = targetUnit.transform;
        _damage = damage;
        _isPhysicalDamage = isPhysicalDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target != null && _target.GetComponent<Collider>() == other)
        {
            _targetUnit.TakeDamage(_damage, _isPhysicalDamage);
            Destroy(this.gameObject);
        }
    }
}
