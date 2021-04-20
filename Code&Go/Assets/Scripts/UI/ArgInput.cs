using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ArgInput : MonoBehaviour
{
    [SerializeField] Text argName;
    [SerializeField] InputField argField;

    public void FillArg(string name)
    {
        gameObject.SetActive(true);
        argName.text = name;
    }

    public string GetInput()
    {
        return argField.text;
    }
}
