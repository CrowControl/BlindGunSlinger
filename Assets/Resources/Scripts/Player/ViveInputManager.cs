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
        //states.
        private PlayingViveController _playingController;
        private GameOverViveController _gameOverController;

        //active state
        private ViveController _activeController;

        void Awake()
        {
            //get components.
            Shooter shooter = GetComponentInParent<Shooter>();
            SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();
            
            //initialize states.
            _playingController = new PlayingViveController(trackedObj, shooter, transform);
            _gameOverController = new GameOverViveController(trackedObj);

            //set current state.
            _activeController = _playingController;
        }

        void Update()
        {
            //read input of current state.
            _activeController.ReadInput();
        }

        void FixedUpdate()
        {
            //apply input of current state.
            _activeController.ApplyInput();
        }


        /// <summary>
        /// Set state to Playing.
        /// </summary>
        public void SetPlaying()
        {
            _activeController = _playingController;
        }

        /// <summary>
        /// Set state to non-playing.
        /// </summary>
        /// <returns>Game over input controller, to be observed by game manager.</returns>
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

        /// <summary>
        /// Read all necessary input.
        /// </summary>
        public void ReadInput()
        {
            //controller can not be initialized when it leaves the Vive Tower area.
            IsTriggerDown = ControllerInitialized && Controller.GetPressDown(TriggerButtonId);
        }

        /// <summary>
        /// Apply all input.
        /// </summary>
        public abstract void ApplyInput();
    }

    public class PlayingViveController : ViveController
    {
        //components.
        private readonly Shooter _shooter;
        private readonly Transform _transform;

        public PlayingViveController(SteamVR_TrackedObject obj, Shooter shooter, Transform transform) : base(obj)
        {
            _shooter = shooter;
            _transform = transform;
        }

        /// <summary>
        /// apply all input.
        /// </summary>
        public override void ApplyInput()
        {
            if (IsTriggerDown)
                Shoot();
        }

        /// <summary>
        /// Shoot your gun.
        /// </summary>
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
            //empty constructor, only used to pass parameters to the base class.
        }

        /// <summary>
        /// Apply all input.
        /// </summary>
        public override void ApplyInput()
        {
            //if the trigger is pulled, we let the observers know.
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
