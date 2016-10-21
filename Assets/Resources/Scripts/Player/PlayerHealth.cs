using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Resources.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour, IObserveSubject
    {
        public int Health;
        public AudioClip[] HealthLevelAudioClips;

        //singleton
        public static PlayerHealth Instance;
        private int _maxHealth;

        private readonly List<IObserver> _observers = new List<IObserver>();
        void Start()
        {
            _maxHealth = Health;
        }

        /// <summary>
        /// Apply damage to the player
        /// </summary>
        /// <param name="damage">amount of damage to be applied.</param>
        public void ApplyDamage(int damage)
        {
            //apply damage
            Health -= damage;

            //we have audio that changes with health, so we ant to update that.
            UpdateHealthAudio();

            //if we reach 0 health, we die.
            if (Health <= 0)
                Die();
        }

        /// <summary>
        /// update the currently playing.
        /// </summary>
        private void UpdateHealthAudio()
        {
            //todo
        }

        private void Die()
        {
            //todo play dying sound
            NotifyObservers();
        }

        public void Restart()
        {
            Health = _maxHealth;
            UpdateHealthAudio();
        }

        #region Observer
        public void NotifyObservers()
        {
            foreach(IObserver observer in _observers)
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
