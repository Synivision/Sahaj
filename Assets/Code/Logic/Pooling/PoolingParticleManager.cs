using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Extensions;
using Assets.Code.Logic.Logging;
using Assets.Code.Models.Pooling;
using Assets.Code.UnityBehaviours;
using UnityEngine;

namespace Assets.Code.Logic.Pooling
{
    public class PoolingParticleManager : IResolvableItem
    {
        /* REFERENCES */
        private readonly Logger _logger;
        private readonly PrefabProvider _prefabProvider;
        private readonly UnityReferenceMaster _unityReferenceMaster;

        /* PROPERTIES */
        private readonly GameObject _particleParent;
        private readonly Dictionary<string, ParticleSystem> _particleSystems;
        private readonly List<ActiveParticleLoop> _loops;

        private float _timeSinceLastSecond;

        public PoolingParticleManager(IoCResolver resolver)
        {
            _loops = new List<ActiveParticleLoop>();
            _particleSystems = new Dictionary<string, ParticleSystem>();

            // resolve references
            resolver.Resolve(out _logger);
            resolver.Resolve(out _prefabProvider);
            resolver.Resolve(out _unityReferenceMaster);

            // initialize properties
            _particleParent = Object.Instantiate(_prefabProvider.GetPrefab("empty_prefab"));
            _particleParent.name = "pooled_particle_system_parent";

            _unityReferenceMaster.OnGamePausedChangedEvent += OnGamePausedChanged;
        }

        private void OnGamePausedChanged(bool isGamePaused)
        {
            if (isGamePaused)
                foreach (var particleSystem in _particleSystems.Values)
                    particleSystem.Pause();
            else
                foreach (var particleSystem in _particleSystems.Values)
                {
                    particleSystem.Play();
                    particleSystem.enableEmission = false;
                }
        }

        public void FixedUpdate()
        {
            if (_unityReferenceMaster.IsPaused) return;

            _timeSinceLastSecond += Time.deltaTime;
            if (_timeSinceLastSecond >= 1f)
                _timeSinceLastSecond = 0;

            foreach (var loop in _loops)
            {
                if (loop.Location != null)
                    _particleSystems[loop.EffectName].transform.position = loop.Location.position + loop.Offset;
                else
                    _particleSystems[loop.EffectName].transform.position = loop.Offset;
                _particleSystems[loop.EffectName].startColor = loop.Tint;
                var emitCount = _particleSystems[loop.EffectName].emissionRate * loop.EmitModifier;
                var emitCountPerFixedDeltaTime = emitCount * Time.fixedDeltaTime;

                // if the pool emits more than 1 particle per fixed update loop
                // it will do so - it will NOT consider decimals though :/ but whatever
                if (emitCountPerFixedDeltaTime > 1)
                    _particleSystems[loop.EffectName].Emit((int)emitCountPerFixedDeltaTime);
                // if the pool emits less than 1 particle per fixed update loop
                // it will only do so every so oft
                else
                {
                    var closestBackFrame = FloatExtensions.Round(_timeSinceLastSecond, 1 / emitCount);

                    if (_timeSinceLastSecond <= closestBackFrame && _timeSinceLastSecond + Time.fixedDeltaTime > closestBackFrame)
                        _particleSystems[loop.EffectName].Emit(1);
                }
            }
        }

        public void Emit(string effectName, Vector3 position, Color tint, int amount)
        {
            if (!_particleSystems.ContainsKey(effectName))
                AddNewSytem(effectName);

            var particleSystem = _particleSystems[effectName];
            if (particleSystem == null) return;

            particleSystem.startColor = tint;
            particleSystem.transform.position = position;
            particleSystem.Emit(amount);
        }

        public ParticleLoopToken Loop(ActiveParticleLoop loopData)
        {
            if (!_particleSystems.ContainsKey(loopData.EffectName))
                AddNewSytem(loopData.EffectName);

            _loops.Add(loopData);

            return new ParticleLoopToken
            {
                End = () => RemoveLoop(loopData),
                Replace = newLoopData => ReplaceLoop(loopData, newLoopData)
            };
        }

        private void AddNewSytem(string name)
        {
            var fab = Object.Instantiate(_prefabProvider.GetPrefab(name));
            fab.name = name;
            fab.transform.SetParent(_particleParent.transform);

            var fabBehaviour = fab.GetComponent<ParticleSystem>();
            if (fabBehaviour == null)
                _logger.Log("WARNING! PoolingParticleManager particle type " + name + " is not a particle system!");

            _particleSystems.Add(name, fabBehaviour);
        }

        private void RemoveLoop(ActiveParticleLoop loop)
        {
            _loops.Remove(loop);
        }

        private ParticleLoopToken ReplaceLoop(ActiveParticleLoop oldLoop, ActiveParticleLoop newLoopData)
        {
            if (!_particleSystems.ContainsKey(newLoopData.EffectName))
                AddNewSytem(newLoopData.EffectName);

            oldLoop.EffectName = newLoopData.EffectName;
            oldLoop.Location = newLoopData.Location;
            oldLoop.EmitModifier = newLoopData.EmitModifier;
            oldLoop.Tint = newLoopData.Tint;

            return new ParticleLoopToken
            {
                End = () => RemoveLoop(oldLoop),
                Replace = replacement => ReplaceLoop(oldLoop, replacement)
            };
        }

        public void TearDown()
        {
            _loops.Clear();

            foreach (var particleSystem in _particleSystems.Values)
                Object.Destroy(particleSystem.gameObject);
            _particleSystems.Clear();

            _unityReferenceMaster.OnGamePausedChangedEvent -= OnGamePausedChanged;
        }
    }
}
