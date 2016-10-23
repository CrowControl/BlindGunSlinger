using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Resources.Scripts;
using Assets.Resources.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class HitBoxController : MonoBehaviour, IObserveSubject
    {
        //health
        public int HealthPoints = 1;
        public float HitToDeathDelay = .2f;

        //damage
        public int Damage = 1;
        private bool _isKilled;

        //time until attack
        public float TimeUntilAttack = 3;
        private float _elapsedTime;

        //hit grunts
        private AudioClip[] _hitGrunts;
        private int _curGruntIndex;

        //spawn and attack.
        private AudioClip _spawnAudioClip;
        private AudioClip _attackAudioClip;
        private AudioClip _deathAudioClip;

        //other components
        private Player _player;

        //observer
        private List<IObserver> _observers;

        private readonly List<AudioSpawn> _audioSpawns = new List<AudioSpawn>(); 

        void Awake()
        {
            _observers = new List<IObserver>();
            _player = Player.Instance;
            InitializeSoundClips();
            PlaySpawnClip();
        }

        #region Audio
        /// <summary>
        /// Initializes all the soundclips for this object.
        /// </summary>
        protected virtual void InitializeSoundClips()
        {
            SoundManager sound = SoundManager.Instance;

            _spawnAudioClip = sound.GetRandomClip("Shotgun Clicks");
            _attackAudioClip = sound.GetRandomClip("Shotgun Shots");
            _deathAudioClip = sound.GetRandomClip("Deathsounds");
        }

        private void SpawnAudioClip(AudioClip clip, float delay = 0)
        {
            AudioSpawn spawn = SoundManager.SpawnAudioSource(clip, transform.position, delay);
            _audioSpawns.Add(spawn);
        }

        /// <summary>
        /// PLays the spawn audio
        /// </summary>
        private void PlaySpawnClip()
        {
            SpawnAudioClip(_spawnAudioClip);
        }
        #endregion

        void Update()
        {
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime >= TimeUntilAttack)
                Attack();
        }

        /// <summary>
        /// Attack the player.
        /// </summary>
        protected virtual void Attack()
        {
            //spawn sound
            SpawnAudioClip(_attackAudioClip);

            //apply damage to player
            _player.ApplyDamage(Damage);

            //destroy this object after finishing the sound clip.
            Destroy();
        }

        #region Getting Hit
        /// <summary>
        /// Called when we got hit. plays a sound, applies damage.
        /// </summary>
        public virtual void GetHit()
        {
            //if health reaches 0, we die. But we wat until the hitgrunt is finished playing.
            HealthPoints--;
            if (HealthPoints <= 0)
                Die();
        }

        /// <summary>
        /// Plays a hit grunt audio clip
        /// </summary>
        private void PlayHitGrunt()
        {
            //loop index around
            if (_curGruntIndex >= _hitGrunts.Length)
                _curGruntIndex = 0;

            //choose grunt and play.
            AudioClip grunt = _hitGrunts[_curGruntIndex];
            SpawnAudioClip(grunt);

            //set index for next time.
            _curGruntIndex++;
        }

        /// <summary>
        /// cancel all still running audio clips.
        /// </summary>
        private void CancelAudioClips()
        {
            foreach (AudioSpawn spawn in _audioSpawns.Where(spawn => spawn != null))
                spawn.Cancel();
        }

        protected virtual void Die()
        {
            CancelAudioClips();
            SpawnAudioClip(_deathAudioClip, HitToDeathDelay);
            _isKilled = true;
            Destroy();
        }

        private void Destroy()
        {
            NotifyObservers();
            Destroy(gameObject);
        }

        #endregion

        #region Observer
        public void NotifyObservers()
        {
            foreach (EnemyDeathObserver obs in _observers.OfType<EnemyDeathObserver>())
                obs.EnemyDestroyNotify(_isKilled);
        }

        public void RegisterObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void UnregisterObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }
        #endregion
    }
}
