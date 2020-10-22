using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private Ship[] _pShipList;
    private Ship[] _cShipList;


    public Ship[] getPShipList()
    {
        return this._pShipList;
    }

    public Ship[] getCShipList()
    {
        return this._cShipList;
    }
}
