using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using AssetPackage;


public class TemaryManager : MonoBehaviour
{
    public static TemaryManager Instance = null;
    private Dictionary<TutorialType, List<PopUpData>> shownTemary;

    [SerializeField] private RectTransform rectTransform;

    private PopUpData lastData;

    [Space]
    [SerializeField] private RectTransform categoryList;
    [SerializeField] private Button categoryButton;
    [Space]
    [SerializeField] private LocalizeStringEvent titlePrefab;
    [SerializeField] private LocalizeStringEvent paragraphPrefab;
    [SerializeField] private LocalizeSpriteEvent imagePrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Button backButton;
    [SerializeField] private LocalizeStringEvent categoryTitle;
    [SerializeField] private GameObject bodyContent;
    [Space]
    [SerializeField] private LocalizeStringEvent localizedCategoryTitle;
    [SerializeField] private LocalizedString[] stringReferences;
    [Space]
    [SerializeField] private PopUpData[] allTutorials;
    private List<string> shownTutorials;

    private Button[] categoryButtons;

    private TutorialType[] categoriesOrder = new TutorialType[] { TutorialType.NONE,
    TutorialType.GENERAL,
    TutorialType.BOARD,
    TutorialType.CATEGORY_VARIABLES,
    TutorialType.ACTION,
    TutorialType.CATEGORY_TYPES,
    TutorialType.CATEGORY_OPERATORS,
    TutorialType.CATEGORY_LOOPS,
    TutorialType.CATEGORY_CONDITIONS
     };

    private void Awake()
    {
        shownTutorials = new List<string>();

        Instance = this;

        CreateCategoryList();

        if (backButton != null)
            backButton.onClick.AddListener(() => ShowTutorialsCategoryList());
    }
    private void Start()
    {
        if (!GameManager.Instance.IsGameLoaded())
            GameManager.Instance.LoadGame();
        shownTutorials.AddRange(TutorialManager.Instance.GetTriggeredTutorials());
        Configure();
    }

    // Slow, avoid calling repetly
    private void Configure()
    {
        shownTemary = new Dictionary<TutorialType, List<PopUpData>>();

        for (int i = 0; i < allTutorials.Length; i++)
        {
            if (!shownTutorials.Contains(Hash.ToHash(allTutorials[i].title + allTutorials[i].content, "TutorialTrigger"))) continue;

            if (!shownTemary.ContainsKey(allTutorials[i].type))
                shownTemary.Add(allTutorials[i].type, new List<PopUpData>());

            List<PopUpData> list = shownTemary[allTutorials[i].type];
            list.Add(allTutorials[i]);
        }

        for (int i = 0; i < categoryButtons.Length; i++)
        {
            TutorialType type = categoriesOrder[i + 1];
            bool enabled = shownTemary.ContainsKey(type);
            categoryButtons[i].interactable = enabled;

            if (enabled && backButton == null && contentRect.childCount <= 1)
                categoryButtons[i].onClick.Invoke();
        }
    }

    private void CreateCategoryList()
    {
        int count = Enum.GetValues(typeof(TutorialType)).Length;
        categoryButtons = new Button[count - 1];
        for (int i = 0; i < count; i++)
        {
            TutorialType type = categoriesOrder[i];
            if (type == TutorialType.NONE) continue;

            Button button = Instantiate(categoryButton, categoryList);
            button.onClick.AddListener(() => ShowCategory(type));

            //button.transform.GetChild(0).GetComponent<Text>().text = TypeToString(type);
            LocalizeStringEvent localized = button.GetComponent<LocalizeStringEvent>();
            localized.StringReference = TypeToString(type);
            localized.RefreshString();

            categoryButtons[i - 1] = button;
        }
        categoryButton.gameObject.SetActive(false);
    }

    private void ShowCategory(TutorialType type)
    {
        if (!shownTemary.ContainsKey(type)) return;

        // Deactivate tutorial category list
        if (backButton != null)
            categoryList.gameObject.SetActive(false);

        // Delete previous content
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        // Show all tutorials
        List<PopUpData> tutorials = shownTemary[type];
        PopUpData lastData = null;
        foreach (PopUpData data in tutorials)
        {
            if (lastData == null || data.localizedTitle.ToString() != lastData.localizedTitle.ToString())
                AddTitle(data.localizedTitle);
            if (!data.localizedImage.IsEmpty)
                AddImage(data.localizedImage);
            AddParagraph(data.localizedContent);
            lastData = data;
        }

        contentRect.gameObject.SetActive(true);

        if (backButton != null)
            backButton.gameObject.SetActive(true);

        if (bodyContent != null)
            bodyContent.SetActive(true);

        //categoryTitle.text = TypeToString(type);
        localizedCategoryTitle.StringReference = TypeToString(type);
        localizedCategoryTitle.RefreshString();

        TrackerAsset.Instance.setVar("tutorial_category", type.ToString());
        TrackerAsset.Instance.GameObject.Used("temary_category_selected");
    }

    private void ShowTutorialsCategoryList()
    {
        // Delete previous content
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        // Deactivate content
        contentRect.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        // Show tutorials category list
        categoryList.gameObject.SetActive(true);

        if (bodyContent != null)
            bodyContent.SetActive(false);
    }

    public void AddTemary(PopUpData data)
    {
        string hash = Hash.ToHash(data.title + data.content, "TutorialTrigger");
        if (shownTutorials.Contains(hash)) return;

        shownTutorials.Add(hash);
        Configure();
    }

    private void AddTitle(LocalizedString s)
    {
        LocalizeStringEvent title = Instantiate(titlePrefab, contentRect);
        title.gameObject.SetActive(true);
        title.StringReference = s;
        title.RefreshString();
    }

    private void AddImage(LocalizedSprite s)
    {
        LocalizeSpriteEvent image = Instantiate(imagePrefab, contentRect);
        image.gameObject.SetActive(true);
        image.AssetReference.SetReference(s.TableReference, s.TableEntryReference);

    }

    private void AddParagraph(LocalizedString s)
    {
        LocalizeStringEvent paragraph = Instantiate(paragraphPrefab, contentRect);
        paragraph.gameObject.SetActive(true);
        paragraph.StringReference = s;
        paragraph.RefreshString();
    }

    public void Load(TutorialSaveData data)
    {
        shownTutorials.AddRange(data.tutorials);
        Configure();
    }

    private LocalizedString TypeToString(TutorialType type)
    {
        return stringReferences[(int)type];
    }
}
