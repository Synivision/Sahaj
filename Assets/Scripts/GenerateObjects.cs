using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GenerateObjects : MonoBehaviour {


	public Button generateObjectButton;
	public GameObject gameObjectPrefab;

	// Use this for initialization
	void Start () {

		generateObjectButton.onClick.AddListener(generateRandomObjects);

	//from visual studio
	}
	
	// Update is called once per frame
	void Update () {

	}

	void generateRandomObjects(){
	


			Vector3 position = new Vector3(Random.Range(-50f, 20.0F), 10, Random.Range(-50f , 20.0f));
			Instantiate(gameObjectPrefab, position, Quaternion.identity);



	}
}
