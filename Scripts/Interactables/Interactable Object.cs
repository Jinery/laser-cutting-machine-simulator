using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class InteractableObject : XRGrabInteractable
{
    [SerializeField] private AudioClip _dropSound;

    [Range(0.01f, 1f), SerializeField] private float _minForceForTriggerSound = 0.1f;

    private Rigidbody _rb;
    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();

        interactionManager = FindFirstObjectByType<XRInteractionManager>();

        if(_rb == null) SetupRigidbody();

        if (_dropSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_dropSound != null)
        {
            if(_rb.linearVelocity.magnitude > _minForceForTriggerSound)
            {
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();

                float volume = _audioSource.volume * Mathf.Clamp01((_rb.linearVelocity.magnitude - _minForceForTriggerSound) 
                    / (_rb.linearVelocity.magnitude - (1 - _minForceForTriggerSound)));
                _audioSource.PlayOneShot(_dropSound, volume);
            }
        }
    }

    private void Update()
    {
        if(_rb.position.y < -100f)
        {
            _rb.position = new Vector3(0, 1, 0);
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    protected override void Reset()
    {
        base.Reset();
        SetupRigidbody();
    }

    private void SetupRigidbody()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
