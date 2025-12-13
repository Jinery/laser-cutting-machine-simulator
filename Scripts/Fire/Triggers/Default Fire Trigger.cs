using System.Collections;
using UnityEngine;

public class DefaultFireTrigger : MonoBehaviour
{
    [SerializeField] protected FireType _spawnedFireType = FireType.A;
    [SerializeField] protected float _triggerDelay = .0f;
    [SerializeField] protected GameObject[] _firePrefabs;

    protected void TriggerFire()
    {
        StartCoroutine(TriggerFireWithDelay());
    }

    private IEnumerator TriggerFireWithDelay()
    {
        yield return new WaitForSeconds(_triggerDelay);
        GameObject firePrefab = GetFirePrefabByType(_spawnedFireType);
        Debug.Log("Fire started.");

        if (firePrefab == null)
        {
            Debug.LogError($"Prefab for firy type: {_spawnedFireType} not found.");
            yield break;
        }

        GameObject fireObject = Instantiate(firePrefab, transform.position, Quaternion.identity);
        fireObject.name = $"TriggeredFire_{_spawnedFireType}";
        FireSource fireSource = fireObject.GetComponent<FireSource>();
        if (fireSource != null)
        {
            fireSource.Type = _spawnedFireType;
            fireSource.FirePrefabs = _firePrefabs;
        }
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

        return _firePrefabs[0];
    }
}
