using System.Collections.Generic;
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

        //damage
        public int Damage = 1;

        //time until attack
        public float TimeUntilAttack = 3;
        private float _elapsedTime;

        //hit grunts
        private AudioClip[] _hitGrunts;
        private int _curGruntIndex;

        //spawn and attack.
        private AudioClip _spawnAudioClip;
        private AudioClip _attackAudioClip;

        //other components
        private Player _player;

        //observer
        private List<IObserver> _observers;

        #region Awake
        void Awake()
        {
            _observers = new List<IObserver>();
            _player = Player.Instance;
            InitializeSoundClips();
            PlaySpawnClip();
        }

        /// <summary>
        /// Initializes all the soundclips for this object.
        /// </summary>
        protected virtual void InitializeSoundClips()
        {
            SoundManager sound = SoundManager.Instance;

            _spawnAudioClip = sound.GetRandomClip("Shotgun Clicks");
            _hitGrunts = sound.GetRandomClips("Enemy Grunts", HealthPoints);
            _attackAudioClip = sound.GetRandomClip("Shotgun Shots");
        }

        /// <summary>
        /// PLays the spawn audio
        /// </summary>
        private void PlaySpawnClip()
        {
            SoundManager.SpawnAudioSource(_spawnAudioClip, transform.position);
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
            SoundManager.SpawnAudioSource(_attackAudioClip, transform.position);

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
            //play a hit sound.
            PlayHitGrunt();
        
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
            SoundManager.SpawnAudioSource(grunt, transform.position);

            //set index for next time.
            _curGruntIndex++;
        }

        protected virtual void Die()
        {
            Debug.Log("Enemy Died");
            //todo dying sound.
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
            foreach (IObserver observer in _observers)
                observer.Notify();
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
