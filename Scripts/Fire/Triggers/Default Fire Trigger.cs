using System.Collections;
using UnityEngine;

public class DefaultFireTrigger : MonoBehaviour
{
    [SerializeField] protected FireType _spawnedFireType = FireType.A;
    [SerializeField] protected float _triggerDelay = .0f;

    protected void TriggerFire()
    {
        StartCoroutine(TriggerFireWithDelay());
    }

    private IEnumerator TriggerFireWithDelay()
    {
        yield return new WaitForSeconds(_triggerDelay);
        GameObject fireObject = new GameObject($"TriggeredFire_{_spawnedFireType.ToString()}");
        fireObject.transform.position = transform.position;

        FireSource fireSource = fireObject.AddComponent<FireSource>();
        fireSource.Type = _spawnedFireType;
    }
}
