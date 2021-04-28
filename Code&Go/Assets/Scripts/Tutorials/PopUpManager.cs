using System;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private GameObject mainContent;
    [SerializeField] private PopUp popupPanel;
    [SerializeField] private Image highlightImage;
    [Space]
    [SerializeField] private Shader highlightShader;
    [SerializeField] [Min(0.0f)] private float highlightPadding;
    [SerializeField] private PanelControl inventoryArea;
    private Material imageMaterial;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            imageMaterial = new Material(highlightShader);
            highlightImage.material = imageMaterial;
            mainContent.SetActive(false);
            return;
        }
        Instance.inventoryArea = inventoryArea;
        Destroy(gameObject);
    }

    public void Show(string title, string content)
    {
        PopUpData data = ScriptableObject.CreateInstance<PopUpData>();
        data.title = title;
        data.content = content;
        if (inventoryArea != null) inventoryArea.DisableDissapear(true);

        imageMaterial.SetVector("_PositionSize", Vector4.zero);
        mainContent.SetActive(true);
        popupPanel.Show(data);
        popupPanel.CenterPosition();

        popupPanel.AddListener(Hide);
    }

    public void Show(string title, string content, Rect rect)
    {
        PopUpData data = new PopUpData();
        data.title = title;
        data.content = content;
        float xPadding = highlightPadding * Screen.width / canvasScaler.referenceResolution.x;
        float yPadding = highlightPadding * Screen.height / canvasScaler.referenceResolution.y;
        Vector2 position = new Vector2(rect.x + rect.width / 2.0f, rect.y + rect.height / 2.0f);
        Vector2 offset = new Vector2(rect.width / 2.0f + xPadding, rect.height / 2.0f + yPadding);
        if (inventoryArea != null) inventoryArea.DisableDissapear(true);

        imageMaterial.SetVector("_PositionSize", new Vector4(rect.x, rect.y, rect.width, rect.height));

        mainContent.SetActive(true);
        popupPanel.Show(data);
        popupPanel.SetTargetPositionAndOffset(position, offset);

        if (data.next != null)
            popupPanel.AddListener(() => Show(data.next, rect));
        else
            popupPanel.AddListener(Hide);
    }

    public void Show(PopUpData data, Rect rect)
    {
        float xPadding = highlightPadding * Screen.width / canvasScaler.referenceResolution.x;
        float yPadding = highlightPadding * Screen.height / canvasScaler.referenceResolution.y;
        Vector2 position = new Vector2(rect.x + rect.width / 2.0f, rect.y + rect.height / 2.0f);
        Vector2 offset = new Vector2(rect.width / 2.0f + xPadding, rect.height / 2.0f + yPadding);
        if (inventoryArea != null) inventoryArea.DisableDissapear(true);

        imageMaterial.SetVector("_PositionSize", new Vector4(rect.x, rect.y, rect.width, rect.height));

        mainContent.SetActive(true);
        popupPanel.Show(data);
        popupPanel.SetTargetPositionAndOffset(position, offset);

        if (data.next != null)
            popupPanel.AddListener(() => Show(data.next, rect));
        else
            popupPanel.AddListener(Hide);

    }

    public void Hide()
    {
        if(inventoryArea != null) inventoryArea.DisableDissapear(false);
        mainContent.SetActive(false);
        popupPanel.Hide();
    }


    public bool IsShowing()
    {
        return popupPanel.gameObject.activeSelf && mainContent.activeSelf;
    }

    public GraphicRaycaster GetGraphicRaycaster()
    {
        return graphicRaycaster;
    }

}
