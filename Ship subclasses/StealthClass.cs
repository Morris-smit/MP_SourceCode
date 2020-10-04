using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthClass : Ship
{
    // Start is called before the first frame update
    void Start()
    {
        this.health = 5;
        this.attackdamage = 4;
        this.speed = 7;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
