using UnityEngine;
using Assets.Code.Ui.CanvasControllers;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(MoveBehaviour))]
[RequireComponent(typeof(StatsBehaviour))]
public class PirateController : MonoBehaviour {


    private MoveBehaviour _moveBehaviour;
	private StatsBehaviour _statsBehaviour;
	private GameObject target;
	private PirateModel _dataModel;
	private List<GameObject> _knownPirates;

	private float time;

    void Start()
    {
        _moveBehaviour = GetComponent<MoveBehaviour>();
		_statsBehaviour = GetComponent<StatsBehaviour>();

        ResetLerp();
        _moveBehaviour.OnLerpEndEvent += ResetLerp;

		_dataModel = new PirateModel();
		_dataModel.Health = 100;
		_dataModel.Descipriton = "pirate discription ";
		_dataModel.AttackDamage = Random.Range(10,20);
		_dataModel.Name = "Pirate No."+Random.Range(0,100).ToString();
    
		_knownPirates = LevelManager.GetKnownPirates();

	}


	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player"){

			if(_knownPirates.Count>1){

				var nearbyPlayers = _knownPirates.Where(pirate => Vector3.Distance(pirate.transform.position, transform.position) < 10).ToList();

				GameObject nearestPlayer = _knownPirates.OrderBy(pirate => Vector3.Distance(pirate.transform.position, transform.position)).ToList()[1];
				
				Debug.Log("Nearest Pirate to "+ this._dataModel.Name+ " is " +nearestPlayer.GetComponent<PirateController>().DataModel.Name);

			}

		}
}


	public void Update(){

	}


    private void ResetLerp()
    {
        _moveBehaviour.LerpToTarget(new Vector3(Random.Range(-50f, 20f), 2, Random.Range(-50f, 20f)));
    }

	public void UpdateUiPanel(){
		UnityEngine.Debug.Log(_dataModel.Health+" : "+_dataModel.Descipriton+" !");
		PirateInfoCanvasController.setDisplayInfo(_dataModel);
	}

	public PirateModel DataModel{

		get{
			return _dataModel;
		}

		set{
			_dataModel = value;

		}


	}

}
