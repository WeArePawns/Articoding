using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseListener
{
    void OnMouseButtonDown(int index);
    void OnMouseButtonUp(int index);
}
