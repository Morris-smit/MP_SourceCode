using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Hangar hangar;
    Dictionary<string, GameObject> Fleet;

    

    // Start is called before the first frame update
    void Start()
    {
        //Fleet = hangar.GetFleet(); 

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void Awake()
    {
        hangar = this.GetComponent<Hangar>();
    }


}
