using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hangar : MonoBehaviour
{
    private Dictionary<string, GameObject> hangar;
    public Dictionary<string, GameObject> fleet;

    public List<Ship> fleetDisplayList;
    // Start is called before the first frame update
    void Start()
    {
        fleetDisplayList = new List<Ship>();
    }

    public Dictionary<string, GameObject> GetFleet()
    {
        if (fleet == null)
        {
            fleet = new Dictionary<string, GameObject>();
        }
        return fleet;
    }
    public Dictionary<string, GameObject> GetHangar()
    {
        if (hangar == null)
        {
            hangar = new Dictionary<string, GameObject>();
        }
        return hangar;
    }
    public List<GameObject> GetFleetList()
    {
        List<GameObject> _fleet = new List<GameObject>();
        Dictionary<string, GameObject> dictionary = GetFleet();

        for (int i = 0; i < GetFleet().Count; i++)
        {
            GameObject ship = dictionary.ElementAt(i).Value;

            _fleet.Add(ship.gameObject);
        }

        return _fleet;
    }

    public List<Ship> GetShipList()
    {
        List<Ship> shipfleet = new List<Ship>();
        Dictionary<string, GameObject> dictionary = GetFleet();

        for (int i = 0; i < GetFleet().Count; i++)
        {
            if (dictionary.ElementAt(i).Value == null)
            {
                Debug.Log("ship was null");
            }
            else
            {
                Ship ship = dictionary.ElementAt(i).Value.GetComponent<Ship>();

                shipfleet.Add(ship);
            }
            
        }

        return shipfleet;
    }

    public void AddShipToHangar(GameObject s)
    {
        if (hangar == null)
        {
            hangar = new Dictionary<string, GameObject>();
        }
        
        GameObject objectToAdd = GameObject.Instantiate(s);
        objectToAdd.name = s.name;
        DontDestroyOnLoad(objectToAdd);
        objectToAdd.GetComponent<Ship>().hangar = this;
        hangar.Add(objectToAdd.name, objectToAdd);
        print(objectToAdd.name + "Added for: " + this);
    }
   
    public void AddShipToFleet(GameObject s)
    {
        if (fleet == null)
        {
            fleet = new Dictionary<string, GameObject>();
        }
    
        if (fleet.ContainsKey(s.name))
        {
            print(s.name + " is already in fleet");
        }
        else
        {
            fleet.Add(s.name, s);
            //fleetDisplayList.Add(s.GetComponent<Ship>());
            s.GetComponent<Ship>().isOnActiveFleet = true;
        }
        
    }

    public void removeShipfromFleet(GameObject s)
    {
        if (fleet.ContainsKey(s.name))
        {
            fleet.Remove(s.name);
            print(s.name + "was removed from fleet. fleet count is now " + fleet.Count.ToString());
            //s.GetComponent<Ship>().resetHealth();
            s.GetComponent<Ship>().isOnActiveFleet = false;
        }
        else
        {
            print("");
        }
        
    }
}
