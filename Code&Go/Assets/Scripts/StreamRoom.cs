using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamRoom : MonoBehaviour
{
    public Animator penguinAnim;

    private Animator streamRoomAnim;

    private void Start()
    {
        streamRoomAnim = GetComponent<Animator>();
    }

    public void FinishLevel()
    {
        penguinAnim.SetTrigger("Walk");
        streamRoomAnim.SetTrigger("Finish");
    }

    public void GameOver()
    {
        penguinAnim.SetTrigger("Walk");
        streamRoomAnim.SetTrigger("GameOver");
    }
    public void Retry()
    {
        penguinAnim.SetTrigger("Idle");
        streamRoomAnim.SetTrigger("Retry");
    }

}
