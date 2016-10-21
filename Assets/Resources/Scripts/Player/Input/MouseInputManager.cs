using System.Collections.Generic;
using Assets.Resources.Scripts.Player.Input;
using Assets.Scripts.Enemies;
using UnityEngine;
using UnityEngine.VR;

namespace Assets.Scripts
{
    public class MouseInputManager : MonoBehaviour, IPlayerInputManager
    { 
        public static MouseInputManager Instance;

        private IPlayerInputController[] _inputControllers;
        private IPlayerInputController _activeInputController;

        void Start()
        {
            InitializeInputControllers();
            SetPlaying();
            SetVrSettings();
        }

        private void InitializeInputControllers()
        {
            Camera cam = GetComponentInChildren<Camera>();
            Shooter shooter = GetComponent<Shooter>();

            _inputControllers = new IPlayerInputController[2];
            _inputControllers[0] = new PlayingInputController(transform, cam, shooter);
            _inputControllers[1] = new GameOverInputController();
        }

        protected virtual void SetVrSettings()
        {
            VRSettings.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            _activeInputController.ReadInput();
        }

        void FixedUpdate()
        {
            _activeInputController.ApplyInput();
        }

        public void SetPlaying()
        {
            _activeInputController = _inputControllers[0];
        }

        public IObserveSubject SetNonPlaying()
        {
            GameOverInputController gameOver = _inputControllers[1] as GameOverInputController;
            _activeInputController = gameOver;
            return gameOver;
        }
    }

    public class PlayingInputController : IPlayerInputController
    {
        private IPlayerInputController _nextState;

        private float _rotateX;
        private bool _mousePressed;

        //components
        private readonly Transform _transform;
        private readonly Camera _camera;
        private readonly Shooter _shooter;

        public PlayingInputController(Transform transform, Camera camera, Shooter shooter)
        {
            _transform = transform;
            _camera = camera;
            _shooter = shooter;

        }

        #region Read input
        /// <summary>
        /// Reads all player input
        /// </summary>
        public virtual void ReadInput()
        {
            ReadMoveInput();
            ReadShootInput();
        }

        private void ReadMoveInput()
        {
            //read rotation input
            _rotateX += Input.GetAxis("Mouse X");
        }

        private void ReadShootInput()
        {
            _mousePressed = Input.GetMouseButtonDown(0);
        }
        #endregion


        #region Apply input
        public void ApplyInput()
        {
            RotatePlayer();

            if (_mousePressed)
                Shoot();
        }

        public void SetNextState(IPlayerInputController nextState)
        {
            _nextState = nextState;
        }

        public IPlayerInputController GetNextState()
        {
            return _nextState;
        }

        /// <summary>
        /// Applies the rotation from the input.
        /// </summary>
        protected virtual void RotatePlayer()
        {
            _transform.eulerAngles = new Vector3(0, _rotateX, 0);
        }

        protected virtual void Shoot()
        {
            Debug.Log("Shot is fired.");
            //Create a ray out of the camera.
            Transform camTransform = _camera.transform;
            Ray ray = new Ray(camTransform.position, camTransform.forward);
            //shoot it.
            _shooter.Shoot(ray);
        }
        #endregion
    }

    public class GameOverInputController : IPlayerInputController, IObserveSubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();

        private bool _mousePressed;
        public void ReadInput()
        {
            _mousePressed = Input.GetMouseButtonDown(0);
        }

        public void ApplyInput()
        {
            if(_mousePressed)
                NotifyObservers();
        }

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
    }

}