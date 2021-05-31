using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CategoryCard : MonoBehaviour
{
    [SerializeField] private Category category;

    [SerializeField] private LocalizeStringEvent localizeString;
    [SerializeField] private Text title;
    [SerializeField] private Text stars;

    [SerializeField] private ProgressBar progressBar;

    [HideInInspector] public Button button;
    public Image image;

    private void Configure()
    {
        if (category == null) return;

        button = GetComponent<Button>();

        localizeString.StringReference = category.nameIDLocalized;
        localizeString.RefreshString();
        var op = localizeString.StringReference.GetLocalizedString();
        title.text = op.Result;

        stars.text = ProgressManager.Instance.GetCategoryTotalStars(category).ToString() + "/" + (category.levels.Count * 3).ToString(); ; //category.description;
        progressBar.minimum = 0.0f;
        progressBar.maximum = category.levels.Count;
        progressBar.current = ProgressManager.Instance.GetCategoryCurrentProgress(category);
        progressBar.Configure();
        image.sprite = category.icon;
    }

    public void ConfigureCategory(Category category)
    {
        this.category = category;
        Configure();
    }
}
