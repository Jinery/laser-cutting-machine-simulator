using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ElecricalPanel : MonoBehaviour
{
    public class ElectricalPanelChangeStateEvent : UnityEvent<bool> {  }
    public class ElectricalPanelEvent : UnityEvent {  }

    [SerializeField] private ElectricalPanelLever _lever;

    [SerializeField] private List<Light> _lights = new List<Light>();

    [Header("Events.")]
    [SerializeField] private ElectricalPanelChangeStateEvent _onPanelStateChanged = new ElectricalPanelChangeStateEvent();
    [SerializeField] private ElectricalPanelEvent _onKnockedOut = new ElectricalPanelEvent();

    private bool _isEnabled;

    public ElectricalPanelChangeStateEvent OnPanelStateChanged => _onPanelStateChanged;
    public ElectricalPanelEvent OnKnockedOut => _onKnockedOut;
    public bool IsEnabled => _isEnabled;

    private void Awake()
    {
        if (_lever == null) _lever = GetComponentInChildren<ElectricalPanelLever>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_lever == null) return;
        
        _isEnabled = _lever.IsEnabled;
        _lever.OnLeverStateChanged.AddListener(UpdateState);
        _lever.OnLeverKnockedOut.AddListener(KnockedOut);
    }

    private void OnDestroy()
    {
        if(_lever == null) return;

        if (_lever.OnLeverStateChanged != null) _lever.OnLeverStateChanged.RemoveListener(UpdateState);
        if (_lever.OnLeverKnockedOut != null) _lever.OnLeverKnockedOut.RemoveListener(KnockedOut);
    }

    private void UpdateState(bool state)
    {
        _isEnabled = state;

        _onPanelStateChanged?.Invoke(state);
        UpdateLightsState(state);
    }

    public void HandleKnockOutPlugs()
    {
        if(_lever != null) _lever.KnockOutPlugs();
    }

    private void KnockedOut()
    {
        _isEnabled = false;
        _onKnockedOut?.Invoke();
        UpdateLightsState(false);
    }

    private void UpdateLightsState(bool newState)
    {
        if (_lights != null && _lights.Count > 0) _lights.Where(light => light.enabled != newState)
                .ToList().ForEach(light => light.enabled = newState);
    }
}
