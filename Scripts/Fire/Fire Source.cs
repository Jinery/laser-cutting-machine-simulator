using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

public class FireSource : MonoBehaviour
{
    [Header("Fire Class Settings.")]
    [SerializeField] private FireType _fireType = FireType.A;
    [SerializeField] private float _maxIntensity = 10.0f, _spreadRadius = 5.0f, _spreadInterval = 3.0f;

    [Header("Visual effects.")]
    [SerializeField] private ParticleSystem _fireParticles;
    [SerializeField] private ParticleSystem _smokeParticles;
    [SerializeField] private Light _fireLight;
    [SerializeField] private AudioSource _fireAudio;

    [SerializeField] private ParticleSystem _sparkPartiles, _metalSparkParticles;
    [SerializeField] private Material _burntMaterial;
    [SerializeField] private Renderer _objectRenderer;

    [Header("Spread")]
    [SerializeField] private GameObject _firePrefab;


    private float _currentIntensity = 0f;
    private bool _isActive = false, _isExtingushed = false;
    private Material _originalMaterial;
    private List<FireSource> _childFires = new List<FireSource>();
    private Collider _fireCollider;

    private float _extinguishProgress = 0f;

    private const float EXTINGUISH_THRESHOLD = 100f;

    public FireType Type
    {
        get => _fireType;
        set => _fireType = value;
    }
    public float Intensity => _currentIntensity;
    public bool IsActive => _isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fireCollider = GetComponent<Collider>();
        if(_objectRenderer != null)
            _originalMaterial = _objectRenderer.material;

        InitializeFire();
    }

    private void InitializeFire()
    {
        SetupSpecifiedEffects();

        StartCoroutine(GlowFire());
    }

    private void SetupSpecifiedEffects()
    {
        switch(_fireType)
        {
            case FireType.A:
                if(_smokeParticles != null)
                {
                    _smokeParticles.Play();
                }
                break;
            case FireType.B:
                if(_fireParticles != null)
                {
                    ParticleSystem.MainModule main = _fireParticles.main;
                    main.startColor = new Color(1f, .6f, .2f);
                }
                break;
            case FireType.C:
                if(_fireParticles != null)
                {
                    ParticleSystem.MainModule main = _fireParticles.main;
                    main.startColor = Color.blue;
                }
                if(_sparkPartiles != null)
                {
                    _sparkPartiles.Play();
                }
                break;
            case FireType.D:
                if (_fireParticles != null)
                {
                    ParticleSystem.MainModule main = _fireParticles.main;
                    main.startColor = Color.white;
                }
                if(_metalSparkParticles != null)
                {
                    _metalSparkParticles.Play();
                }
                break;
        }
    }

    private IEnumerator GlowFire()
    {
        _currentIntensity = 1f;
        _isActive = true;

        UpdateFireEffects();

        FireEventSystem.Instance?.ReportFire(this);
        yield return new WaitForSeconds(_spreadInterval);

        while(_isActive && _currentIntensity > 0f)
        {
            if(_currentIntensity >= _maxIntensity * .5f)
            {
                SpreadFire();
            }
            yield return new WaitForSeconds(_spreadInterval);
        }
    }

    private void SpreadFire()
    {
        float spreadChance = _fireType.GetSpreadChance();
        if(URandom.value > spreadChance)
        {
            List<Collider> hitColliders = Physics.OverlapSphere(transform.position, _spreadRadius).ToList();
            hitColliders.ForEach(collider =>
            {
                Flammable flammable = collider.GetComponent<Flammable>();
                if (flammable != null && !flammable.IsOnFire)
                {
                    RaycastHit hit;
                    Vector3 direction = collider.transform.position - transform.position;
                    if (Physics.Raycast(transform.position, direction.normalized, out hit, _spreadRadius))
                    {
                        if (hit.collider == collider)
                        {
                            flammable.Ignite();
                        }
                    }
                }
            });
        }
    }

    public void Extinguish(FireExtinguisher extinguisher)
    {
        if (!_isActive || _isExtingushed) return;

        ExtinguisherType extinguisherType = extinguisher.Type;
        float effectivness = extinguisherType.GetEffectiveness(_fireType);

        if (effectivness < 0)
        {
            HandleWrongExtinguisher(extinguisherType, effectivness);
            return;
        }

        ApplyExtinguishing(effectivness, extinguisher);
    }

    private void HandleWrongExtinguisher(ExtinguisherType extinguisherType, float effectivness)
    {
        _currentIntensity = Mathf.Min(_currentIntensity + Mathf.Abs(effectivness) * .1f, _maxIntensity);

        switch(_fireType)
        {
            case FireType.B when extinguisherType == ExtinguisherType.Water:
                CreateExplosionEffect();
                break;
            case FireType.D when extinguisherType == ExtinguisherType.Water:
                CreateMetalExplosion();
                break;
        }

        UpdateFireEffects();
    }

    private void ApplyExtinguishing(float effectivness, FireExtinguisher extinguisher)
    {
        _currentIntensity -= effectivness * Time.deltaTime * .1f;


        _extinguishProgress += effectivness * Time.deltaTime;

        if(extinguisher != null)
        {
            extinguisher.SendHapticFeedback(.3f);
        }

        if (_currentIntensity <= 0)
        {
            CompleteExtinguish();
        }
        else
        {
            UpdateFireEffects();
        }
    }

    private void CompleteExtinguish()
    {
        _isActive = false;
        _isExtingushed = true;
        _currentIntensity = 0f;
        UpdateFireEffects();

        if(_objectRenderer != null && _burntMaterial != null) _objectRenderer.material = _burntMaterial;

        _childFires.Where(child => child != null).ToList()
            .ForEach(child => child.ForceExtinguish());
    }

    private void CreateExplosionEffect()
    {
        GameObject explosion = new GameObject("ExplosionEffect");
        explosion.transform.position = transform.position;
        
        ParticleSystem explosionPs = explosion.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = explosionPs.main;
        main.startSize = 3f;
        main.startSpeed = 10f;
        main.startLifetime = 2f;

        Destroy(explosion, 2f);
    }

    private void CreateMetalExplosion()
    {

    }

    private void ForceExtinguish()
    {
        CompleteExtinguish();
    }

    private void UpdateFireEffects()
    {
        if(_fireParticles != null)
        {
            ParticleSystem.EmissionModule emission = _fireParticles.emission;
            emission.rateOverTime = _currentIntensity * 50f;

            ParticleSystem.MainModule main = _fireParticles.main;
            main.startSize = _currentIntensity * .5f;
        }

        if(_fireLight != null)
        {
            _fireLight.intensity = _currentIntensity * 5f;
            _fireLight.range = _currentIntensity * 2f;
        }

        if(_fireAudio != null)
        {
            _fireAudio.volume = _currentIntensity * .3f;
            _fireAudio.pitch = .8f + _currentIntensity * .5f;

            if (!_fireAudio.isPlaying && _currentIntensity > 0) _fireAudio.Play();
            else if (_fireAudio.isPlaying && _currentIntensity <= 0) _fireAudio.Stop();
        }
    }
}
