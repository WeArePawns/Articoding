using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    [SerializeField] private Category[] categories;

    [SerializeField] private GameObject categoriesParent;
    [SerializeField] private CategoryCard categoryCardPrefab;

    [SerializeField] private GameObject levelsParent;
    [SerializeField] private LevelCard levelCardPrefab;

    [SerializeField] private GameObject levelsCreatedParent;
    [SerializeField] private GameObject createLevelButton;
    [SerializeField] private Category levelsCreatedCategory;

    public Text categoryName;
    public Text categoryDescription;

    public Text levelName;
    public Text levelDescription;

    public Text levelCreatedName;
    public Text levelCreatdeDescription;

    public GameObject categoriesPanel;
    public GameObject levelsPanel;

    public GameObject currentCategoryPanel;
    public GameObject currentLevelPanel;

    public Text currentCategoryLevelsText;

    public int currentCategory;
    private int currentLevel;
    private int levelCreatedIndex;

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
            //If it's not unlocked it can't be selected
            if (!ProgressManager.Instance.IsCategoryUnlocked(index)) card.button.enabled = false;
            //TODO: Poner icono de candado o algo
        }

        HideLevels();

        SelectCategory(currentCategory);

        CreateUserLevelsCards();
    }

    private void CreateUserLevelsCards()
    {
        for (int i = 0; i < levelsCreatedCategory.levels.Count; i++)
        {
            int index = i;
            LevelData levelData = levelsCreatedCategory.levels[i];
            LevelCard levelCard = Instantiate(levelCardPrefab, levelsCreatedParent.transform);
            levelCard.ConfigureLevel(levelData, levelsCreatedCategory, i + 1);
            levelCard.DeactivateStars();
            levelCard.button.onClick.AddListener(() =>
            {
                levelCreatedIndex = index;
                levelCreatedName.text = levelData.levelName;
                levelCreatdeDescription.text = levelData.description;
            });
        }
        createLevelButton.SetActive(true);
        createLevelButton.transform.SetParent(levelsCreatedParent.transform);
    }

    private void SelectCategory(int index)
    {
        if (!ProgressManager.Instance.IsCategoryUnlocked(index)) return;

        if (index >= 0 && index < categories.Length)
        {
            currentCategory = index;

            categoryName.text = categories[currentCategory].name_id;
            categoryDescription.text = categories[currentCategory].description;
        }
    }

    public void ShowLevels()
    {
        if (!ProgressManager.Instance.IsCategoryUnlocked(currentCategory)) return;

        Category category = categories[currentCategory];
        currentCategoryLevelsText.text = "Levels - " + category.name_id;

        categoriesPanel.SetActive(false);
        levelsPanel.SetActive(true);

        currentCategoryPanel.SetActive(false);
        currentLevelPanel.SetActive(true);

        for (int i = 0; i < category.levels.Count; i++)
        {
            int index = i;
            LevelData levelData = category.levels[i];
            LevelCard levelCard = Instantiate(levelCardPrefab, levelsParent.transform);
            levelCard.ConfigureLevel(levelData, category, i + 1);
            if (ProgressManager.Instance.IsLevelUnlocked(currentCategory, i))
            {
                levelCard.button.onClick.AddListener(() =>
                {
                    currentLevel = index;
                    levelName.text = levelData.levelName;
                    levelDescription.text = levelData.description;
                });
            }
            else
                levelCard.DeactivateCard();
        }

        // TODO: seleccionar primer level sin completar
    }

    public void HideLevels()
    {
        currentCategoryLevelsText.text = "Categories";

        categoriesPanel.SetActive(true);
        levelsPanel.SetActive(false);

        currentCategoryPanel.SetActive(true);
        currentLevelPanel.SetActive(false);

        while (levelsParent.transform.childCount != 0)
            DestroyImmediate(levelsParent.transform.GetChild(0).gameObject);
    }

    public void PlaySelectedLevel()
    {
        GameManager.Instance.LoadLevel(categories[currentCategory], currentLevel);
    }

    public void PlayLevelCreated()
    {
        GameManager.Instance.LoadLevel(levelsCreatedCategory, levelCreatedIndex);
    }
}
