using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [Range(0.1f, 10), SerializeField] private float _maxLength = 10f;
    [SerializeField] private LayerMask _collisionLayer;

    private float _currentLength = 0;

    public float CurrentLength
    {
        get 
        {
            if(_currentLength <= 0 || _currentLength > _maxLength)
            {
                _currentLength = _maxLength;
            }
            return _currentLength;
        }
        set
        {
            if(value != _currentLength)
            {
                value = Mathf.Clamp(value, 0.1f, _maxLength);
                _currentLength = value;
            }
        }
    }

    private void Awake()
    {
        if(_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
        SetLaserVisible(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(_lineRenderer.enabled)
        {
            UpdateLaserBeamPosition();
        }
    }

    public void SetLaserVisible(bool visible)
    {
        if(_lineRenderer != null) _lineRenderer.enabled = visible;
    }

    private void UpdateLaserBeamPosition()
    {
        if (_lineRenderer == null) return;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + -transform.up * CurrentLength;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, CurrentLength, _collisionLayer))
        {
            endPos = hit.point;
        }

        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    private void Reset()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnValidate()
    {
        CurrentLength = _maxLength;
        UpdateLaserBeamPosition();
    }
}
