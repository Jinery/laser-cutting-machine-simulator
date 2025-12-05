using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutterPad : MonoBehaviour
{
    [SerializeField] private ShapeCard _cardPrefab;
    [SerializeField] private Canvas _padCanvas;
    [SerializeField] private ShapeLibrary _library;

    [SerializeField] private TMP_Text _infoText;

    private GridLayoutGroup _cardContainer;
    private LaserCutter _laserCutter;
    private CuttingController _cuttingController;

    private List<ShapeCard> _activeCards = new List<ShapeCard>();

    private void Awake()
    {

        InitializeComponents();

        if (_infoText != null)
            _infoText.enabled = false;

        Debug.Log("CutterPad Awake finished");
    }

    private void OnDestroy()
    {
        if (_laserCutter != null) _laserCutter.OnPowerStateChanged.RemoveListener(OnCutterPowerStateChanged);
    }

    private void OnCutterPowerStateChanged(bool isEnabled)
    {
        if (isEnabled)
        {
            _padCanvas.enabled = true;
            GenerateCards();
        }
        else
        {
            _padCanvas.enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_laserCutter != null)
        {
            _laserCutter.OnPowerStateChanged.AddListener(OnCutterPowerStateChanged);
            _padCanvas.enabled = _laserCutter.IsEnabled;
        }
    }

    public void GenerateCards()
    {
        ClearAll();

        Debug.Log($"Library: {_library != null}, Shapes count: {(_library?.Shapes?.Count ?? 0)}");

        _infoText.enabled = false;
        if (_library == null)
        {
            Debug.LogError("Library is null!");
            _infoText.enabled = true;
            _infoText.SetText("Библиотека рисунков не найдена.");
            return;
        }

        if (_library.Shapes == null || _library.Shapes.Count == 0)
        {
            Debug.LogError("No shapes in library!");
            _infoText.enabled = true;
            _infoText.SetText("В библиотеке нету ни одного рисунка.");
            return;
        }

        Debug.Log($"Generating {_library.Shapes.Count} cards");

        foreach (PresetShape preset in _library.Shapes)
        {
            if (preset == null)
            {
                Debug.LogWarning("Found null preset in library");
                continue;
            }

            Debug.Log($"Creating card for: {preset.shapeName}");
            ShapeCard newCard = Instantiate(_cardPrefab, _cardContainer.transform);
            newCard.SetupCard(preset);
            newCard.OnCardClick.AddListener(OnCardClick);
            _activeCards.Add(newCard);
        }

        Debug.Log($"Created {_activeCards.Count} cards");
    }

    private void OnCardClick(ShapeCard card)
    {
        Debug.LogWarning($"Clicked at card {card.GetCardTitle()}");
        if (_cuttingController != null)
        {
            _cuttingController.StartShapeCutting(card.Preset.points);
            Debug.LogWarning("Shape cutting method invoked");
        }
        else
        {
            Debug.LogError("Cutting controller is null.");
        }
    }

    public void ClearAll()
    {
        Debug.LogWarning("Removing all cards.");
        foreach(ShapeCard card in _activeCards)
        {
            if(card == null) continue;

            Debug.LogWarning($"Removing card: {card.name}");
            card.OnCardClick.RemoveListener(OnCardClick);
            Destroy(card.gameObject);
        }

        Debug.LogWarning($"Elements on array: {_activeCards.Count}\n " + string.Join('\n', _activeCards));
        _activeCards.Clear();
        Debug.LogWarning($"New Elements on array: {_activeCards.Count}");
    }

    private void InitializeComponents()
    {
        _cardContainer = _padCanvas.GetComponentInChildren<GridLayoutGroup>();
        _laserCutter = GetComponentInParent<LaserCutter>();
        _cuttingController = GetComponentInParent<CuttingController>();
    }
}
