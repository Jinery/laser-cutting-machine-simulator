using UnityEngine;

public class Flammable : MonoBehaviour
{
    [SerializeField] private FireType _fireType = FireType.A;
    [SerializeField] private float _ignitionTemperature = 100.0f, _brunDuration = 60.0f;
    [SerializeField] private bool _canSpread = true;
    [SerializeField] private GameObject[] _firePrefabs;

    private bool _isOnFire = false;
    private FireSource _fireSource;
    private Renderer _renderer;

    public bool IsOnFire => _isOnFire;

    public void Ignite()
    {
        if (_isOnFire) return;

        GameObject firePrefab = GetFirePrefabByType(_fireType);
        if (firePrefab == null)
        {
            Debug.LogError($"Нет префаба огня для типа {_fireType}");
            return;
        }

        GameObject fireObject = Instantiate(firePrefab, transform.position, Quaternion.identity);
        fireObject.transform.SetParent(transform);
        fireObject.name = $"FireSource_{_fireType}";

        _fireSource = fireObject.GetComponent<FireSource>();
        if (_fireSource != null)
        {
            _fireSource.Type = _fireType;
        }

        _isOnFire = true;
    }

    private GameObject GetFirePrefabByType(FireType type)
    {
        if (_firePrefabs == null || _firePrefabs.Length == 0)
            return null;

        foreach (var prefab in _firePrefabs)
        {
            if (prefab == null) continue;

            FireSource fireSource = prefab.GetComponent<FireSource>();
            if (fireSource != null && fireSource.Type == type)
                return prefab;
        }

        return _firePrefabs.Length > 0 ? _firePrefabs[0] : null;
    }
}
