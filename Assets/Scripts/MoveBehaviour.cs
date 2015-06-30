using UnityEngine;
using System.Collections;

public class MoveBehaviour : MonoBehaviour {


	void Start () {
		

	}
	
	void Update () {

	}

	public void move(Vector3 startPos, Vector3 endPos, float timer){

		transform.position = Vector3.Lerp(startPos, endPos, (Time.time - timer)/2);

	}

}
