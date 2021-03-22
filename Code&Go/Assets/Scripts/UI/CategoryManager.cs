using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    [SerializeField] private Category[] categories;

    [SerializeField] private GameObject categoriesParent;
    [SerializeField] private CategoryCard categoryCardPrefab;

    public Text categoryName;
    public Text categoryDescription;

    public GameObject categoriesPanel;
    public GameObject levelsPanel;

    public GameObject currentCategoryPanel;
    public GameObject currentLevelPanel;

    public Text currentCategoryLevelsText;

    public int currentCategory;

    private void Start()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;

            CategoryCard card = Instantiate(categoryCardPrefab, categoriesParent.transform);
            card.ConfigureCategory(categories[i]);
            card.button.onClick.AddListener(() =>
            {
                SelectCategory(index);
            });
        }

        HideLevels();
    }

    private void SelectCategory(int index)
    {
        if (index >= 0 && index < categories.Length)
        {
            currentCategory = index;

            categoryName.text = categories[currentCategory].name_id;
            categoryDescription.text = categories[currentCategory].description;
        }
    }

    public void ShowLevels()
    {
        currentCategoryLevelsText.text = "Levels - " + categories[currentCategory].name_id;

        categoriesPanel.SetActive(false);
        levelsPanel.SetActive(true);

        currentCategoryPanel.SetActive(false);
        currentLevelPanel.SetActive(true);
    }

    public void HideLevels()
    {
        currentCategoryLevelsText.text = "Categories";

        categoriesPanel.SetActive(true);
        levelsPanel.SetActive(false);

        currentCategoryPanel.SetActive(true);
        currentLevelPanel.SetActive(false);

    }
}
