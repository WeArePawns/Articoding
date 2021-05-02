using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Text titlePrefab;
    [SerializeField] private Text paragraphPrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Button backButton;
    [Space]
    [SerializeField] private PopUpData[] allTutorials;
    private List<string> shownTutorials;

    private Button[] categoryButtons;

    private void Awake()
    {
        shownTutorials = new List<string>();

        Instance = this;

        CreateCategoryList();
        backButton.onClick.AddListener(() => ShowTutorialsCategoryList());
    }
    private void Start()
    {
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
            TutorialType type = (TutorialType)(i + 1);
            categoryButtons[i].interactable = shownTemary.ContainsKey(type);
        }
    }

    private void CreateCategoryList()
    {
        int count = Enum.GetValues(typeof(TutorialType)).Length;
        categoryButtons = new Button[count - 1];
        for (int i = 0; i < count; i++)
        {
            TutorialType type = (TutorialType)i;
            if (type == TutorialType.NONE) continue;

            Button button = Instantiate(categoryButton, categoryList);
            button.onClick.AddListener(() => ShowCategory(type));

            button.transform.GetChild(0).GetComponent<Text>().text = TypeToString(type);
            categoryButtons[i - 1] = button;
        }
        categoryButton.gameObject.SetActive(false);
    }

    private void ShowCategory(TutorialType type)
    {
        if (!shownTemary.ContainsKey(type)) return;

        // Deactivate tutorial category list
        categoryList.gameObject.SetActive(false);

        // Delete previous content
        foreach (Transform child in contentRect)
            Destroy(child.gameObject);

        // Show all tutorials
        List<PopUpData> tutorials = shownTemary[type];
        PopUpData lastData = null;
        foreach(PopUpData data in tutorials)
        {
            if(lastData == null || data.title != lastData.title)
                AddTitle(data.title);
            AddParagraph(data.content);
            lastData = data;
        }

        contentRect.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
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
    }

    public void AddTemary(PopUpData data)
    {
        string hash = Hash.ToHash(data.title + data.content, "TutorialTrigger");
        if (shownTutorials.Contains(hash)) return;

        shownTutorials.Add(hash);
        Configure();
    }

    private void AddTitle(string s)
    {
        Text title = Instantiate(titlePrefab, contentRect);
        title.gameObject.SetActive(true);
        title.text = s;
    }

    private void AddParagraph(string s)
    {
        Text paragraph = Instantiate(paragraphPrefab, contentRect);
        paragraph.gameObject.SetActive(true);
        paragraph.text = s;
    }

    public void Load(TutorialSaveData data)
    {
        shownTutorials.AddRange(data.tutorials);
        Configure();
    }

    private string TypeToString(TutorialType type)
    {
        string[] arr = { "Ninguna", "General", "Tablero", "Variables", "Tipos", "Operadores", "Bucles", "Condiciones", "Acciones" };

        return arr[(int)type];
    }
}
