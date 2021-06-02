using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssetPackage;

public class ArgumentLoader : MonoBehaviour
{
    [SerializeField] private ArgInput[] inputs;
    [SerializeField] private Text text;

    private BoardObject currentObject;

    public void SetBoardObject(BoardObject newObject)
    {
        if (newObject == null) return;

        currentObject = newObject;
        gameObject.SetActive(true);
        text.text = currentObject.GetNameWithIndex();

        string[] argsNames = currentObject.GetArgsNames();
        if (argsNames.Length == 0) gameObject.SetActive(false);
        for (int i = 0; i < inputs.Length; i++)
        {
            if (i < argsNames.Length)
            {
                int index = i;
                inputs[i].Init((bool action) => TraceCheckBox(action, argsNames[index]));
                inputs[i].FillArg(argsNames[i]);
            }
            else
                inputs[i].gameObject.SetActive(false);
        }
    }

    private void TraceCheckBox(bool active, string argName)
    {
        TrackerAsset.Instance.setVar("argument_name", argName.ToLower());
        TrackerAsset.Instance.setVar("element_type", currentObject.GetName().ToLower());
        TrackerAsset.Instance.setVar("element_name", currentObject.GetNameWithIndex().ToLower());
        TrackerAsset.Instance.setVar("element_id", currentObject.GetID().ToLower());
        TrackerAsset.Instance.setVar("old_value", !active);
        TrackerAsset.Instance.setVar("new_value", active);
        TrackerAsset.Instance.setVar("action", "change_value");
        TrackerAsset.Instance.GameObject.Interacted("argument_checkbox");
    }

    public void LoadArgs()
    {
        if (currentObject == null) return;

        TrackerAsset.Instance.setVar("element_type", currentObject.GetName().ToLower());
        TrackerAsset.Instance.setVar("element_name", currentObject.GetNameWithIndex().ToLower());

        string[] args = new string[inputs.Length];
        for (int i = 0; i < args.Length; i++)
        {
            if (inputs[i].gameObject.activeSelf)
            {
                args[i] = inputs[i].GetInput();
                TrackerAsset.Instance.setVar("arg_name", currentObject.GetArgsNames()[i].ToLower());
                TrackerAsset.Instance.setVar("arg_value", args[i].ToLower());
            }
        }

        currentObject.LoadArgs(args);

        TrackerAsset.Instance.setVar("action", "state_change");
        TrackerAsset.Instance.GameObject.Interacted(currentObject.GetID().ToLower());
    }

    private void Update()
    {
        if (currentObject == null && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
