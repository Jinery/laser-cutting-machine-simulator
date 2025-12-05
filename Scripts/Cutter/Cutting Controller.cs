using System.Collections;
using UnityEngine;

public class CuttingController : MonoBehaviour
{
    [Header("Controller References")]
    [SerializeField] private LaserCutter _cutter;
    [SerializeField] private GantryController _track;
    [SerializeField] private HeadController _needle;
    [SerializeField] private LaserBeam _laserBeam;
    [SerializeField] private LaserCutterHead _laserHead;

    [Header("Movement Settings")]
    [Tooltip("Cutting speed in millimeters per second."), SerializeField] private float _cuttingSpeed = .05f.ToMillimeters();
    [Tooltip("Fast cutting speed in millimeters per second."), SerializeField] private float _fastMoveSpeed = .15f.ToMillimeters();

    private Coroutine _currentCuttingRoutine;
    private Vector2 _homePosition = Vector2.zero;

    public bool IsCutting => _currentCuttingRoutine != null;

    private void Awake()
    {
        if(_cutter == null) _cutter = GetComponent<LaserCutter>();
        if(_laserHead == null) _laserHead = GetComponentInChildren<LaserCutterHead>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _homePosition = new Vector2(_needle.transform.localPosition.x, _track.transform.localPosition.z);

        if (_cutter != null)
            _cutter.OnPowerStateChanged.AddListener(OnPowerStateChanged);
    }

    private void OnDestroy()
    {
        if (_cutter != null)
        {
            _cutter.OnPowerStateChanged.RemoveListener(OnPowerStateChanged);
        }
    }

    private void OnPowerStateChanged(bool isEnabled)
    {
        if (!isEnabled)
        {
            StopAllCutting();
            ReturnToHomePosition();
        }
    }

    public void StartShapeCutting(Vector2[] points)
    {
        Debug.Log("Starting cutting...");
        if (!ValidateCuttingConditions()) return;

        Debug.Log("Cutting has been started.");

        if (_currentCuttingRoutine != null)
            StopCoroutine(_currentCuttingRoutine);

        _currentCuttingRoutine = StartCoroutine(CutShapeRoutine(points));
    }

    private IEnumerator CutShapeRoutine(Vector2[] points)
    {
        if (points.Length < 2) yield break;

        yield return StartCoroutine(MoveToPosition(points[0], _fastMoveSpeed, false));
        _cutter.StartCutting();

        if (_laserBeam != null) _laserBeam.SetLaserVisible(true);

        for(int pointIndex = 1; pointIndex < points.Length; pointIndex++)
        {
            yield return StartCoroutine(MoveToPosition(points[pointIndex], _cuttingSpeed, true));
        }

        _cutter.StopCutting();
        if (_laserBeam != null) _laserBeam.SetLaserVisible(false);

        yield return StartCoroutine(MoveToPosition(_homePosition, _fastMoveSpeed, false));
        _currentCuttingRoutine = null;
    }

    private IEnumerator MoveToPosition(Vector2 target, float speed, bool isCutting)
    {
        _track.MoveSpeed = speed;
        _needle.MoveSpeed = speed;

        Debug.LogWarning($"Moving to\nTrack: {target.y}\nNeedle: {target.x}");

        _track.MoveToZ(target.y);
        _needle.MoveToX(target.x);

        while(_track.IsMoving || _needle.IsMoving)
        {
            if(IsCutting) UpdateCutting();
            yield return null;
        }
    }

    private void UpdateCutting()
    {
        if(_laserHead != null)
        {
            _laserHead.PerformCutting();
        }
    }

    public void ReturnToHomePosition()
    {
        if (_currentCuttingRoutine != null)
        {
            StopCoroutine(_currentCuttingRoutine);
            _currentCuttingRoutine = null;
        }

        _track.MoveSpeed = _fastMoveSpeed;
        _needle.MoveSpeed = _fastMoveSpeed;
        _track.MoveToZ(_homePosition.y);
        _needle.MoveToX(_homePosition.x);

        _cutter.StopCutting();
        if (_laserBeam != null) _laserBeam.SetLaserVisible(false);
    }

    public void StopAllCutting()
    {
        if (_currentCuttingRoutine != null)
        {
            StopCoroutine(_currentCuttingRoutine);
            _currentCuttingRoutine = null;
        }
        _cutter.StopCutting();
    }

    private bool ValidateCuttingConditions()
    {
        return _cutter.IsEnabled && !_cutter.Door.IsOpened && _cutter.Grid.HasMaterial;
    }
}