using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFit : MonoBehaviour
{
    [SerializeField] private BoardManager board;
    [SerializeField] private Vector2 marginOffset;
    [SerializeField] private RectTransform viewportRect;


    private void Start()
    {
        FitViewPort();
    }

    private void FitViewPort()
    {
        if (viewportRect == null) return;

        Camera mainCamera = Camera.main;

        Canvas.ForceUpdateCanvases();

        mainCamera.rect = new Rect(viewportRect.rect.width / Screen.currentResolution.width, 0.0f,
                                    viewportRect.rect.width / Screen.currentResolution.width, viewportRect.rect.height / Screen.currentResolution.height);
    }

    public void FitBoard(int rows, int columns)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float rowsFixed = rows + marginOffset.x;
            float columnsFixed = columns + marginOffset.y;

            float aspectRatio = (float)mainCamera.aspect; // ratio de pantalla
            float targetRatio = columnsFixed / rowsFixed; // ratio del board

            if (aspectRatio >= targetRatio)
                mainCamera.orthographicSize = (float)rowsFixed / 2.0f;
            else
            {
                float differenceInSize = targetRatio / aspectRatio;
                mainCamera.orthographicSize = (float)(rowsFixed / 2.0f) * differenceInSize;
            }
        }
    }
}
