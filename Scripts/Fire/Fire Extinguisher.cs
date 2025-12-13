using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FireExtinguisher : InteractableObject
{
    [Header("Extinguisher Settings.")]
    [SerializeField] private ExtinguisherType _type = ExtinguisherType.Water;
    [SerializeField] private ParticleSystem _extinguisherParticles;
    [SerializeField] private AudioSource _extingushingAudio;
    [SerializeField] private MeshRenderer _renderer;

    [SerializeField] private ExtinguisherCollider _extingusherCollider;

    private bool _isActivated = false;
    private XRBaseInputInteractor _currentInteractor;

    public ExtinguisherType Type => _type;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        if (_extingusherCollider != null) _extingusherCollider.enabled = false;
        if (_extinguisherParticles != null) _extinguisherParticles.Stop();
        SetupColor();
    }

    private void SetupColor()
    {
        ParticleSystem.MainModule main = _extinguisherParticles.main;
        Color newColor = _type.GetColorByType();
        if(_renderer != null)
        {
            _renderer.material.color = newColor;
        }

        main.startColor = newColor;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            _currentInteractor = inputInteractor;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if(args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            _currentInteractor = null;

            if (_isActivated)
            {
                DeactivateExtiguisher();
            }
        }
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);

        if(!_isActivated)
        {
            ActivateExtinguisher();
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        if(_isActivated)
        {
            DeactivateExtiguisher();
        }
    }

    private void ActivateExtinguisher()
    {
        _isActivated = true;
        _extingusherCollider.enabled = true;

        if (_extinguisherParticles != null)
            _extinguisherParticles.Play();
        if(_extingushingAudio != null)
            _extingushingAudio.Play();

        StartCoroutine(ConsumeExtinguisher());
    }

    private void DeactivateExtiguisher()
    {
        if (!_isActivated) return;
        _isActivated = false;
        _extingusherCollider.enabled = false;

        if (_extinguisherParticles != null && _extinguisherParticles.isPlaying) 
            _extinguisherParticles.Stop();
        if (_extingushingAudio != null && _extingushingAudio.isPlaying) _extingushingAudio.Stop();
    }

    private IEnumerator ConsumeExtinguisher()
    {
        while(_isActivated)
        {
            yield return null;
        }
    }

    public void SendHapticFeedback(float amplitude, float duration = .1f)
    {
        if(_currentInteractor != null)
        {
            _currentInteractor.SendHapticImpulse(amplitude, duration);
        }
    }
}
