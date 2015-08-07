using UnityEngine;
using Assets.Code.Ui.CanvasControllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.DataPipeline;
using Assets.Code.UnityBehaviours.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.Logic.Pooling;
using Assets.Code.DataPipeline.Providers;

public class PlayerManager  : IResolvableItem {
	
	//Refences
	private  IoCResolver _resolver;
	private  Messager _messager;
	private  MessagingToken _onPirateCreated;
	
	private PlayerModel _playerModel;
	
	public void Initialize (IoCResolver resolver, PlayerModel data, LevelManager levelManager)
	{
		//Get Resolver
		_resolver = resolver;
		_resolver.Resolve (out _messager);
		_playerModel = data;

		//Debug.Log("Player Manager Created");
	}
	
	
}
