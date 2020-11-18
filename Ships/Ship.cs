using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    protected List<string> selectedTargets;
    protected Dictionary<string, GameObject> PossibleTargets;
    
    public bool isOnActiveFleet;

    public int Side;

    private List<GameObject> stateButtonList;
    
    protected GameObject targetButton;

    public Slider hpSlider;
    protected Text text;


    public enum State
    {
        neutral,
        attack,
        defend,
        heal
    }

    //1 is for player ship 2 is for enemy ship
    public State state;

    [SerializeField]
    protected int attackdamage;
    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int health;
    [SerializeField]
    protected int healAmount;

    protected int totalHealth;

    public Hangar hangar;


    public UnityEvent<string> onTargetPress;


    protected void OnTargetPress()
    {
        onTargetPress.Invoke(this.name);
        Debug.Log("button pressed for " + this.name);
    }


    public void OnTargetSelectionCallback(string s)
    {
        this.SelectTargets(s);
    }


    public virtual void Init()
    {
        //isOnActiveFleet = false;
        totalHealth = this.GetHealth();
        hpSlider.maxValue = this.totalHealth;
        text = hpSlider.gameObject.GetComponent<Text>();
        updateHpBar();
        targetButton.GetComponent<Button>().onClick.AddListener(OnTargetPress);

    }

    public virtual void InitializeStats()
    {

    }

    #region UIelements
    public void setTargetButton(GameObject btn)
    {
        this.targetButton = btn;
        this.moveTargetButton(new Vector3(40, 0, 0));
    }

    public void activateButton()
    {
        this.targetButton.gameObject.SetActive(true);
    }

    public void deactivateButton()
    {
        this.targetButton.gameObject.SetActive(false);
    }

    public void moveTargetButton(Vector3 pos)
    {
        this.targetButton.transform.position += pos;
    }

    public void setStateButtons(List<GameObject> btnList)
    {
        this.stateButtonList = btnList;
    }

    public void setState(Ship.State state)
    {
        this.state = state;
    }

    public List<GameObject> getStateButtons()
    {
        return this.stateButtonList;
    }

    public void setButtonText(string text)
    {
        this.targetButton.GetComponentInChildren<Text>().text = text;
    }

    private void updateHpBar()
    {
        hpSlider.maxValue = this.totalHealth;
        text.text = this.GetHealth().ToString() + "/" + this.totalHealth.ToString();
        hpSlider.value = this.GetHealth();
    }

    #endregion
    public int GetSpeed()
    {
        return this.speed;
    }

    public int GetAttDamage()
    {
        return this.attackdamage;
    }

    public int GetHealth()
    {
        return this.health;
    }

    public void takeDamage(int amount)
    {
        if (this.health - amount <= 0)
        {
            this.health -= amount;
            print(this.name + " was attacked and got destroyed in takeDamage()");
            updateHpBar();
            this.DIE();
        }
        else
        {
            this.health -= amount;
            updateHpBar();
            print(this.name + " was attacked and got damaged, remaining health: " + this.health);
        }
    }

    public void heal(int amount)
    {
        this.health += amount;
        updateHpBar();
    }

    public virtual void attack()
    {
        Ship _ship = this;

        if (hasDied() == false)
        {
            print(this.name + "'s dead level is: " + this.hasDied());
            foreach (string t in selectedTargets)
            {
                Ship tship = PossibleTargets[t].GetComponent<Ship>();

                if (tship.hasDied())
                {
                    print(this.name + " tried attacking while it is dead");
                }
                else
                {
                    print(this.name + " Attacked " + PossibleTargets[t]);
                    tship.takeDamage(this.attackdamage);
                }
            }
        }
        else
        {
            print(this.name + " died and tried attacking");
        }
        this.clearSelectedTargets();
        this.clearTargets();
    }

    public void resetHealth()
    {
        this.health = totalHealth;
    }

    public void DIE()
    {
        if (isOnActiveFleet)
        {
            hangar.removeShipfromFleet(this.gameObject);
        }

        print(this.name + " died in DIE()");

        this.hpSlider.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void AddTargets(List<GameObject> s) 
    {
        if (PossibleTargets == null)
        {
            PossibleTargets = new Dictionary<string, GameObject>(0);
        }


        PossibleTargets.Clear();

        foreach (GameObject ship in s)
        {
            PossibleTargets.Add(ship.name, ship);
        }

        
    }

    public void SelectTargets(List<string> t)
    {
        if (selectedTargets == null)
        {
            selectedTargets = new List<string>();
        }
        selectedTargets = t;
    }

    public void SelectTargets(string t)
    {
        List<string> targets = new List<string>();
        targets.Add(t);

        SelectTargets(targets);
    }

    public void clearTargets()
    {
        PossibleTargets.Clear();
        selectedTargets.Clear();
    }

    public void clearSelectedTargets()
    {
        selectedTargets.Clear();
    }

    public Dictionary<string, GameObject> GetTargets()
    {
        if (PossibleTargets == null)
        {
            PossibleTargets = new Dictionary<string, GameObject>();
        }

        return PossibleTargets;
    }

    public void increaseSpeed(int amount)
    {
        this.speed += amount;
    }
    public void increaseHealth(int amount)
    {
        this.totalHealth += amount;
        this.health += amount;
    }
    public void increaseDamage(int amount)
    {
        this.attackdamage += amount;
    }

    public void increaseHealAmount(int amount)
    {
        this.healAmount += amount;
    }

    public List<string> GetSelectedTargets()
    {
        if (selectedTargets == null)
        {
            selectedTargets = new List<string>();
        }
        return selectedTargets;
    }

    public bool hasDied()
    {
        if (this.health <= 0)
        {
            Debug.Log(this + " died in hasDied()");
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
