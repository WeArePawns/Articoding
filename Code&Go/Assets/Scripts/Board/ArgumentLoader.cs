using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArgumentLoader : MonoBehaviour
{
    [SerializeField] private InputField[] argsText;
    [SerializeField] private Text text;

    private BoardObject currentObject;

    public void SetBoardObject(BoardObject newObject)
    {
        if (newObject == null) return;

        currentObject = newObject;
        gameObject.SetActive(true);
        text.text = currentObject.GetName();
    }

    public void LoadArgs()
    {
        if (currentObject == null) return;

        string[] args = new string[argsText.Length];
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = argsText[i].text;
        }

        currentObject.LoadArgs(args);
    }

    private void Update()
    {
        if (currentObject == null && gameObject.activeSelf)
            gameObject.SetActive(false);        
    }
}
