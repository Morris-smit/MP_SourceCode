﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    new void Awake()
    {
        hangar = this.GetComponent<Hangar>();
    }

    public void Sethangar(Hangar hangar)
    {
        this.hangar = hangar;
    }
}
