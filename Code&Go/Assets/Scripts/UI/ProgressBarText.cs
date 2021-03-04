using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
[RequireComponent(typeof(Text))]
public class ProgressBarText : MonoBehaviour
{
    public ProgressBar progressBar;
    public Text text;

    private void Update()
    {
        if (progressBar == null)
        {
            print("Progress bar not asigned");
            return;
        }

        if(text == null)
        {
            print("Text not asigned");
            return;
        }

        int current = Mathf.RoundToInt(progressBar.GetCurrent());
        int maximum = Mathf.RoundToInt(progressBar.GetMaximum());
        text.text = current.ToString() + "/" + maximum.ToString();
    }
}
