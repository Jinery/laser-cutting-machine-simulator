using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShapeCard : MonoBehaviour, IPointerClickHandler
{
    public class ShapeCardEvent : UnityEvent<ShapeCard> {  }

    [SerializeField] private Image cardIcon;
    [SerializeField] private TMP_Text cardTitle;
    [SerializeField] private ShapeCardEvent _onCardClick = new ShapeCardEvent();

    private PresetShape _preset;

    public PresetShape Preset => _preset;

    public ShapeCardEvent OnCardClick => _onCardClick;

    public void SetupCard(PresetShape preset)
    {
        _preset = preset;
        if(cardIcon != null) cardIcon.sprite = preset.shapeImage;
        if (cardTitle != null)
        {
            cardTitle.text = preset.shapeName.Length > 20 ? preset.shapeName.Substring(0, 20).Trim() + "..." : preset.shapeName;
            Debug.LogWarning($"Setting card text: {cardTitle.text ?? "Empty"}");
        }
        else
        {
            Debug.LogWarning("Card title component is null");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogWarning($"Card clicked.");
        _onCardClick?.Invoke(this);
    }

    public string GetCardTitle() => cardTitle != null && !string.IsNullOrEmpty(cardTitle.text) ? cardTitle.text : "Null.";
}
