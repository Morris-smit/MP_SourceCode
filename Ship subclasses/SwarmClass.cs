using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmClass : Ship
{
    
    // Start is called before the first frame update
    void Start()
    {
        this.health = 7;
        this.attackdamage = 9;
        this.speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void attack()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            GameObject s = Targets.ElementAt(i).Value;
            Ship _s = s.GetComponent<Ship>();

            _s.takeDamage(this.attackdamage / 3);

            hasAttacked = true;
        }
    }
}
