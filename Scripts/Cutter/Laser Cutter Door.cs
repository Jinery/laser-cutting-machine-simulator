using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LaserCutterDoor : XRGrabInteractable
{
    [SerializeField] private bool _isOpened = false;

    [Header("Angle settings")]
    [SerializeField] private float _minAngle = 0f;
    [SerializeField] private float _maxAngle = 90f;

    private float _currentAngle = 0f, _initialGrabAngle;
    private Vector3 _initialRotation, _initialGrabPoint;
    private XRBaseInteractor _currentInteractor;

    public bool IsOpened => _isOpened;

    protected override void Awake()
    {
        base.Awake();
        _initialRotation = transform.localEulerAngles;
        _currentAngle = IsOpened ? _maxAngle : _minAngle;

        movementType = MovementType.Instantaneous;

        ApplyRotation();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _currentInteractor = args.interactorObject as XRBaseInteractor;

        _initialGrabPoint = _currentInteractor.transform.position;
        _initialGrabAngle = _currentAngle;

        _isOpened = _currentAngle > (_maxAngle - _minAngle) * 0.5f;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (_currentInteractor != null) _currentInteractor = null;
        _isOpened = _currentAngle > (_maxAngle - _minAngle) * 0.5f;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(_currentInteractor != null && updatePhase is XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            UpdateDoorRotation();
        }
    }

    private void UpdateDoorRotation()
    {
        if (_currentInteractor == null) return;

        Vector3 currentGrabPoint = _currentInteractor.transform.position;

        Vector3 rotationCenter = transform.position;
        Vector3 rotationAxis = transform.right;

        Vector3 initialVector = _initialGrabPoint - rotationCenter;
        Vector3 currentVector = currentGrabPoint - rotationCenter;

        float angleDelta = Vector3.SignedAngle(initialVector, currentVector, rotationAxis);

        _currentAngle = Mathf.Clamp(_initialGrabAngle + angleDelta, _minAngle, _maxAngle);

        ApplyRotation();
        _isOpened = _currentAngle > (_maxAngle - _minAngle) * 0.5f;
    }

    private void ApplyRotation()
    {
        Vector3 newRotation = _initialRotation;
        newRotation.x = _initialRotation.x + _currentAngle;
        transform.localEulerAngles = newRotation;
    }
}
