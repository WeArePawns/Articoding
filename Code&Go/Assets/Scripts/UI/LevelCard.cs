using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelCard : MonoBehaviour
{
    private LevelData level;

    [SerializeField] private Text title;
    [SerializeField] private GameObject[] stars;

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

        // TODO: cambiar el color de las estrellas que ha conseguido el jugador en el nivel

    }

    public void ConfigureLevel(LevelData level, int numLevel)
    {
        this.level = level;
        this.numLevel = numLevel;
        Configure();
    }
}
