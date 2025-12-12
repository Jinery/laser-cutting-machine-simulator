using UnityEngine;
using URandom = UnityEngine.Random;

public class CutterFireTrigger : DefaultFireTrigger
{
    [SerializeField] private float _checkDelay = 2f;
    [Range(0.01f, 1.0f), SerializeField] private float _fireChance = 0.2f;
    private float _currentTimer = 0f;

    private void Start()
    {
        _spawnedFireType = FireType.D;
    }

    private void Update()
    {
        _currentTimer += Time.deltaTime;
        if(_currentTimer >= _checkDelay)
        {
            if(URandom.value <= _fireChance)
            {
                TriggerFire();
            }
            _currentTimer = 0f;
        }
    }
}
