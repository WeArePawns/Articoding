using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class ArgInput : MonoBehaviour
{
    [SerializeField] Text argName;
    [SerializeField] Toggle argField;

    public void Init(UnityAction<bool> valueChangedEvent)
    {
        argField.onValueChanged.RemoveAllListeners();
        argField.onValueChanged.AddListener(valueChangedEvent);
    }

    public void FillArg(string name)
    {
        gameObject.SetActive(true);
        argName.text = name;
    }

    public string GetInput()
    {
        return argField.isOn ? "1" : "0";
    }
}
