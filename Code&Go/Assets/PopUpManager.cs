using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [SerializeField] private PopUp popupPanel;

    private void Awake()
    {
        Instance = this;
        //gameObject.SetActive(false);
    }

    
}
