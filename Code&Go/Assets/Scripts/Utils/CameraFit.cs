using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFit : MonoBehaviour
{
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private BoardManager board;
    [SerializeField] private Vector2 marginOffset;
    [SerializeField] private RectTransform viewportRect;

    private float lastWidth;
    private float lastHeight;

    private void Awake()
    {
        lastWidth = contentRect.rect.width;
        lastHeight = contentRect.rect.height;
    }

    private void Start()
    {
        FitViewPort();
    }

    private void LateUpdate()
    {
        if (contentRect.rect.width != lastWidth || contentRect.rect.height != lastHeight)
        {
            FitViewPort();
            //FitBoard(board.GetRows(), board.GetColumns());
            lastWidth = contentRect.rect.width;
            lastHeight = contentRect.rect.height;
        }
    }

    private void FitViewPort()
    {
        if (viewportRect == null) return;

        Camera mainCamera = Camera.main;

        Canvas.ForceUpdateCanvases();

        float wRatio = viewportRect.rect.width / contentRect.rect.width;
        float hRatio = viewportRect.rect.height / contentRect.rect.height;

        mainCamera.rect = new Rect(1.0f - wRatio, 0.0f, wRatio, hRatio);
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

    public void SetViewPort(RectTransform viewport)
    {
        this.viewportRect = viewport;
        FitViewPort();
    }
}
