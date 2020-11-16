using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HangarSlot : MonoBehaviour
{
    private Button button;
    private Image Icon;

    [SerializeField]
    private Dictionary<string, Sprite> iconDict;


    private Ship ship;
    // Start is called before the first frame update
    void Start()
    {
        this.button = gameObject.GetComponentInChildren<Button>();
        
    }
   
    public Button GetButton()
    {
        if (this.button == null)
        {
            this.button = gameObject.GetComponentInChildren<Button>();
        }
        return this.button;
    }

    private void getIcon()
    {
        Image[] icons = gameObject.GetComponentsInChildren<Image>(); ;
        foreach (Image icon in icons)
        {
            if (icon.gameObject.transform.parent != this.transform.parent)
            {
                Icon = icon;
            }
        }
    }

    public Ship GetShip()
    {
        return this.ship;
    }

    public void SetShip(GameObject s)
    {
        this.ship = s.GetComponent<Ship>();
        getIcon();
        if (ship)
        {
            this.Icon.sprite = GetIcon(ship.GetType().ToString());
        }
        else
        {
            print("Ship in hangarslot " + this + " was null");
        }
        

        
    }

    private Sprite GetIcon(string s)
    {
        string path = "Sprites/" + s;

        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }
   
}
