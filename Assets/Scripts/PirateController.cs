using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MoveBehaviour))]
public class PirateController : MonoBehaviour {

	public MoveBehaviour moveBehaviour;
	Vector3 startPos;
	Vector3 endPos;
	float timer;

	void Start(){
			
		moveBehaviour = gameObject.GetComponent<MoveBehaviour>();


	}

	
	void Update () {

		if(Time.time -timer >1){

			RandomPosition();
		}
		moveBehaviour.move(startPos,endPos,timer);

	}


	void RandomPosition()
	{
		timer = Time.time;
		startPos = transform.position;
		endPos = new Vector3(Random.Range(-50f, 20f), 2, Random.Range(-50f, 20f));
	}

}
