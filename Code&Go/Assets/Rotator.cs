using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public float speed = 45;
    void Update()
    {
        var newRotation = (transform.rotation.eulerAngles.z - speed * Time.deltaTime) % 360;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, newRotation));
    }
}
