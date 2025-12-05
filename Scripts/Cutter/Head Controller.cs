
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [Tooltip("Min X in millimeters"), SerializeField] private float _minX = 0f;
    [Tooltip("Max X in millimeters"), SerializeField] private float _maxX = .1f.ToMillimeters();

    [HideInInspector] public float MoveSpeed = 0f;

    private float _targetX = 0f;
    private float _currentX = 0f;
    private bool _isMoving = false;

    public float CurrentX => _currentX;
    public bool IsMoving => _isMoving;

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if(_isMoving && !Mathf.Approximately(_currentX, _targetX))
        {
            float step = MoveSpeed * Time.deltaTime;
            _currentX = Mathf.MoveTowards(_currentX, _targetX, step);
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
        localPos.x += _currentX.ToMeters();
        transform.localPosition = localPos;
    }

    public void MoveToX(float xPosition)
    {
        _targetX = Mathf.Clamp(xPosition, _minX, _maxX);
        _isMoving = true;
    }

    public void MoveToXInstant(float xPosition)
    {
        _targetX = Mathf.Clamp(xPosition, _minX, _maxX);
        _currentX = _targetX.ToMillimeters();
        ApplyPosition();
        _isMoving = false;
    }

    public void ResetPosition()
    {
        MoveToXInstant(_initialPosition.x);
    }

    private void OnValidate()
    {
        _maxX = Mathf.Max(_minX + 10f, _maxX);   
    }
}
