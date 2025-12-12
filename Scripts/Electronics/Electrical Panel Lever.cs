using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ElectricalPanelLever : XRGrabInteractable
{
    public class ElectronicalPanelLeverEvent : UnityEvent {  }
    public class ElectronicalPanelLeverStateChangedEvent : UnityEvent<bool> {  }

    [SerializeField] private bool _isEnabled = false;
    [SerializeField] private float _knockOutPlugsDuration = .2f;
    [SerializeField] private float _minAngle = .0f, _maxAngle = 180f, _snapAngle = 90f, _snapThreshold = 5f, _snapAnimationDuration = 0.1f;

    [Header("Events")]
    [SerializeField] private ElectronicalPanelLeverStateChangedEvent _onLeverStateChanged = new ElectronicalPanelLeverStateChangedEvent();
    [SerializeField] private ElectronicalPanelLeverEvent _onLeverKnockedOut = new ElectronicalPanelLeverEvent();

    private float _currentAngle = 0f, _initialGrabAngle;
    private Vector3 _initialRotation, _initialGrabPoint;
    private bool _isSnapped = false;
    private XRBaseInputInteractor _currentController;

    public ElectronicalPanelLeverStateChangedEvent OnLeverStateChanged => _onLeverStateChanged;
    public ElectronicalPanelLeverEvent OnLeverKnockedOut => _onLeverKnockedOut;

    public bool IsEnabled => _isEnabled;

    private void SetEnabledState(bool value, bool invokeEvents = true)
    {
        if (_isEnabled == value) return;

        _isEnabled = value;

        if (invokeEvents) _onLeverStateChanged?.Invoke(_isEnabled);
    }

    protected override void Awake()
    {
        base.Awake();
        _initialRotation = transform.localEulerAngles;
        _currentAngle = _isEnabled ? _maxAngle : _minAngle;

        movementType = MovementType.Instantaneous;

        ApplyRotation();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            _currentController = inputInteractor;

            _initialGrabPoint = _currentController.transform.position;
            _initialGrabAngle = _currentAngle;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (_currentController != null)
        {
            if (args.interactorObject is XRBaseInputInteractor)
            {
                _currentController = null;
            }
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(_currentController != null && updatePhase is XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            UpdateLeverRotation();
        }
    }

    public void KnockOutPlugs()
    {
        if (!_isEnabled) return;

        if(_currentController != null) _currentController = null;
        _isSnapped = false;
        StartCoroutine(KnockOutPlugsAnim());
    }

    private IEnumerator KnockOutPlugsAnim()
    {
        float startAngle = _currentAngle;
        float elapsedTime = 0f;

        while (elapsedTime < _knockOutPlugsDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _knockOutPlugsDuration;
            _currentAngle = Mathf.Lerp(startAngle, _minAngle, progress);
            ApplyRotation();
            yield return null;
        }

        _currentAngle = _minAngle;
        ApplyRotation();

        SetEnabledState(false);
        _onLeverKnockedOut?.Invoke();
    }

    private void UpdateLeverRotation()
    {
        if (_currentController == null) return;

        Vector3 currentGrabPoint = _currentController.transform.position;

        Vector3 rotationCenter = transform.position;
        Vector3 rotationAxis = transform.right;

        Vector3 initialVector = _initialGrabPoint - rotationCenter;
        Vector3 currentVector = currentGrabPoint - rotationCenter;

        float angleDelta = Vector3.SignedAngle(initialVector, currentVector, rotationAxis);

        _currentAngle = Mathf.Clamp(_initialGrabAngle + angleDelta, _minAngle, _maxAngle);

        CheckSnapState();
        ApplyRotation();
    }

    private void CheckSnapState()
    {
        float targetAngle = Mathf.Abs(_currentAngle - _maxAngle) < Mathf.Abs(_currentAngle - _minAngle) ? _maxAngle : _minAngle;
        
        if(Mathf.Abs(_currentAngle - targetAngle) < _snapAngle && !_isSnapped)
        {
            StartCoroutine(SnapToAngle(targetAngle));

            if(_currentController != null)
            {
                _currentController.SendHapticImpulse(.5f, .05f);
            }

            bool newState = targetAngle == _maxAngle;
            if (newState != _isEnabled)
            {
                SetEnabledState(newState);
            }
        }
        else if (Mathf.Abs(_currentAngle - targetAngle) > _snapThreshold * 2)
        {
            _isSnapped = false;
        }
    }

    private IEnumerator SnapToAngle(float targetAngle)
    {
        _isSnapped = true;
        float startAngle = _currentAngle;
        float elapsedTime = 0f;

        while(elapsedTime < _snapAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _snapAnimationDuration;
            _currentAngle = Mathf.Lerp(startAngle, targetAngle, progress);
            ApplyRotation();
            yield return null;
        }
    }

    private void ApplyRotation()
    {
        Vector3 newRotation = _initialRotation;
        newRotation.x = _initialRotation.x + _currentAngle;
        transform.localEulerAngles = newRotation;
    }
}