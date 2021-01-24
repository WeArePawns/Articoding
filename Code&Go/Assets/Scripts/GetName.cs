using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class GetName : MonoBehaviour
{
    [SerializeField] private Text text;

#if !UNITY_EDITOR
    private void Awake()
    {
        if (text == null)
            Debug.LogError("Text no asignado");

        text.text = gameObject.name;
    }
#else
    private void Update()
    {
        if (text == null)
            Debug.LogError("Text no asignado");

        text.text = gameObject.name;
    }
#endif
}
