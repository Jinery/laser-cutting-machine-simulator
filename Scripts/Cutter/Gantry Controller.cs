using UnityEngine;

public class GantryController : MonoBehaviour
{
    [Tooltip("Min Z in millimeters"), SerializeField] private float _minZ = 0f;
    [Tooltip("Max Z in millimeters"), SerializeField] private float _maxZ = .5f.ToMillimeters();
    
    [HideInInspector]public float MoveSpeed = 0f;

    private float _targetZ = 0f;
    private float _currentZ = 0f;
    private bool _isMoving = false;

    private Vector3 _initialPosition;

    public float CurrentZ => _currentZ;
    public bool IsMoving => _isMoving;

    private void Start()
    {
        _initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isMoving && !Mathf.Approximately(_currentZ, _targetZ))
        {
            float step = MoveSpeed * Time.deltaTime;
            _currentZ = Mathf.MoveTowards(_currentZ, _targetZ, step);
            ApplyPosition();
        }
        else
        {
            _isMoving = false;
        }
    }

    private void ApplyPosition()
    {
        Vector3 localPos = transform.localPosition;
        localPos.z += -_currentZ.ToMeters();
        transform.localPosition = localPos;
    }

    public void MoveToZ(float zPosition)
    {
        _targetZ = Mathf.Clamp(zPosition, _minZ, _maxZ);
        _isMoving = true;
    }

    public void MoveToZInstant(float zPosition)
    {
        _targetZ = Mathf.Clamp(zPosition, _minZ, _maxZ);
        _currentZ = _targetZ;
        ApplyPosition();
        _isMoving = false;
    }

    public void ResetPosition()
    {
        MoveToZInstant(_initialPosition.z);
    }

    private void OnValidate()
    {
        _maxZ = Mathf.Max(_minZ + 10, _maxZ);
    }
}
