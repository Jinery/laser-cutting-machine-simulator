using UnityEngine;

public class Flammable : MonoBehaviour
{
    [SerializeField] private FireType _fireType = FireType.A;
    [SerializeField] private float _ignitionTemperature = 100.0f, _brunDuration = 60.0f;
    [SerializeField] private bool _canSpread = true;

    private bool _isOnFire = false;
    private FireSource _fireSource;
    private Renderer _renderer;

    public bool IsOnFire => _isOnFire;

    public void Ignite()
    {
        if (_isOnFire) return;

        GameObject fireObject = new GameObject($"FireSource_{_fireType.ToString()}");
        fireObject.transform.position = transform.position;
        fireObject.transform.SetParent(transform);

        _fireSource = fireObject.AddComponent<FireSource>();
        _fireSource.Type = _fireType;

        _isOnFire = false;
    }
}
