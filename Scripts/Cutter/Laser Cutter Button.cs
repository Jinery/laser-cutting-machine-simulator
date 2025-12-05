using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent (typeof(AudioSource))]
public class LaserCutterButton : XRSimpleInteractable
{
    public class CutterButtonEvent : UnityEvent { }

    [Header("Button Settings")]
    [SerializeField] private float _buttonMovement = 0.02f;
    [SerializeField] private Transform _buttonVisual;

    [Header("Visual Feedback")]
    [SerializeField] private Material _enabledMaterial;
    [SerializeField] private Material _disabledMaterial;
    [SerializeField] private Renderer _buttonRenderer;

    [Header("Audio")]
    [SerializeField] private AudioClip _clickSound;

    [Header("Events")]
    [SerializeField] private CutterButtonEvent _onButtonPressed = new CutterButtonEvent();
    [SerializeField] private CutterButtonEvent _onButtonReleased = new CutterButtonEvent();

    private LaserCutter _laserCutter;
    private AudioSource _audioSource;
    private Vector3 _initialPosition;
    private bool _isPressed = false;
    private bool _isEnabled = false;

    public CutterButtonEvent OnButtonPressed => _onButtonPressed;
    public CutterButtonEvent OnButtonReleased => _onButtonReleased;

    protected override void Awake()
    {
        base.Awake();
        selectMode = InteractableSelectMode.Single;
        InitializeComponents();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_buttonVisual != null)
            _initialPosition = _buttonVisual.localPosition;

        if (_buttonRenderer != null)
            _buttonRenderer.material = _isEnabled ? _enabledMaterial : _disabledMaterial;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (!_isPressed)
        {
            PressButton();
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (_isPressed)
        {
            ReleaseButton();
        }
    }

    private void PressButton()
    {
        _isPressed = true;
        if (_buttonVisual != null)
        {
            Vector3 pressedPosition = _initialPosition + transform.forward * _buttonMovement;
            _buttonVisual.localPosition = pressedPosition;
        }

        if(_buttonRenderer != null)
        {
            _buttonRenderer.material = _isEnabled ? _enabledMaterial : _disabledMaterial;
        }

        if (_audioSource != null && _clickSound != null)
        {
            _audioSource.PlayOneShot(_clickSound);
        }

        OnButtonPressed?.Invoke();
        ToggleLaserCutter();
    }

    private void ReleaseButton()
    {
        _isPressed = false;

        if (_buttonVisual != null)
        {
            _buttonVisual.localPosition = _initialPosition;
        }

        OnButtonReleased?.Invoke();
    }

    private void ToggleLaserCutter()
    {
        if (_laserCutter != null)
        {
            _isEnabled = !_isEnabled;
            _laserCutter.IsEnabled = _isEnabled;
            UpdateButtonAppearance();
        }
    }

    private void UpdateButtonAppearance()
    {
        if (_buttonRenderer != null && _enabledMaterial != null && _disabledMaterial != null)
        {
            _buttonRenderer.material = _isEnabled ? _enabledMaterial : _disabledMaterial;
        }
    }

    protected override void Reset()
    {
        base.Reset();
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if(_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_laserCutter == null) _laserCutter = GetComponentInParent<LaserCutter>();
        if (_buttonVisual == null) _buttonVisual = this.transform;
    }

    private void OnDrawGizmosSelected()
    {
        if (_buttonVisual != null)
        {
            Gizmos.color = Color.red;
            Vector3 worldInitialPos = transform.TransformPoint(_initialPosition);
            Vector3 worldPressedPos = worldInitialPos + transform.forward * _buttonMovement;

            Gizmos.DrawWireSphere(worldInitialPos, 0.005f);
            Gizmos.DrawWireSphere(worldPressedPos, 0.005f);
            Gizmos.DrawLine(worldInitialPos, worldPressedPos);
        }
    }
}
