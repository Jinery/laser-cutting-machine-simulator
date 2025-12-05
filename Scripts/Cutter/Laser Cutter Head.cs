using UnityEngine;

public class LaserCutterHead : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 0.1f;
    [SerializeField] private Transform _laserOrigin;
    [SerializeField] private LayerMask _cuttableLayer;

    private LaserBeam _laserBeam;
    private LaserCutter _cutter;
    private bool _isCutting = false;
    private Vector3 _targetPosition;

    public bool IsCutting => _isCutting;

    private void Awake()
    {
        if(_laserBeam == null || _cutter == null) InitializeComponents();
        _targetPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, _moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 newPos)
    {
        _targetPosition = newPos;
    }

    public void StartCutting()
    {
        _isCutting = true;
        if (_laserBeam != null)
            _laserBeam.SetLaserVisible(true);
    }

    public void StopCutting()
    {
        _isCutting = false;
        if (_laserBeam != null)
            _laserBeam.SetLaserVisible(false);
    }

    public void PerformCutting()
    {
        if (_laserOrigin == null || _cutter.Grid.CurrentMaterial == null) return;

        RaycastHit hit;
        if(Physics.Raycast(_laserOrigin.position, -_laserOrigin.up, out hit, _laserBeam.CurrentLength, _cuttableLayer))
        {
            if(hit.collider.TryGetComponent<CuttableMaterial>(out CuttableMaterial material))
            {
                Debug.Log($"hit at: {hit.point}");
                Vector3 gridPos = _cutter.Grid.GetGridPosition(hit.point);
                material.CutAtPosition(gridPos);
            }
            else if(hit.collider != null)
            {
                Debug.Log($"Hit {hit.collider.name}\nPos: {hit.point}");
            }
        }
    }

    private void Reset()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _cutter = GetComponentInParent<LaserCutter>();
        _laserBeam = GetComponentInChildren<LaserBeam>();
    }
}
