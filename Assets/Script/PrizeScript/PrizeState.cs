using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrizeState
{
    protected Landing_Prize ObjectData;

    public PrizeState(Landing_Prize _objectdata)
    {
        ObjectData = _objectdata;
    }

    public virtual void TouchFunction(Collision2D _col)
    {
        
    }
}
