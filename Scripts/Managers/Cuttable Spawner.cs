using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CuttableSpawner : MonoBehaviour
{
    [SerializeField] private CuttableMaterial[] _cuttablePrefabs;
    [SerializeField] private int _maxCuttableObjectsInZone = 3;

    [SerializeField] private List<CuttableMaterial> _cuttableObjectsInZone;
    private BoxCollider _spawnZone;

    void Start()
    {
        _cuttableObjectsInZone = new List<CuttableMaterial>();
        _spawnZone = GetComponent<BoxCollider>();
        _spawnZone.isTrigger = true;

        for (int index = 0; index < _maxCuttableObjectsInZone; index++)
        {
            SpawnPrefab();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CuttableMaterial>(out CuttableMaterial cuttableObject))
        {
            _cuttableObjectsInZone.Add(cuttableObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CuttableMaterial>(out CuttableMaterial cuttableObject))
        {
            _cuttableObjectsInZone.Remove(cuttableObject);

            if (_cuttableObjectsInZone.Count < _maxCuttableObjectsInZone)
            {
                SpawnPrefab();
            }
        }
    }

    private void SpawnPrefab()
    {
        if (_cuttablePrefabs.Length == 0) return;

        CuttableMaterial prefabForSpawn = SelectRandomPrefab();
        Vector3 spawnPosition = GetRandomSpawnPosition();

        CuttableMaterial spawnedObject = Instantiate(prefabForSpawn, spawnPosition, Quaternion.identity);
        _cuttableObjectsInZone.Add(spawnedObject);
    }

    private CuttableMaterial SelectRandomPrefab()
    {
        return _cuttablePrefabs[Random.Range(0, _cuttablePrefabs.Length)];
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = _spawnZone.center + transform.position;
        Vector3 size = _spawnZone.size * 0.5f;

        float x = Random.Range(center.x - size.x, center.x + size.x);
        float y = Random.Range(center.y - size.y, center.y + size.y);
        float z = Random.Range(center.z - size.z, center.z + size.z);

        return new Vector3(x, y, z);
    }

    private void OnDrawGizmos()
    {
        if (_spawnZone == null)
            _spawnZone = GetComponent<BoxCollider>();

        if (_spawnZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + _spawnZone.center, _spawnZone.size);
        }
    }
}
