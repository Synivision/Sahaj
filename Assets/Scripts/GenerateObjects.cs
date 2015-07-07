using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GenerateObjects : MonoBehaviour {


	public Button generateObjectButton;

	public Button tearDownObjectButton;
	LevelManager levelManager;

	// Use this for initialization
	void Start () {
		levelManager = new LevelManager();
		generateObjectButton.onClick.AddListener(generateRandomObjects);
		tearDownObjectButton.onClick.AddListener(tearDownObjects);
	//from visual studio
	}
	
	// Update is called once per frame
	void Update () {

	}

	void generateRandomObjects(){
	

		levelManager.CreatePirate();
	}

	void tearDownObjects(){
		System.Console.WriteLine("teardown object");
		Debug.Log("teardown listener");
		levelManager.TearDownLevel();
		
		
	}
}
