using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    [SerializeField] private GameObject mainContent;
    [SerializeField] private PopUp popupPanel;
    [SerializeField] private Image highlightImage;
    [Space]
    [SerializeField] private Shader highlightShader;
    [SerializeField] [Min(0.0f)] private float highlightPadding; 
    private Material imageMaterial;

    //TODO: quitar
    public RectTransform exampleUIRect;

    private void Awake()
    {
        Instance = this;
        imageMaterial = new Material(highlightShader);
        highlightImage.material = imageMaterial;
        mainContent.SetActive(false);
    }

    private void Update()
    {
        // TODO: quitar, esto es para probar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Show("Hola, titulo", "Esto es una prueba, solo queria saludar a un amigo", RectUtils.RectTransformToScreenSpace(exampleUIRect));
        }
    }

    public void Show(string title, string content)
    {
        PopUpData data = new PopUpData();
        data.title = title;
        data.content = content;
        data.position = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

        imageMaterial.SetVector("_PositionSize", Vector4.zero);
        mainContent.SetActive(true);
        popupPanel.Show(data);
    }

    public void Show(string title, string content, Rect rect)
    {
        PopUpData data = new PopUpData();
        data.title = title;
        data.content = content;
        data.position = new Vector2(rect.x + rect.width / 2.0f, rect.y + rect.height / 2.0f);
        data.offset = new Vector2(rect.width / 2.0f + highlightPadding, rect.height / 2.0f + highlightPadding);

        imageMaterial.SetVector("_PositionSize", new Vector4(rect.x, rect.y, rect.width, rect.height));

        mainContent.SetActive(true);
        popupPanel.Show(data);
    }

    public void Show(PopUpData data, Rect rect)
    {
        data.position = new Vector2(rect.x + rect.width / 2.0f, rect.y + rect.height / 2.0f);
        data.offset = new Vector2(rect.width / 2.0f + highlightPadding, rect.height / 2.0f + highlightPadding);

        imageMaterial.SetVector("_PositionSize", new Vector4(rect.x, rect.y, rect.width, rect.height));

        mainContent.SetActive(true);
        popupPanel.Show(data);
    }

    public void Hide()
    {
        mainContent.SetActive(false);
    }
}
