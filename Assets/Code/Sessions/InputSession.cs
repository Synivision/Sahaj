using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.DataPipeline;
public class InputSession : IResolvableItem  {
	private InputSessionData _data;

	public void Initialize (InputSessionData data){

		_data = data;
        CurrentShipAttackCost = 0;
	}


	public string CurrentlySelectedPirateName{
		get{return _data.Name;}
		set{_data.Name = value;}
	}

	public string CurrentlySelectedShipAttackName
	{
		get { return _data.ShipAttackName; }
		set { _data.ShipAttackName = value; }
	}

    public int CurrentShipAttackCost
    {
        get { return _data.ShipAttackCost; }
        set { _data.ShipAttackCost = value; }
    }
    //	set{fire off event; data = value}
}
