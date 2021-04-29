using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPackage;
using UBlockly.UGUI;


public class LevelTestManager : MonoBehaviour
{
    [SerializeField] Category levelsCreatedCategory;

    [SerializeField] GameObject levelObjects;
    [SerializeField] GameObject levelCanvas;
    [SerializeField] GameObject levelButtons;
    [SerializeField] CameraFit cameraFit;

    [SerializeField] GameObject creatorObjects;
    [SerializeField] GameObject creatorCanvas;

    [SerializeField] BoardManager board;
    [SerializeField] BoardCreator boardCreator;
    [SerializeField] TextAsset activeBlocks;

    private bool inCreator = false;

    private void Start()
    {
        Invoke("ChangeMode", 0.01f);
        ActivateLevelBlocks(activeBlocks, false);
    }

    public void ChangeMode()
    {
        inCreator = !inCreator;

        levelObjects.SetActive(!inCreator);
        levelCanvas.SetActive(!inCreator);
        levelButtons.SetActive(!inCreator);

        creatorObjects.SetActive(inCreator);
        creatorCanvas.SetActive(inCreator);

        board.SetModifiable(inCreator);

        board.transform.position = Vector3.zero;
        board.transform.localScale = Vector3.one;
        if (!inCreator) cameraFit.FitBoard(board.GetRows(), board.GetColumns());
        else boardCreator.FitBoard();
    }

    public void LoadLevelToTest()
    {

    }

    public void LoadMainMenu()
    {
        //TrackerAsset.Instance.setVar("steps", board.GetCurrentSteps());
        TrackerAsset.Instance.GameObject.Used("main_menu_return");

        GameManager.Instance.LoadScene("MenuScene");
    }

    public void ActivateLevelBlocks(TextAsset textAsset, bool allActive)
    {
        if (textAsset == null) return;

        StartCoroutine(AsyncActivateLevelBlocks(textAsset, allActive));
    }

    IEnumerator AsyncActivateLevelBlocks(TextAsset textAsset, bool allActive)
    {
        if (allActive) BlocklyUI.WorkspaceView.Toolbox.SetActiveAllBlocks();
        else if (textAsset != null)
        {
            ActiveBlocks blocks = ActiveBlocks.FromJson(textAsset.text);
            BlocklyUI.WorkspaceView.Toolbox.SetActiveBlocks(blocks.AsMap());
        }

        yield return null;
    }
}
