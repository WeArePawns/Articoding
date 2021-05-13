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

    public void finishLevel()
    {
        penguinAnim.SetTrigger("Walk");
        streamRoomAnim.SetTrigger("Finish");
    }

    public void gameOver()
    {
        penguinAnim.SetTrigger("Walk");
        streamRoomAnim.SetTrigger("GameOver");
    }
    public void retry()
    {
        penguinAnim.SetTrigger("Idle");
        streamRoomAnim.SetTrigger("Retry");
    }

}
