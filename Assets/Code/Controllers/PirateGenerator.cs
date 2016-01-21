using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours;

public class PirateGenerator : IResolvableItem {

	private Dictionary <string,Dictionary<string,List<PirateGeneratorModel>>> _piratesInProcess;
	private  IoCResolver _resolver;
	private UnityReferenceMaster _unityReference;
	private PlayerManager _playerManager;

	public void Initialize (IoCResolver resolver)
	{
		//Get Resolver
		_resolver = resolver;

		//Resolve required objects
		_resolver.Resolve (out _unityReference);
		_resolver.Resolve (out _playerManager);

		_piratesInProcess =new Dictionary <string,Dictionary<string,List<PirateGeneratorModel>>>();
	}

	//Add the pirate to be created to the list in this method
	public void GeneratePirate(PirateModel pirate,string buildingName,System.TimeSpan startTime){
	
		PirateGeneratorModel model = new PirateGeneratorModel ();
		model.PirateModel = pirate;
		model.StartTime = startTime;
		model.PirateGenerationStatus = (int)PirateGeneratorModel.Status.IN_QUEUE;

		//dictionary of Pirates For This Building
		Dictionary<string,List<PirateGeneratorModel>> dictionaryofPirates;

		//check if this building is creating any pirate or not
		if (_piratesInProcess.TryGetValue (buildingName, out dictionaryofPirates)) {

			//list Of Pirates For Any Key To Building
			List<PirateGeneratorModel> listOfPirates;

			//check if any pirate of this type is in queue, if no then create it
			if(dictionaryofPirates.TryGetValue(pirate.Name,out listOfPirates)){

				//add pirate to list obtained from dictionary of pirates
				listOfPirates.Add(model);
				dictionaryofPirates[pirate.Name] = listOfPirates;

			}else{

				listOfPirates = new List<PirateGeneratorModel>();
				listOfPirates.Add (model);
				dictionaryofPirates.Add(pirate.Name,listOfPirates);

			}

			//add dictionary of pirates to our main dictionary
			_piratesInProcess[buildingName] = dictionaryofPirates;

		} else { //create a new entry for this type of pirate for this building
		
			//new dictionary of Pirates For This Building
			dictionaryofPirates = new Dictionary<string, List<PirateGeneratorModel>>();

			//new list Of Pirates For a new Key To Building
			List<PirateGeneratorModel> listOfPirates = new List<PirateGeneratorModel>();

			model.PirateGenerationStatus = (int)PirateGeneratorModel.Status.IN_PROCESS;

			listOfPirates.Add(model);

			dictionaryofPirates.Add(pirate.Name,listOfPirates);

			_piratesInProcess.Add (buildingName,dictionaryofPirates);

			//create first pirate of the list
			_unityReference.Delay (() => DoneGeneratingPirate(model,buildingName), pirate.TrainingTime);	

		}
			
	}

	void DoneGeneratingPirate(PirateGeneratorModel model,string buildingName){

		//increase the pirates count in pirate count dict
		int newPirateCount = 0;
		_playerManager.Model.PirateCountDict.TryGetValue (model.PirateModel.Name, out newPirateCount);
		newPirateCount++;
		_playerManager.Model.PirateCountDict.Add (model.PirateModel.Name,newPirateCount);

		Dictionary<string,List<PirateGeneratorModel>> tempDict;

		List<PirateGeneratorModel> tempModelList;

		//Delete this pirate from pirates in process list
		if (_piratesInProcess.TryGetValue (buildingName, out tempDict) && tempDict.TryGetValue (model.PirateModel.Name,out tempModelList)) {

			tempModelList.Remove(model);
			tempDict[model.PirateModel.Name] = tempModelList;
			
			//Start next pirate creation (if any)
			if (tempModelList.Count > 0) {
				//set status of first pirate to in process
				tempModelList[0].PirateGenerationStatus = (int)PirateGeneratorModel.Status.IN_PROCESS;
				tempDict[model.PirateModel.Name] = tempModelList;

				_unityReference.Delay (() => DoneGeneratingPirate(tempModelList[0],buildingName), tempModelList[0].PirateModel.TrainingTime);
				
			}//else pirates in queue are completed :) Enjoy!!

			_piratesInProcess[buildingName] = tempDict;
		
		}
	}

	//returns time in seconds
	public System.TimeSpan GetTimeToCompletionOfPirate(string buildingName,string pirateName){
	
		System.TimeSpan timeLeft = new System.TimeSpan();

		//dictionary of Pirates For This Building
		Dictionary<string,List<PirateGeneratorModel>> dictionaryofPirates;

		//list Of Pirates For a new Key To Building
		List<PirateGeneratorModel> listOfPirates;

		if(_piratesInProcess.TryGetValue(buildingName,out dictionaryofPirates) 
		   && dictionaryofPirates.TryGetValue(pirateName,out listOfPirates)){

			int piratesLeft = listOfPirates.Count;

			foreach(var pirate in listOfPirates){

				if(pirate.PirateGenerationStatus == (int)PirateGeneratorModel.Status.IN_QUEUE){

					timeLeft += System.TimeSpan.FromSeconds((double)pirate.PirateModel.TrainingTime);
				
				}else{

					//time remaining for processing pirate
					var time = (pirate.StartTime - System.TimeSpan.FromSeconds((double)pirate.PirateModel.TrainingTime));
					//add this time only if it is positive because at this instant it might be possible that status changed
					timeLeft += (time > System.TimeSpan.FromSeconds(0)) ? time : System.TimeSpan.FromSeconds(0);
				
				}

			}

		}

		return timeLeft;
	
	}

}
