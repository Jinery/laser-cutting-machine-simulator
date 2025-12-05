using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class LaserCutter : MonoBehaviour
{
    public class CutterPowerEvent : UnityEvent<bool> {  }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _cutterSound;
    [SerializeField] private LaserBeam _laserBeam;
    [SerializeField] private CuttingController _cuttingController;

    [SerializeField] private CutterPowerEvent _onPowerStateChanged = new CutterPowerEvent();


    private LaserCutterDoor _cutterDoor;
    private LaserCutterGrid _grid;
    private LaserCutterHead _head;
    private bool _isEnabled = false;

    public bool IsEnabled
    {
        get { return _isEnabled; }
        set
        {
            if(value != _isEnabled)
            {
                _isEnabled = value;
                ToggleCutterState(value);
            }
        }
    }

    public LaserCutterGrid Grid => _grid;
    public LaserCutterHead Head => _head;
    public LaserCutterDoor Door => _cutterDoor;
    
    public CutterPowerEvent OnPowerStateChanged => _onPowerStateChanged;

    private void Awake()
    {
        InitializeComponents();

        if(_audioSource != null && _cutterSound != null)
        {
            _audioSource.clip = _cutterSound;
        }
    }

    private void ToggleCutterState(bool cutterState)
    {
        if(_audioSource != null && _audioSource.clip != null)
        {
            if (cutterState) _audioSource.Play(); else _audioSource.Stop();
        }

        if (!cutterState) _cuttingController.ReturnToHomePosition();

        _onPowerStateChanged.Invoke(cutterState);
    }

    public void StartCutting()
    {
        if(_isEnabled)
        {
            if(_cutterDoor.IsOpened || _head.IsCutting)
            {
                // Waitly empty.
                return;
            }

            _head.StartCutting();
        }
        else
        {
            if(_head.IsCutting) _head.StopCutting();
        }
    }

    public void StopCutting()
    {
        if(_head.IsCutting) _head.StopCutting();
    }

    private void Reset()
    {
        InitializeComponents();
        SetupAudioSourceSettings();
    }

    private void InitializeComponents()
    {
        if (_cutterDoor == null)
            _cutterDoor = GetComponentInChildren<LaserCutterDoor>();

        if (_grid == null)
            _grid = GetComponentInChildren<LaserCutterGrid>();

        if (_head == null)
            _head = GetComponentInChildren<LaserCutterHead>();

        if (_laserBeam == null)
            _laserBeam = GetComponentInChildren<LaserBeam>();

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if(_cuttingController == null)
            _cuttingController = GetComponent<CuttingController>();
    }

    private void SetupAudioSourceSettings()
    {
        if (_audioSource != null)
        {
            _audioSource.volume = 0.6f;
            _audioSource.loop = true;
            _audioSource.enabled = _isEnabled;
            _audioSource.spatialBlend = 1;
        }
    }
}
