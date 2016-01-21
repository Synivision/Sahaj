using UnityEngine;
using System.Collections;
using Assets.Code.Models;

public class PirateGeneratorModel : IGameDataModel
{
	public enum Status{
		IN_PROCESS=0,
		IN_QUEUE=1
	};

	public int PirateGenerationStatus{get;set;}
	public PirateModel PirateModel{ get; set;}
	public System.TimeSpan StartTime{ get; set;}
	public string Name { get; set;}
}