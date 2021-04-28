using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FollowingText : MonoBehaviour
{
    [SerializeField] private FollowingTextUI nameTextPrefab;
    [SerializeField] private Transform textPosition;
    private FollowingTextUI nameText;

    void Update()
    {
        if (nameText == null) return;
        //Hide the name tag if the object is not visible
        nameText.gameObject.SetActive(ObjectVisible());
        Vector3 namePos = Camera.main.WorldToScreenPoint(textPosition.transform.position);
        nameText.transform.position = namePos;
    }

    public void SetName(string name)
    {
        if (nameText == null)
            nameText = Instantiate(nameTextPrefab, GameObject.Find("Canvas").transform);
        nameText.transform.SetAsFirstSibling();

        nameText.SetText(name);
    }

    private void OnDestroy()
    {
        if(nameText != null)
            Destroy(nameText.gameObject);
    }

    private bool ObjectVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(textPosition.transform.position));
        RaycastHit hit;
        // Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1<<LayerMask.NameToLayer("LaserLayer"))))            
            return hit.collider.gameObject == gameObject || (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject == gameObject);
        return false;
    }
}
