using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using Valve.VR;

namespace Assets.Resources.Scripts.Player.Input
{
    public class ViveInputManager : MonoBehaviour, IPlayerInputManager
    {
        private PlayingViveController _playingController;
        private GameOverViveController _gameOverController;

        private ViveController _activeController;

        void Awake()
        {
            Shooter shooter = GetComponentInParent<Shooter>();
            SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();
            
            _playingController = new PlayingViveController(trackedObj, shooter, transform);
            _gameOverController = new GameOverViveController(trackedObj);

            _activeController = _playingController;
        }

        void Update()
        {
            _activeController.ReadInput();
        }

        void FixedUpdate()
        {
            _activeController.ApplyInput();
        }

       
        public void SetPlaying()
        {
            _activeController = _playingController;
        }

        public IObserveSubject SetNonPlaying()
        {
            _activeController = _gameOverController;
            return _gameOverController;
        }
    }

    public abstract class ViveController : IPlayerInputController
    {
        //The index of the controller can change if it leaves and reenters the tower view, so we need to keep asking.
        private SteamVR_Controller.Device Controller
        {
            get
            {
                int index = (int) _trackedObj.index;
                SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
                return device;
            }
        }

        private bool ControllerInitialized
        {
            get { return Controller != null; }
        }
        //input data.   
        protected bool IsTriggerDown;

        //other components.
        private readonly SteamVR_TrackedObject _trackedObj;

        //constants.
        private const EVRButtonId TriggerButtonId = EVRButtonId.k_EButton_SteamVR_Trigger;

        protected ViveController(SteamVR_TrackedObject obj)
        {
            _trackedObj = obj;
        }

        public void ReadInput()
        {
            bool initialized = ControllerInitialized;
            IsTriggerDown = ControllerInitialized && Controller.GetPressDown(TriggerButtonId);
        }

        public abstract void ApplyInput();
    }

    public class EmptyController : ViveController
    {
        public EmptyController(SteamVR_TrackedObject obj) : base(obj)
        {
        }

        public override void ApplyInput()
        {
            
        }
    }

    public class PlayingViveController : ViveController
    {
        private readonly Shooter _shooter;
        private readonly Transform _transform;
        public PlayingViveController(SteamVR_TrackedObject obj, Shooter shooter, Transform transform) : base(obj)
        {
            _shooter = shooter;
            _transform = transform;
        }

        public override void ApplyInput()
        {
            if (IsTriggerDown)
                Shoot();
        }

        private void Shoot()
        {
            Debug.Log("Shot is fired.");
            //Create a ray out of the camera.
            Ray ray = new Ray(_transform.position, _transform.forward);
            _shooter.Shoot(ray);
        }
    }

    public class GameOverViveController : ViveController, IObserveSubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>(); 
        public GameOverViveController(SteamVR_TrackedObject obj) : base(obj)
        {
        }

        public override void ApplyInput()
        {
            if(IsTriggerDown)
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
