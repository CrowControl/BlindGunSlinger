using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Scripts.Player.Input;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Resources.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        //singleton
        public static Player Instance;

        //components
        private PlayerHealth _health;

        public List<IPlayerInputManager> PlayerInput
        {
            get { return _playerInput ?? (_playerInput = GetComponentsInChildren<IPlayerInputManager>().ToList()); }
            set { _playerInput = value; } 
        }

        private List<IPlayerInputManager> _playerInput;
        private Camera _camera;

        void Awake()
        {
            ApplySingleton();
            _health = GetComponentInChildren<PlayerHealth>();
            PlayerInput = GetComponentsInChildren<IPlayerInputManager>().ToList();
            _camera = GetComponentInChildren<Camera>();
        }

        /// <summary>
        /// apply singleton pattern to ensure there is only 1 instance of this object.
        /// </summary
        private void ApplySingleton()
        {
            if (Instance == null)
                Instance = this;

            if (Instance != this)
                Destroy(gameObject);
        }

        public float GetAngleWithView(Vector3 position)
        {
            Transform camTrans = _camera.transform;

            Vector3 camPos = camTrans.position;
            Vector3 toPoint = position - camPos;

            Vector3 camForward = camTrans.forward;
            return Vector3.Angle(camForward, toPoint);
        }

        /// <summary>
        /// apply damage to the player
        /// </summary>
        /// <param name="damage">amount of damage</param>
        public void ApplyDamage(int damage)
        {
            _health.ApplyDamage(damage);
        }

        /// <summary>
        /// reset the player
        /// </summary>
        public void Reset()
        {
            _health.Restart();
            SetActive();
        }

        /// <summary>
        /// Set the player to active.
        /// </summary>
        public void SetActive()
        {
            foreach (IPlayerInputManager input in PlayerInput)
                input.SetPlaying();
        }
    
        /// <summary>
        /// set the player to inactive
        /// </summary>
        /// <returns>controller observe subject so the game manager knows when button is pressed to begin again.</returns>
        public IObserveSubject[] SetInActive()
        {
            int length = PlayerInput.Count;

            //set to non playing, and retreive observe subjects for the gamemanager.
            IObserveSubject[] subjects = new IObserveSubject[length];
            for(int i = 0; i < length; i++)
                subjects[i] = PlayerInput[i].SetNonPlaying();

            return subjects;
        }

        /// <summary>
        /// register asobserver of the player's health.
        /// </summary>
        /// <param name="observer">the observer that wants to register.</param>
        public void RegisterHealthObserver(IObserver observer)
        {
            _health.RegisterObserver(observer);
        }
    }
}
