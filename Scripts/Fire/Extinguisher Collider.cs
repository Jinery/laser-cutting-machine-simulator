using UnityEngine;

public class ExtinguisherCollider : MonoBehaviour
{
    private Collider _collider;
    private FireExtinguisher _extinguisher;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _collider = GetComponent<Collider>();
        _extinguisher = GetComponentInParent<FireExtinguisher>(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_extinguisher != null)
        {
            if (other.TryGetComponent<FireSource>(out FireSource fireSource))
            {
                fireSource.Extinguish(_extinguisher);
            }
        }
    }
}
