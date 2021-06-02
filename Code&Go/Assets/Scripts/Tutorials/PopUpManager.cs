using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using AssetPackage;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [SerializeField] private RectTransform bodyRect;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private GameObject mainContent;
    [SerializeField] private PopUp popupPanel;
    [SerializeField] private Image highlightImage;
    [Space]
    [SerializeField] private Shader highlightShader;
    [SerializeField] [Min(0.0f)] private float highlightPadding;
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
        Destroy(gameObject);
    }

    public void Show(PopUpData data)
    {
        imageMaterial.SetVector("_PositionSize", Vector4.zero);
        mainContent.SetActive(true);
        popupPanel.Show(data);
        popupPanel.CenterPosition();

        TraceShow(data);
        popupPanel.AddListener(() => { TraceHide(data); Hide(); });
    }


    public void Show(PopUpData data, Rect rect)
    {
        TraceShow(data);
        float xPadding = highlightPadding * Screen.width / bodyRect.rect.width;
        float yPadding = highlightPadding * Screen.height / bodyRect.rect.height;
        Vector2 position = new Vector2(rect.x + rect.width / 2.0f, rect.y + rect.height / 2.0f);
        Vector2 offset = new Vector2(rect.width / 2.0f + xPadding, rect.height / 2.0f + yPadding);

        imageMaterial.SetVector("_PositionSize", new Vector4(rect.x, rect.y, rect.width, rect.height));

        mainContent.SetActive(true);
        popupPanel.Show(data);
        popupPanel.SetTargetPositionAndOffset(position, offset);

        if (data.next != null)
            popupPanel.AddListener(() => { TraceHide(data); Show(data.next, rect); });
        else
            popupPanel.AddListener(() => { TraceHide(data); Hide(); });

    }

    private void TraceShow(PopUpData data)
    {
        string content = data.localizedTitle.GetLocalizedString().Result + ": " + data.localizedContent.GetLocalizedString().Result;
        TrackerAsset.Instance.setVar("content", content.Replace("\"", "'"));
        TrackerAsset.Instance.Completable.Initialized("tip_" + data.name.ToLower(), CompletableTracker.Completable.DialogFragment);
    }

    private void TraceHide(PopUpData data)
    {
        string content = data.localizedTitle.GetLocalizedString().Result + ": " + data.localizedContent.GetLocalizedString().Result;
        TrackerAsset.Instance.setVar("content", content.Replace("\"", "'"));
        TrackerAsset.Instance.Completable.Completed("tip_" + data.name.ToLower(), CompletableTracker.Completable.DialogFragment);
    }

    public void Hide()
    {
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
