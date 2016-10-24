using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Resources.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour, IObserveSubject
    {
        public int Health;
        public int MaxHealth = 3;
        public AudioClip[] HealthLevelAudioClips;

        //singleton
        public static PlayerHealth Instance;

        public string DeathAudioPath = "EndSound";
        private AudioClip _deathClip;

        private readonly List<IObserver> _observers = new List<IObserver>();
        void Start()
        {
            _deathClip = SoundManager.Instance.GetRandomClip(DeathAudioPath);
            Restart();
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
            SoundManager.SpawnAudioSource(_deathClip, transform.position);
            NotifyObservers();
        }

        public void Restart()
        {
            Health = MaxHealth;
            UpdateHealthAudio();
        }

        #region Observer
        public void NotifyObservers()
        {
            foreach (IPlayerHealtObserver healtObserver in _observers.OfType<IPlayerHealtObserver>())
                healtObserver.PlayerDeathNotify();
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
