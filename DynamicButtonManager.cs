using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DynamicButtonManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] protected GameObject DynamicButtonPref;

    [SerializeField]
    private Canvas canvas;

    private List<GameObject> btnList;
    
    private GameObject targetButton;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        btnList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createTargetButton(GameObject s)
    {
        Debug.Log("creating button for " + s.name);

        //instantiate the button object
        targetButton = Instantiate(DynamicButtonPref, new Vector3(0,0,0), Quaternion.identity);

        //set the parent of the button the canvas so that it appears on the UI for the player
        targetButton.transform.SetParent(canvas.transform);
        targetButton.transform.position = Camera.main.WorldToScreenPoint(s.transform.position);

        s.GetComponent<Ship>().setTargetButton(targetButton);
        targetButton.SetActive(false);
        btnList.Add(targetButton);
        Debug.Log("button created for " + s.name);
    }

    public void deActivateTargetButtons()
    {
        for (int i = 0; i < btnList.Count; i++)
        {
            btnList[i].SetActive(false);
        }
    }
}
