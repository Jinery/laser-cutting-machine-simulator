using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LaserCutterGrid : MonoBehaviour
{

    [SerializeField] private XRSocketInteractor _materialSocket;

    private CuttableMaterial _currentMaterial;

    public bool HasMaterial => _currentMaterial != null;
    public CuttableMaterial CurrentMaterial => _currentMaterial;

    private void Start()
    {
        _materialSocket.selectEntered.AddListener(OnMaterialPlaced);
        _materialSocket.selectExited.AddListener(OnMaterialRemoved);
    }

    private void OnDestroy()
    {
        _materialSocket.selectEntered.RemoveListener(OnMaterialPlaced);
        _materialSocket.selectExited.RemoveListener(OnMaterialRemoved);
    }

    private void OnMaterialPlaced(SelectEnterEventArgs args)
    {
        _currentMaterial = args.interactableObject.transform.GetComponent<CuttableMaterial>();

        IXRInteractable interactable = args.interactableObject;
        if (interactable.transform.TryGetComponent<CuttableMaterial>(out CuttableMaterial material))
        {
            _currentMaterial = material;
        }
        else
        {
            interactable = null;
        }
    }

    private void OnMaterialRemoved(SelectExitEventArgs arg0)
    {
        _currentMaterial = null;
    }

    public Vector3 GetGridPosition(Vector3 worldPosition) => transform.InverseTransformPoint(worldPosition);
}