using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class LearnManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [SerializeField] private TutorialStep[] tutorialSteps;
    
    [Header("UI Reference")]
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private TMP_Text _instructionText, _stepCounterText;
    [SerializeField] private RawImage _videoDisplay;
    [SerializeField] private VideoPlayer _videoPlayer;

    [SerializeField] private Button _nextButton, _skipButton;

    [SerializeField] private VideoClip[] _safetyVideos;
    [SerializeField] private GameObject _safetyVideoPanel;
    [SerializeField] private RawImage _safetyVideoDisplay;
    [SerializeField] private VideoPlayer _safetyVideoPlayer;
    [SerializeField] private TMP_Text _safetyVideoTitle;

    [Header("Events")]
    [SerializeField] private UnityEvent _onTutorialStart = new UnityEvent();
    [SerializeField] private UnityEvent _onTutorialComplete = new UnityEvent();
    [SerializeField] private UnityEvent _onStepChanged = new UnityEvent();

    public UnityEvent OnTutorialStart => _onTutorialStart;
    public UnityEvent OnTutorialComplete => _onTutorialComplete;
    public UnityEvent OnStepChanged => _onStepChanged;

    private int _currentStepIndex = 0;
    private Dictionary<string, TutorialAction.ActionType> _completedActions = new Dictionary<string, TutorialAction.ActionType>();
    private bool _isTutorialActive = false;
    private LaserCutter _laserCutter;
    private LaserCutterButton _powerButton;
    private LaserCutterGrid _materialGrid;
    private CutterPad _cutterPad;

    private string[] safetyVideoTitles = new string[]
    {
        "Опасность лазерного излучения",
        "Пожарная безопасность",
        "Защита глаз",
        "Защита органов дыхания",
        "Общие меры предосторожности"
    };

    private void Awake()
    {
        InitializeComponents();
        SetupUIListeners();

        if (_videoPlayer != null)
        {
            _videoPlayer.targetTexture = 
                new RenderTexture((int)_videoDisplay.rectTransform.rect.width, (int)_videoDisplay.rectTransform.rect.height, 24);
            _videoDisplay.texture = _videoPlayer.targetTexture;
        }

        if (_safetyVideoPlayer != null)
        {
            _safetyVideoPlayer.targetTexture = 
                new RenderTexture((int)_safetyVideoDisplay.rectTransform.rect.width, (int)_safetyVideoDisplay.rectTransform.rect.height, 24);
            _safetyVideoDisplay.texture = _safetyVideoPlayer.targetTexture;
        }
    }

    private void Start()
    {
        if (_powerButton != null)
        {
            if (_powerButton != null)
            {
                _powerButton.OnButtonPressed.AddListener(() => 
                    CompleteAction(_laserCutter.IsEnabled ? TutorialAction.ActionType.PowerOn : TutorialAction.ActionType.PowerOff));
            }
        }

        if (_laserCutter != null)
        {
            _laserCutter.OnPowerStateChanged.AddListener(OnPowerStateChanged);
        }

        StartTutorial();
    }

    private void SetupUIListeners()
    {
        if (_nextButton != null) _nextButton.onClick.AddListener(NextStep);
        if (_skipButton != null) _skipButton.onClick.AddListener(SkipTutorial);
    }

    public void StartTutorial()
    {
        _isTutorialActive = true;
        _currentStepIndex = 0;
        _tutorialPanel.SetActive(true);
        OnTutorialStart?.Invoke();

        LoadStep(_currentStepIndex);
    }

    public void LoadStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= tutorialSteps.Length)
        {
            CompleteTutorial();
            return;
        }

        _currentStepIndex = stepIndex;
        TutorialStep currentStep = tutorialSteps[_currentStepIndex];

        _instructionText.text = currentStep.instructionText;
        _stepCounterText.text = $"Шаг {_currentStepIndex + 1} из {tutorialSteps.Length}";

        if (currentStep.instructionVideo != null && _videoPlayer != null)
        {
            _videoPlayer.clip = currentStep.instructionVideo;
            _videoPlayer.Play();
            _videoDisplay.gameObject.SetActive(true);
        }
        else
        {
            _videoDisplay.gameObject.SetActive(false);
        }

        currentStep.onStepStart?.Invoke();

        if (currentStep.requiredAction != null)
        {
            string actionKey = $"{currentStep.requiredAction.actionType}_{currentStep.requiredAction.targetObjectName}";
            if (_completedActions.ContainsKey(actionKey))
            {
                currentStep.requiredAction.isCompleted = true;
                _nextButton.interactable = true;
            }
            else
            {
                currentStep.requiredAction.isCompleted = false;
                _nextButton.interactable = false;
            }
        }
        else
        {
            _nextButton.interactable = true;
        }

        OnStepChanged?.Invoke();
    }

    public void CompleteAction(TutorialAction.ActionType actionType, string targetName = "")
    {
        if (!_isTutorialActive) return;

        string actionKey = $"{actionType}_{targetName}";
        if (!_completedActions.ContainsKey(actionKey))
        {
            _completedActions.Add(actionKey, actionType);
        }

        TutorialStep currentStep = tutorialSteps[_currentStepIndex];
        if (currentStep.requiredAction != null &&
            currentStep.requiredAction.actionType == actionType &&
            (string.IsNullOrEmpty(currentStep.requiredAction.targetObjectName) ||
             currentStep.requiredAction.targetObjectName == targetName))
        {
            currentStep.requiredAction.isCompleted = true;
            currentStep.onStepComplete?.Invoke();
            _nextButton.interactable = true;

            if (currentStep.requiredAction.isCompleted)
            {
                NextStep();
            }
        }
    }

    public void NextStep()
    {
        if (_currentStepIndex < tutorialSteps.Length - 1)
        {
            LoadStep(_currentStepIndex + 1);
        }
        else
        {
            CompleteTutorial();
        }
    }

    public void SkipTutorial()
    {
        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        _isTutorialActive = false;
        _tutorialPanel.SetActive(false);
        OnTutorialComplete?.Invoke();
    }

    public void ShowSafetyVideo(int videoIndex)
    {
        if (videoIndex >= 0 && videoIndex < _safetyVideos.Length)
        {
            _safetyVideoPanel.SetActive(true);
            _safetyVideoPlayer.clip = _safetyVideos[videoIndex];
            _safetyVideoPlayer.Play();
            _safetyVideoTitle.text = safetyVideoTitles[Mathf.Min(videoIndex, safetyVideoTitles.Length - 1)];
        }
    }

    public void HideSafetyVideo()
    {
        _safetyVideoPanel.SetActive(false);
        _safetyVideoPlayer.Stop();
    }

    public void PlayAllSafetyVideos()
    {
        StartCoroutine(PlaySafetyVideosSequence());
    }

    private IEnumerator PlaySafetyVideosSequence()
    {
        _safetyVideoPanel.SetActive(true);

        for (int i = 0; i < _safetyVideos.Length; i++)
        {
            ShowSafetyVideo(i);
            _safetyVideoTitle.text = safetyVideoTitles[Mathf.Min(i, safetyVideoTitles.Length - 1)];

            yield return new WaitForSeconds((float)_safetyVideos[i].length + 1f);
        }

        HideSafetyVideo();
    }

    private void OnPowerStateChanged(bool isPowered)
    {
        CompleteAction(isPowered ? TutorialAction.ActionType.PowerOn : TutorialAction.ActionType.PowerOff, "LaserCutter");
    }

    private void InitializeComponents()
    {
        _laserCutter = FindFirstObjectByType<LaserCutter>();
        _powerButton = FindFirstObjectByType<LaserCutterButton>();
        _materialGrid = FindFirstObjectByType<LaserCutterGrid>();
        _cutterPad = FindFirstObjectByType<CutterPad>();
    }

    [ContextMenu("Create Default Tutorial Steps")]
    private void CreateDefaultTutorialSteps()
    {
        List<TutorialStep> steps = new List<TutorialStep>();

        steps.Add(new TutorialStep
        {
            stepName = "Введение",
            instructionText = "Добро пожаловать в обучение по работе с лазерным станком. Это оборудование требует осторожности и соблюдения техники безопасности."
        });

        steps.Add(new TutorialStep
        {
            stepName = "Включение станка",
            instructionText = "Нажмите красную кнопку питания, чтобы включить лазерный станок.",
            requiredAction = new TutorialAction
            {
                actionType = TutorialAction.ActionType.PowerOn
            }
        });

        steps.Add(new TutorialStep
        {
            stepName = "Установка материала",
            instructionText = "Поместите материал для резки на решётку лазерного станка.",
            requiredAction = new TutorialAction
            {
                actionType = TutorialAction.ActionType.PlaceMaterial
            }
        });

        steps.Add(new TutorialStep
        {
            stepName = "Проверка безопасности",
            instructionText = "Перед началом резки убедитесь, что:\n1. Защитная дверца закрыта\n3. В помещении нет легковоспламеняющихся материалов"
        });

        steps.Add(new TutorialStep
        {
            stepName = "Выбор формы",
            instructionText = "Для начала резки на панели управления выберите форму для резки.",
            requiredAction = new TutorialAction
            {
                actionType = TutorialAction.ActionType.StartCutting
            }
        });

        tutorialSteps = steps.ToArray();
    }

    private void Reset()
    {
        CreateDefaultTutorialSteps();
    }

    private void OnDestroy()
    {
        if (_powerButton != null)
        {
            _powerButton.OnButtonPressed.RemoveAllListeners();
            _powerButton.OnButtonReleased.RemoveAllListeners();
        }

        if (_laserCutter != null)
        {
            _laserCutter.OnPowerStateChanged.RemoveListener(OnPowerStateChanged);
        }
    }
}
