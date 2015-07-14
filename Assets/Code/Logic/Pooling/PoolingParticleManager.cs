using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using UnityEngine;

namespace Assets.Code.Logic.Pooling
{
    public class PoolingParticleManager : IResolvableItem
    {
        /* REFERENCES */
        private readonly PrefabProvider _prefabProvider;

        /* PROPERTIES */
        private readonly Dictionary<string, ParticleSystem> _particleSystems;

        public PoolingParticleManager(IoCResolver resolver)
        {
            _particleSystems = new Dictionary<string, ParticleSystem>();

            // resolve references
            resolver.Resolve(out _prefabProvider);
        }

        public void Emit(string effectName, Vector3 position, Color tint, int amount)
        {

			if(!_particleSystems.ContainsKey(effectName))
				AddNewSytem(effectName	);

			var particleSystem = _particleSystems[effectName];
           
				particleSystem.startColor = tint;
            particleSystem.transform.position = position;
            
			particleSystem.Emit(amount);
        }

        private void AddNewSytem(string name)
        {
            var fab = Object.Instantiate(_prefabProvider.GetPrefab(name));
		//	var fab =new ObjectPool(_prefabProvider.GetPrefab(name));
			fab.name = name;

            var fabBehaviour = fab.GetComponent<ParticleSystem>();
            if (fabBehaviour == null)
                Debug.Log("WARNING! PoolingParticleManager particle type " + name + " is not a particle system!");

			_particleSystems.Add(name, fabBehaviour);
        }

        public void TearDown()
        {
            foreach (var particle in _particleSystems)
                Object.Destroy(particle.Value.gameObject);
        }
    }
}
