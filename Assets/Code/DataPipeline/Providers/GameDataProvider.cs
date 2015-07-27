using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Extensions;
using Assets.Code.Models;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class GameDataProvider : IResolvableItem
    {
        private readonly Dictionary<Type, Dictionary<string, IGameDataModel>> _data;

        public GameDataProvider()
        {
            _data = new Dictionary<Type, Dictionary<string, IGameDataModel>>();
        }

        public void AddData<T>(T data) where T : class, IGameDataModel
        {
            var targetType = typeof(T);

            if (data == null)
            {
                Debug.Log("WARNING! attempted to add null object of type " + targetType + " to GameDataProvider!");
                return;
            }

            if (!_data.ContainsKey(targetType))
                _data.Add(targetType, new Dictionary<string, IGameDataModel>());

            if(_data[targetType].ContainsKey(data.Name))
                Debug.Log("WARNING! duplicate name (" + data.Name + ") of type " + targetType + " added to GameDataProvider!");
			Debug.Log(data.Name);
			_data[targetType].Add(data.Name, data);
        }

        public T GetData<T>(string name) where T : class, IGameDataModel
        {
            var requestedType = typeof (T);

            if (!_data.ContainsKey(requestedType))
            {
                Debug.Log("WARNING! no models of type " + requestedType + " does not exist");
                return null;
            }

            if (!_data[requestedType].ContainsKey(name)) {
                Debug.Log("WARNING! model of type " + requestedType + " with name " + name + " does not exist");
                return null;
            }

            return _data[requestedType][name] as T;
        }

        public T GetRandomData<T>() where T : class, IGameDataModel
        {
            var requestedType = typeof(T);

            if (!_data.ContainsKey(requestedType))
            {
                Debug.Log("WARNING! no models of type " + requestedType + " does not exist");
                return null;
            }

            return _data[requestedType].ToList().GetRandomItem().Value as T;
        }

        public List<T> GetAllData<T>() where T : class
        {
            var requestedType = typeof (T);

            if (!_data.ContainsKey(requestedType))
            {
                Debug.Log("WARNING! no models of type " + requestedType + " does not exist");
                return null;
            }

            return _data[requestedType].Select(item => item.Value as T).ToList();
        } 
    }
}
