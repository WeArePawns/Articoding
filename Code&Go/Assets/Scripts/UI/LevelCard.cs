using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelCard : MonoBehaviour
{
    private LevelData level;
    private Category category;

    [SerializeField] private Text title;
    [SerializeField] private Image[] stars;
    [SerializeField] private Image cardImage;

    [HideInInspector] public Button button;

    int numLevel = 1;

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
        if (level == null) return;

        button = GetComponent<Button>();

        title.text = numLevel.ToString();

        uint levelStars = ProgressManager.Instance.GetLevelStars(category, numLevel - 1);

        //cambia el color de las estrellas que ha conseguido el jugador en el nivel
        for (int i = 0; i < 3; i++)
            if (i >= levelStars)
                stars[i].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void ConfigureLevel(LevelData level, Category category, int numLevel)
    {
        this.level = level;
        this.numLevel = numLevel;
        this.category = category;
        Configure();
    }

    public void DeactivateCard()
    {
        cardImage.color = Color.grey;
    }
}
