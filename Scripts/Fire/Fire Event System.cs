using UnityEngine;
using UnityEngine.Events;

public class FireEventSystem : MonoBehaviour, IFireSystem
{
    private static FireEventSystem _instance;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReportExtinguished(FireSource source)
    {
        throw new System.NotImplementedException();
    }

    public void ReportFire(FireSource source)
    {
        throw new System.NotImplementedException();
    }
}
