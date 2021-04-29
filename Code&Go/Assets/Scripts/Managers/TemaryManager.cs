using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemaryManager : MonoBehaviour
{
    private static Dictionary<TutorialType, List<PopUpData>> shownTemary = new Dictionary<TutorialType, List<PopUpData>>();

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Category currentCategory;

    private static TutorialType lastTutorialType;
    private static PopUpData lastData;

    [Space]
    [SerializeField] private RectTransform categoryList;
    [SerializeField] private Button categoryButton;
    [Space]
    [SerializeField] private Text titlePrefab;
    [SerializeField] private Text paragraphPrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Button backButton;

    private Button[] categoryButtons;

    private void Awake()
    {
        /*if (GameManager.Instance != null)
            currentCategory = GameManager.Instance.GetCurrentCategory();*/
        CreateCategoryList();

        backButton.onClick.AddListener(() => ShowTutorialsCategoryList());
    }

    private void CreateCategoryList()
    {
        int count = Enum.GetValues(typeof(TutorialType)).Length;
        categoryButtons = new Button[count];
        for (int i = 0; i < count; i++)
        {
            TutorialType type = (TutorialType)i;
            Button button = Instantiate(categoryButton, categoryList);
            button.onClick.AddListener(() => ShowCategory(type));

            button.transform.GetChild(0).GetComponent<Text>().text = type.ToString();
            categoryButtons[i] = button;
        }
        categoryButton.gameObject.SetActive(false);

        // TODO: disable button if theres no tutorials
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

    public static void AddTemary(TutorialType id, PopUpData data)
    {
        if (!shownTemary.ContainsKey(id))
            shownTemary.Add(id, new List<PopUpData>());

        List<PopUpData> list = shownTemary[id];
        if (!list.Contains(data))
            list.Add(data);
        else return;

        lastTutorialType = id;
        lastData = data;
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
}
