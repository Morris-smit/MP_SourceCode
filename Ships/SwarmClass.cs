using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwarmClass : Ship
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void InitializeStats()
    {
        this.health = 7;
        this.attackdamage = 9;
        this.speed = 4;
    }
    public override void attack()
    {
        for (int i = 0; i < PossibleTargets.Count; i++)
        {
            GameObject s = PossibleTargets.ElementAt(i).Value;
            Ship _s = s.GetComponent<Ship>();

            _s.takeDamage(this.attackdamage / 3);

        }
    }

}
