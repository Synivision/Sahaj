using UnityEngine;
using Assets.Code.Ui.CanvasControllers;

[RequireComponent(typeof(MoveBehaviour))]
[RequireComponent(typeof(StatsBehaviour))]
public class PirateController : MonoBehaviour {


    private MoveBehaviour _moveBehaviour;
	private StatsBehaviour _statsBehaviour;
	private GameObject target;
	private PirateModel _dataModel;
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
    }



    private void ResetLerp()
    {
        _moveBehaviour.LerpToTarget(new Vector3(Random.Range(-50f, 20f), 2, Random.Range(-50f, 20f)));
    }

	public void UpdateUiPanel(){

		UnityEngine.Debug.Log(_dataModel.Health+" : "+_dataModel.Descipriton+" !");
		PirateInfoCanvasController.setDisplayInfo(_dataModel);
	}

}
