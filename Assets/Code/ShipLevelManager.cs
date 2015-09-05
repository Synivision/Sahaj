using System.Linq;
using Assets.Code.Models;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;

public class ShipLevelManager {

	/* REFENCES */
	readonly GameDataProvider _gameDataProvider;
	private readonly PrefabProvider _prefabProvider;
	private SpriteProvider _spriteProvider;
	private readonly PoolingObjectManager _poolingObjectmanager;
	private Messager _messager;
	private readonly PoolingObjectManager _poolingObjectManager;
	private readonly IoCResolver _resolver;
	private readonly UnityReferenceMaster _unityReferenceMaster;

	//Grid Map generator
	private int AreaToCover = 25;
	public float GridSize = 10;
	private float _centreAdjustments;
	private string[,] blueprint;

	MapLayout _map;

	public ShipLevelManager (IoCResolver resolver, MapLayout map)
	{
		_resolver = resolver;
		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _gameDataProvider);
		_resolver.Resolve (out _poolingObjectmanager);
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _spriteProvider);
		_resolver.Resolve (out _poolingObjectManager);
		_resolver.Resolve(out _unityReferenceMaster);

		_map = map;

		//_messager.Subscribe<OpenShopMessage>(EnableShopCanvas);
		GenerateLevelMap();
	}

	public void GenerateLevelMap(){
		_unityReferenceMaster.AStarPlane.SetActive(true);
	}
	
	public void TearDown(){

		_unityReferenceMaster.AStarPlane.SetActive(false);
	}
}
