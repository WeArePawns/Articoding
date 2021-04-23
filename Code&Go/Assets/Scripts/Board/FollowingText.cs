using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FollowingText : MonoBehaviour
{
    [SerializeField] private Text nameTextPrefab;
    private Text nameText;

    void Update()
    {
        if (nameText == null) return;
        //Hide the name tag if the object is not visible
        //nameText.gameObject.SetActive(ObjectVisible());

        Vector3 namePos = Camera.main.WorldToScreenPoint(this.transform.position);
        nameText.transform.position = namePos;
    }

    public void SetName(string name)
    {
        if (nameText == null)
            nameText = Instantiate(nameTextPrefab, GameObject.Find("Canvas").transform);

        nameText.text = name;
    }

    private void OnDestroy()
    {
        Destroy(nameText);
    }

    private bool ObjectVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(this.transform.position));
        RaycastHit hit;
        // Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            return hit.transform.gameObject == gameObject;
        return false;
    }
}
