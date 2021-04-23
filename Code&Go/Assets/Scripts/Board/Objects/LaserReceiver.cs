using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : BoardObject, ILaserReceiver
{
    [SerializeField] private Renderer receiverRenderer;

    private bool registered = false;

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
        if (boardManager != null)
            boardManager.RegisterReceiver();
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
        receiverRenderer.material = offMaterial;
        onParticles.Stop();
        light.SetActive(false);

        if (registered)
        {
            boardManager.ReceiverDeactivated();
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
}
