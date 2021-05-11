using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : BoardObject, ILaserReceiver
{
    [SerializeField] private Renderer receiverRenderer;

    private bool registered = false;
    private bool registeredOnBoard = false;

    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    [SerializeField] private ParticleSystem onParticles;
    [SerializeField] private GameObject light;

    private void Awake()
    {
        typeName = "Receiver";
        argsNames = new string[0];
    }

    private void Start()
    {
        if (boardManager != null && !registeredOnBoard)
        {
            boardManager.RegisterReceiver();
            registeredOnBoard = true;
        }
    }

    public void OnLaserReceived()
    {
        receiverRenderer.material = onMaterial;
        onParticles.Play();
        light.SetActive(true);
        Invoke("ReceiverActive", 1.0f);
    }

    public void OnLaserReceiving()
    {
        // DO STUFF
    }

    public void OnLaserLost()
    {
        if (receiverRenderer != null) receiverRenderer.material = offMaterial;
        if (onParticles != null) onParticles.Stop();
        if (light != null) light.SetActive(false);

        if (registered)
        {
            if (boardManager != null) boardManager.ReceiverDeactivated();
            registered = false;
        }
        else
            CancelInvoke("ReceiverActive");
    }

    private void ReceiverActive()
    {
        registered = true;
        if (boardManager != null)
            boardManager.ReceiverActivated();
    }

    override public void SetBoard(BoardManager board)
    {
        this.boardManager = board;
        if (!registeredOnBoard)
        {
            registeredOnBoard = true;
            boardManager.RegisterReceiver();
            if (registered) boardManager.ReceiverActivated();
        }
    }
}
