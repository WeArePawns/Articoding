using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

// Class that controls inner functionality
public class PopUp : MonoBehaviour
{
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private GameObject titleContent;
    [SerializeField] private GameObject contentContent;
    [SerializeField] private LocalizeStringEvent titleText;
    [SerializeField] private LocalizeStringEvent contentText;
    [SerializeField] private Text buttonText;
    [SerializeField] private Button nextButton;
    [Space]
    [SerializeField] private LayoutElement capLayoutElement;
    [SerializeField] [Min(0)] private int titleCharacterCap;
    [SerializeField] [Min(0)] private int contentCharacterCap;
    [Space]
    [SerializeField] private bool lerpX = false;
    [SerializeField] private bool lerpY = true;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        gameObject.SetActive(false);
        panelRect.anchoredPosition = new Vector2(contentRect.rect.width / 2.0f, contentRect.rect.height / 2.0f);
    }

    // Warning: this method empties button listeners
    public void Show(PopUpData data)
    {
        data.localizedTitle.RefreshString();
        data.localizedContent.RefreshString();

        // Set texts
        titleContent.SetActive(!string.IsNullOrEmpty(data.localizedTitle.GetLocalizedString().ToString()));
        titleText.gameObject.SetActive(!string.IsNullOrEmpty(data.localizedTitle.GetLocalizedString().ToString()));
     
        titleText.StringReference = data.localizedTitle;
        titleText.RefreshString();

        contentContent.SetActive(!string.IsNullOrEmpty(data.localizedContent.GetLocalizedString().ToString()));
        contentText.gameObject.SetActive(!string.IsNullOrEmpty(data.localizedContent.GetLocalizedString().ToString()));
        contentText.StringReference = data.localizedContent;
        contentText.RefreshString();

        // Set action
        nextButton.onClick.RemoveAllListeners();
        if (data.next == null)
        {
            buttonText.text = "Cerrar";
            //nextButton.onClick.AddListener(Hide); // Controlled from manager
        }
        else
        {
            buttonText.text = "Siguiente";
            //nextButton.onClick.AddListener(() => Show(data.next)); // Controlled from manager
        }

        // Update size dynamically
        UpdateCapLimit();

        // Activate panel
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetTargetPositionAndOffset(Vector2 position, Vector2 offset)
    {
        if (position == null) return;

        float width = contentRect.rect.width;
        float height = contentRect.rect.height;

        // Scale position
        position.x *= width / Screen.width;
        position.y *= height / Screen.height;
/*
        // Offset position
        float mAspectRatio = (float)Screen.width / Screen.height;
        float xDiff = (canvasScaler.referenceResolution.x - canvasScaler.referenceResolution.y * mAspectRatio) * canvasScaler.matchWidthOrHeight;
        float yDiff = (canvasScaler.referenceResolution.y - canvasScaler.referenceResolution.x / mAspectRatio) * (1.0f - canvasScaler.matchWidthOrHeight);
        position.x -= xDiff;
        position.y -= yDiff;*/

        // Scale offset
        offset.x *= width / Screen.width;
        offset.y *= height / Screen.height;

        Vector2 pivot = new Vector2(position.x / width, position.y / height);
        if (!lerpX) pivot.x = Mathf.Round(pivot.x);
        if (!lerpY) pivot.y = Mathf.Round(pivot.y);
        panelRect.pivot = pivot;

        // Offset position
        if (offset != null)
        {
            position.x -= offset.x * (panelRect.pivot.x * 2.0f - 1.0f);
            position.y -= offset.y * (panelRect.pivot.y * 2.0f - 1.0f);
        }

        // Check on screen
        float leftBound = panelRect.pivot.x * panelRect.rect.width;
        float rigthBound = width - (1 - panelRect.pivot.x) * panelRect.rect.width; 
        float downBound = panelRect.pivot.y * panelRect.rect.height;
        float upBound = height - (1 - panelRect.pivot.y) * panelRect.rect.height;

        if (position.x < leftBound) position.x = leftBound;
        else if (position.x > rigthBound) position.x = rigthBound;

        if (position.y < downBound) position.y = downBound;
        else if (position.y > upBound) position.y = upBound;

        panelRect.anchoredPosition = position;
    }

    public void SetTargetPosition(float x, float y)
    {
        SetTargetPositionAndOffset(new Vector2(x, y), Vector2.zero);
    }

    public void CenterPosition()
    {
        Vector2 position = new Vector2(contentRect.rect.width / 2.0f, contentRect.rect.height / 2.0f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        /*
        // Offset position
        float mAspectRatio = (float)Screen.width / Screen.height;
        float xDiff = (canvasScaler.referenceResolution.x - canvasScaler.referenceResolution.y * mAspectRatio) * canvasScaler.matchWidthOrHeight;
        float yDiff = (canvasScaler.referenceResolution.y - canvasScaler.referenceResolution.x / mAspectRatio) * (1.0f - canvasScaler.matchWidthOrHeight);
        position.x -= xDiff / 2.0f;
        position.y -= yDiff / 2.0f;*/

        panelRect.anchoredPosition = position;
    }

    public void AddListener(UnityAction action)
    {
        nextButton.onClick.AddListener(action);
    }

    private void UpdateCapLimit()
    {
        bool capPassed = contentText.StringReference.ToString().Length >= contentCharacterCap || titleText.StringReference.ToString().Length >= titleCharacterCap;
        capLayoutElement.enabled = capPassed;
    }
}
