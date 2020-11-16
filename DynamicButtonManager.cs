using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DynamicButtonManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] protected GameObject DynamicTargetButtonPref;
    [SerializeField] protected GameObject DynamicBattleButtonPref;
    [SerializeField] protected GameObject attackButtonPref;
    [SerializeField] protected GameObject defenceButtonPref;
    [SerializeField] protected GameObject healButtonPref;
    [SerializeField] protected GameObject StateSelectionUIElementPref;

    private Canvas canvas;

    private List<GameObject> targetButtonList;
    private List<GameObject> stateBtnList;

    public GameObject targetButton;
    public GameObject battleButton;
    public GameObject attackButton;
    public GameObject defenceButton;
    public GameObject healButton;

    public GameObject StateSelectionUIElement;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        targetButtonList = new List<GameObject>();
        stateBtnList = new List<GameObject>();

        DynamicTargetButtonPref = Resources.Load("prefabs/Buttons/TargetButton") as GameObject;
        DynamicBattleButtonPref = Resources.Load("prefabs/Buttons/BattleButton") as GameObject;

        StateSelectionUIElementPref = Resources.Load("prefabs/Buttons/StateSelection") as GameObject;
    }


    public void createTargetButton(GameObject s)
    {

        //instantiate the button object
        GameObject tButton = Instantiate(DynamicTargetButtonPref, new Vector3(0,0,0), Quaternion.identity);

        //set the parent of the button the canvas so that it appears on the UI for the player
        tButton.transform.SetParent(canvas.transform);
        tButton.transform.position = Camera.main.WorldToScreenPoint(s.transform.position);

        s.GetComponent<Ship>().setTargetButton(tButton);
        tButton.SetActive(false);
        targetButtonList.Add(tButton);
    }

    public void createStateSelectionUIElement()
    {                
        //make the attack button at the position of the gameobject
        StateSelectionUIElement = GameObject.Instantiate(StateSelectionUIElementPref);
        StateSelectionUIElement.transform.SetParent(canvas.transform);

        attackButton = GameObject.FindGameObjectWithTag("AttackButton");
        stateBtnList.Add(attackButton);
        defenceButton = GameObject.FindGameObjectWithTag("DefButton");
        stateBtnList.Add(defenceButton);
        healButton = GameObject.FindGameObjectWithTag("healbutton");
        stateBtnList.Add(healButton);

        StateSelectionUIElement.SetActive(false);

    }

    public void setStateSelectionPosition(Vector3 posToLoad)
    {
        StateSelectionUIElement.transform.position = Camera.main.WorldToScreenPoint(posToLoad);
        StateSelectionUIElement.SetActive(true);
    }

    public void activateStateButtons(Ship s)
    {
        stateBtnList[0].SetActive(true);
        stateBtnList[1].SetActive(true);
        if (s is SupportClass)
        {
            stateBtnList[2].SetActive(true);
        }
    }

    public void deActivateStateButtons()
    {
        for (int i = 0; i < stateBtnList.Count; i++)
        {
            stateBtnList[i].SetActive(false);
        }
    }

    public List<GameObject> getStateButtons()
    {
        return this.stateBtnList;
    }



    public void CreateBattleButtonAtPos(Vector3 position)
    {
        //instantiate the button object
        battleButton = Instantiate(DynamicBattleButtonPref, new Vector3(0, 0, 0), Quaternion.identity);

        //set the parent of the button the canvas so that it appears on the UI for the player
        battleButton.transform.SetParent(canvas.transform);
        battleButton.transform.position = Camera.main.WorldToScreenPoint(position);
    }
    public void deActivateTargetButtons()
    {
        for (int i = 0; i < targetButtonList.Count; i++)
        {
            targetButtonList[i].SetActive(false);
        }
    }

    public void setCanvas(Canvas canvas)
    {
        this.canvas = canvas;
    }
}
