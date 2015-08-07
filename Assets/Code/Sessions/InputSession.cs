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
	}


	public string CurrentlySelectedPirateName{
		get{return _data.Name;}
		set{_data.Name = value;}
	}
	//	set{fire off event; data = value}
}
