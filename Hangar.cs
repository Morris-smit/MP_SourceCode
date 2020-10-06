using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hangar : MonoBehaviour
{
    private Dictionary<string, GameObject> hangar;
    private Dictionary<string, GameObject> fleet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Dictionary<string, GameObject> GetFleet()
    {
        if (fleet == null)
        {
            fleet = new Dictionary<string, GameObject>();
        }
        return fleet;
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

    public List<Ship> GetshipList()
    {
        List<Ship> shipfleet = new List<Ship>();
        Dictionary<string, GameObject> dictionary = GetFleet();

        for (int i = 0; i < GetFleet().Count; i++)
        {
            Ship ship = dictionary.ElementAt(i).Value.GetComponent<Ship>();

            shipfleet.Add(ship);
        }

        return shipfleet;
    }

    public void AddShipToHangar(GameObject s)
    {
        if (hangar == null)
        {
            hangar = new Dictionary<string, GameObject>();
        }

        hangar.Add(s.name, s);
        print(s.name + "Added for: " + this);
    }

    public void AddShipToFleet(GameObject s)
    {
        if (fleet == null)
        {
            fleet = new Dictionary<string, GameObject>();
        }

        fleet.Add(s.name, s);
        s.GetComponent<Ship>().SetHangar(this);
    }


    public void moveShipToFleet(GameObject s)
    {
        fleet.Add(s.name, s);
        hangar.Remove(s.name);
    }

    public void moveShipToHangar(GameObject s)
    {
        hangar.Add(s.name, s);
        fleet.Remove(s.name);
    }

    public void removeShipfromFleet(GameObject s)
    {
        fleet.Remove(s.name);
    }
}
