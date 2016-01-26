using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

public class PirateGenerator : IResolvableItem {

	private Dictionary <string,List<PirateGeneratorModel>> _piratesInProcess;
	private  IoCResolver _resolver;
	private UnityReferenceMaster _unityReference;
	private PlayerManager _playerManager;
	private Messager _messager;
	public void Initialize (IoCResolver resolver)
	{
		//Get Resolver
		_resolver = resolver;

		//Resolve required objects
		_resolver.Resolve (out _unityReference);
		_resolver.Resolve (out _playerManager);
		_resolver.Resolve (out _messager);

		_piratesInProcess =new Dictionary <string,List<PirateGeneratorModel>>();
	}

	//Add the pirate to be created to the list in this method
	public void GeneratePirate(PirateModel pirate,string buildingName,System.TimeSpan startTime){
	
		PirateGeneratorModel model = new PirateGeneratorModel ();
		model.PirateModel = pirate;
		model.StartTime = startTime;
		model.PirateGenerationStatus = (int)PirateGeneratorModel.Status.IN_QUEUE;

		//list Of Pirates For Any Key To Building
		List<PirateGeneratorModel> listOfPirates;
	
		//check if this building is creating any pirate or not
		if (_piratesInProcess.TryGetValue (buildingName, out listOfPirates)) {

			//add pirate to list obtained from dictionary of pirates
			if(listOfPirates != null){

				listOfPirates.Add(model);
				if(listOfPirates.Count == 1){
					_unityReference.Delay (() => DoneGeneratingPirate(model,buildingName), pirate.TrainingTime);	

				}
			}else{

				listOfPirates = new List<PirateGeneratorModel>();
				listOfPirates.Add (model);
			}

			_piratesInProcess[buildingName] = listOfPirates;

		} else { //create a new entry for this type of pirate for this building

			//new list Of Pirates For a new Key To Building
			listOfPirates = new List<PirateGeneratorModel>();

			model.PirateGenerationStatus = (int)PirateGeneratorModel.Status.IN_PROCESS;

			listOfPirates.Add(model);

			_piratesInProcess.Add (buildingName,listOfPirates);

			//Create first pirate
			_unityReference.Delay (() => DoneGeneratingPirate(model,buildingName), pirate.TrainingTime);	

		}
	}

	void DoneGeneratingPirate(PirateGeneratorModel model,string buildingName){

		//increase the pirates count in pirate count dict
		int newPirateCount = 0;
		_playerManager.Model.PirateCountDict.TryGetValue (model.PirateModel.Name, out newPirateCount);
		newPirateCount++;
		_playerManager.Model.PirateCountDict[model.PirateModel.Name] = newPirateCount;

		List<PirateGeneratorModel> tempModelList = new List<PirateGeneratorModel>();

		//Delete this pirate from pirates in process list
		if (_piratesInProcess.TryGetValue (buildingName, out tempModelList)) {

			tempModelList.RemoveAt(0);

			/*for(int i = 0; i < tempModelList.Count; i++){

				Debug.Log(tempModelList[i].PirateModel.Name);

			}*/
			//Debug.Log ("Count of pirates left = " + tempModelList.Count);
			//Start next pirate creation (if any)
			if (tempModelList.Count > 0) {

				//set status of first pirate to in process
				tempModelList[0].PirateGenerationStatus = (int)PirateGeneratorModel.Status.IN_PROCESS;
				_unityReference.Delay (() => DoneGeneratingPirate(tempModelList[0],buildingName), tempModelList[0].PirateModel.TrainingTime);
				
			}//else pirates in queue are completed :) Enjoy!!


			_piratesInProcess[buildingName] = tempModelList;

			_messager.Publish(new NewPirateGeneratedMessage{
				PirateModel = model.PirateModel
			});
		}
	}

	//returns time in seconds
	public System.TimeSpan GetTimeToCompletionOfPirate(string buildingName){
	
		System.TimeSpan timeLeft = new System.TimeSpan();

		//list Of Pirates For a new Key To Building
		List<PirateGeneratorModel> listOfPirates;

		if(_piratesInProcess.TryGetValue(buildingName,out listOfPirates)){

			int piratesLeft = listOfPirates.Count;

			foreach(var pirate in listOfPirates){

				if(pirate.PirateGenerationStatus == (int)PirateGeneratorModel.Status.IN_QUEUE){

					timeLeft += System.TimeSpan.FromSeconds((double)pirate.PirateModel.TrainingTime);
				
				}else{

					//time remaining for processing pirate
					//var time = (System.TimeSpan.FromSeconds((double)pirate.PirateModel.TrainingTime) - (System.TimeSpan.FromSeconds((double)Time.time)));
					var time = (System.TimeSpan.FromSeconds((double)Time.time) - pirate.StartTime );
					//add this time only if it is positive because at this instant it might be possible that status changed
					timeLeft += (time < System.TimeSpan.FromSeconds((double)pirate.PirateModel.TrainingTime)) ? (System.TimeSpan.FromSeconds((double)pirate.PirateModel.TrainingTime) - time): System.TimeSpan.FromSeconds(0);
				
				}

			}

		}

		return timeLeft;
	
	}

	public List<string> PiratesBiengGeneratedForBuilding(string buildingName){

		List<string> result= new List<string>();

		List<PirateGeneratorModel> listOfPirates;

		if (_piratesInProcess.TryGetValue (buildingName, out listOfPirates)) {

			foreach(var pirateBiengGenerated in listOfPirates){

				result.Add (pirateBiengGenerated.PirateModel.Name);

			}
		
		}

		return result;

	}

}
