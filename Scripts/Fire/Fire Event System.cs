using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using URandom = UnityEngine.Random;

public class FireEventSystem : MonoBehaviour, IFireSystem
{
    private static FireEventSystem _instance;

    [SerializeField] private ElecricalPanel _elecricalPanel;
    [SerializeField] private AlarmSystem _alarmSystem;

    private Dictionary<FireType, List<FireSource>> _activeFiresByClass = new Dictionary<FireType, List<FireSource>>();

    public static FireEventSystem Instance => _instance;

    public event UnityAction<FireSource> onFireStarted;
    public event UnityAction<FireSource> onFireExtinguished;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        foreach(FireType type in Enum.GetValues(typeof(FireType)))
        {
            _activeFiresByClass[type] = new List<FireSource>();
        }
    }

    public void ReportFire(FireSource source)
    {
        FireType fireType = source.Type;
        if(_activeFiresByClass.TryGetValue(fireType, out List<FireSource> fireSources))
        {
            if(!fireSources.Contains(source))
            {
                _activeFiresByClass[fireType].Add(source);
                onFireStarted?.Invoke(source);

                if(GetTotalActiveFires() == 1)
                {
                    StartCoroutine(ActivateAlarm());
                }
            }
        }
    }

    public void ReportExtinguished(FireSource source)
    {
        FireType fireType = source.Type;
        if (_activeFiresByClass.TryGetValue(fireType, out List<FireSource> fireSources))
        {
            if (fireSources.Contains(source))
            {
                _activeFiresByClass[fireType].Remove(source);
                onFireExtinguished?.Invoke(source);

                if (GetTotalActiveFires() == 0)
                {
                    DeactivateAlarm();
                }
            }
        }
    }

    private IEnumerator ActivateAlarm()
    {
        yield return new WaitForSeconds(URandom.Range(5f, 10f));
        if(_alarmSystem != null) _alarmSystem.Activate();
        yield return new WaitForSeconds(2f);
        if (_elecricalPanel != null) _elecricalPanel.HandleKnockOutPlugs();
    }

    private void DeactivateAlarm() => _alarmSystem?.Deactivate();

    private int GetTotalActiveFires()
    {
        int total = 0;
        _activeFiresByClass.Select(dict => dict.Value).ToList().ForEach(list => total += list.Count);
        return total;
    }
}
