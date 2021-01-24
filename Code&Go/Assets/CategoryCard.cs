using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CategoryCard : MonoBehaviour
{
    [SerializeField] private Category category;

    [SerializeField] private Text title;
    [SerializeField] private Text description;

    [SerializeField] private ProgressBar progressBar; // TODO: hacer algo con esto

#if !UNITY_EDITOR
    private void Awake()
    {
        Configure();
    }
#else
    private void Update()
    {
        //Configure();
    }
#endif
    private void Configure()
    {
        if (category == null) return;
        title.text = category.name_id;
        description.text = category.description;
    }

    public void ConfigureCategory(Category category)
    {
        this.category = category;
        Configure();
    }
}
