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

public class UserController : PoolingBehaviour {

	//Refences
	private  IoCResolver _resolver;
	private  Messager _messager;
	private  MessagingToken _onPirateCreated;

	private UserModel _userModel;

	public void Initialize (IoCResolver resolver, UserModel data, LevelManager levelManager)
	{
		//Get Resolver
		_resolver = resolver;
		_resolver.Resolve (out _messager);
		
	}


}
