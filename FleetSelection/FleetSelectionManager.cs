using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FleetSelectionManager : MonoBehaviour
{
    public Hangar _playerHangar;

    private HangarSlot[] hangarSlots;

    private GameObject StatOverView;

    [SerializeField]
    private GameObject battleButton;


    [Header("StatOverView")]
    [SerializeField]
    private Text NameText;
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Text DamageText;
    [SerializeField]
    private Text SpeedText;
    [SerializeField]
    private GameObject IconHolder;
    [SerializeField]
    private GameObject addShipToFleetButton;
    [SerializeField]
    private GameObject removeShipFromFleetButton;

    // Start is called before the first frame update
    void Start()
    {
        _playerHangar = PersistantDataManager.Instance.GetPlayerHangar();
        Debug.Log(_playerHangar.GetHangar().Count);
        hangarSlots = gameObject.GetComponentsInChildren<HangarSlot>();
        StatOverView = GameObject.FindGameObjectWithTag("StatOverView");

        //SetShips(_playerHangar.GetHangar());
        //updateSlots();

        StatOverView.SetActive(false);
        this.gameObject.SetActive(false);
        battleButton.SetActive(false);
        
    }
    private void test(HangarSlot selectedSlot)
    {
        print("test " + selectedSlot);
    }
    private void SetShips(Dictionary<string, GameObject> shipList)
    {
        for (int i = 0; i < shipList.Count; i++)
        {
            hangarSlots[i].SetShip(shipList.ElementAt(i).Value);
        }
    }

    public void updateHangar(Hangar hangar)
    {
        SetShips(hangar.GetHangar());
        List<GameObject> activeFleet = hangar.GetFleetList();
        for (int i = 0; i < activeFleet.Count; i++)
        {
            if (activeFleet[i].GetComponent<Ship>().isOnActiveFleet)
            {
                activeFleet[i].GetComponent<Ship>().isOnActiveFleet = false;
            }
        }
        updateSlots();
    }
    private void OpenStatOverview(Ship s)
    {
        addShipToFleetButton.SetActive(false);
        addShipToFleetButton.GetComponent<Button>().onClick.RemoveAllListeners();

        removeShipFromFleetButton.SetActive(false);
        removeShipFromFleetButton.GetComponent<Button>().onClick.RemoveAllListeners();

        //print(_playerHangar.GetFleet().Count);

        StatOverView.SetActive(true);
        if (s.isOnActiveFleet)
        {
            removeShipFromFleetButton.SetActive(true);
            removeShipFromFleetButton.GetComponent<Button>().onClick.AddListener(delegate () { removeShipFromFleet(s.gameObject); });
        }
        else
        {
            addShipToFleetButton.SetActive(true);
            addShipToFleetButton.GetComponent<Button>().onClick.AddListener(delegate () { addShipToFleet(s.gameObject); });
        }
        Ship _ship = s.GetComponent<Ship>();

        NameText.text = s.name;
        healthText.text = "health: " + _ship.GetHealth().ToString();
        DamageText.text = "Damage: " + _ship.GetAttDamage().ToString();
        SpeedText.text =  "Speed: " + _ship.GetSpeed().ToString();

        IconHolder.GetComponent<Image>().sprite = GetIcon(s.GetType().ToString());
    } 
    private void addShipToFleet(GameObject s)
    {
        _playerHangar.AddShipToFleet(s);
        addShipToFleetButton.SetActive(false);

        if (_playerHangar.GetFleet().Count == 3)
        {
            battleButton.SetActive(true);
        }
        else
        {
            battleButton.SetActive(false);
        }


        removeShipFromFleetButton.SetActive(true);
        removeShipFromFleetButton.GetComponent<Button>().onClick.AddListener(delegate () { removeShipFromFleet(s.gameObject); });

        PersistantDataManager.Instance.SetPlayerHangar(_playerHangar);
    }

    public Hangar getPlayerHangar()
    {
        return _playerHangar;
    }

    private void removeShipFromFleet(GameObject s)
    {
        _playerHangar.removeShipfromFleet(s);
        removeShipFromFleetButton.SetActive(false);

        if (_playerHangar.GetFleet().Count == 3)
        {
            battleButton.SetActive(true);
        }
        else
        {
            battleButton.SetActive(false);
        }


        addShipToFleetButton.SetActive(true);
        addShipToFleetButton.GetComponent<Button>().onClick.AddListener(delegate () { addShipToFleet(s.gameObject); });

        PersistantDataManager.Instance.SetPlayerHangar(_playerHangar);

    }
    private void onSlotSelection(HangarSlot selectedSlot)
    {
        OpenStatOverview(selectedSlot.GetShip());
        //TODO: make it clear which ship is in acitveFleet
    }

    private void updateSlots()
    {
        for (int i = 0; i < hangarSlots.Count() - 1; i++)
        {
            if (hangarSlots[i].GetShip())
            {
                HangarSlot hangarSlot = hangarSlots[i];
                hangarSlot.GetButton().onClick.AddListener(delegate () { onSlotSelection(hangarSlot); });
            }
            else
            {
                HangarSlot hangarSlot = hangarSlots[i];
                hangarSlot.GetButton().onClick.AddListener(delegate () { test(hangarSlot); });
            }
        }
    }

    private Sprite GetIcon(string s)
    {
        string path = "Sprites/" + s;

        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }

}
