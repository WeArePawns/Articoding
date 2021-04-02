using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class PopUp : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Text titleText;
    [SerializeField] private Text contentText;
    [SerializeField] private Text buttonText;
    [SerializeField] private Button nextButton;
    [Space]
    [SerializeField] private LayoutElement capLayoutElement;
    [SerializeField] private int characterCap;
    [Space]
    [SerializeField] private bool lerpX = false;
    [SerializeField] private bool lerpY = true;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        //gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateCapLimit();
        }
    }

    public void Show(string title, string content, System.Action nextAction = null)
    {
        // Set texts
        if (string.IsNullOrEmpty(title))
        {
            titleText.gameObject.SetActive(false);
        }
        titleText.text = title;

        if (string.IsNullOrEmpty(content))
        {
            contentText.gameObject.SetActive(false);
        }
        contentText.text = content;

        // Set action
        if (nextAction == null)
        {
            nextAction = Hide;
            buttonText.text = "Cerrar";
        }
        buttonText.text = "Siguiente";
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(nextAction.Invoke);

        // Activate panel
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector2 position, Vector2 offset)
    {
        float width = Screen.width;
        float height = Screen.height;

        Vector2 pivot = new Vector2(position.x / width, position.y / height);
        if (!lerpX) pivot.x = Mathf.Round(pivot.x);
        if (!lerpY) pivot.y = Mathf.Round(pivot.y);
        panelRect.pivot = pivot;

        // Offset position
        position.x -= offset.x * (panelRect.pivot.x * 2.0f - 1.0f);
        position.y -= offset.y * (panelRect.pivot.y * 2.0f - 1.0f);

        // Check on screen
        float leftBound = panelRect.pivot.x * panelRect.rect.width;
        float rigthBound = Screen.width - (1 - panelRect.pivot.x) * panelRect.rect.width; 
        float downBound = panelRect.pivot.y * panelRect.rect.height;
        float upBound = Screen.height - (1 - panelRect.pivot.y) * panelRect.rect.height;

        if (position.x < leftBound) position.x = leftBound;
        else if (position.x > rigthBound) position.x = rigthBound;

        if (position.y < downBound) position.y = downBound;
        else if (position.y > upBound) position.y = upBound;

        panelRect.anchoredPosition = position;
    }

    public void SetPosition(float x, float y)
    {
        SetPosition(new Vector2(x, y), Vector2.zero);
    }

    private void UpdateCapLimit()
    {
        bool capPassed = contentText.text.Length >= characterCap;
        capLayoutElement.enabled = capPassed;
    }
}
