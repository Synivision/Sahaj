using UnityEngine;
using System.Collections;
using Assets.Code.Models;
using UnityEngine.UI;
using System.Collections.Generic;

public class InputSession  {
	private InputSessionData _data;

	public InputSession(InputSessionData data){

		_data = data;
	}

	public string CurrentlySelectedPirateName{
		get{return _data.Name;}
		set{_data.Name = value;}
	}
	//	set{fire off event; data = value}
}
