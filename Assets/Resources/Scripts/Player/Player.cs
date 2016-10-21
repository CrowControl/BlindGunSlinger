using UnityEngine;
using System.Collections;
using Assets.Resources.Scripts.Player;
using Assets.Scripts;
using Assets.Resources.Scripts.Player.Input;

public class Player : MonoBehaviour
{
    //singleton
    public static Player Instance;

    //components
    private PlayerHealth health;
    private IPlayerInputManager[] _playerInput;

    void Awake()
    {
        ApplySingleton();
        health = GetComponentInChildren<PlayerHealth>();
        _playerInput = GetComponentsInChildren<IPlayerInputManager>();
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

    /// <summary>
    /// apply damage to the player
    /// </summary>
    /// <param name="damage">amount of damage</param>
    public void ApplyDamage(int damage)
    {
        health.ApplyDamage(damage);
    }

    /// <summary>
    /// reset the player
    /// </summary>
    public void Reset()
    {
        health.Restart();
        SetActive();
    }

    /// <summary>
    /// Set the player to active.
    /// </summary>
    public void SetActive()
    {
        foreach (IPlayerInputManager input in _playerInput)
            input.SetPlaying();
    }
    
    /// <summary>
    /// set the player to inactive
    /// </summary>
    /// <returns>controller observe subject so the game manager knows when button is pressed to begin again.</returns>
    public IObserveSubject[] SetInActive()
    {
        int length = _playerInput.Length;

        //set to non playing, and retreive observe subjects for the gamemanager.
        IObserveSubject[] subjects = new IObserveSubject[length];
        for(int i = 0; i < length; i++)
            subjects[i] = _playerInput[i].SetNonPlaying();

        return subjects;
    }

    /// <summary>
    /// register asobserver of the player's health.
    /// </summary>
    /// <param name="observer">the observer that wants to register.</param>
    public void RegisterHealthObserver(IObserver observer)
    {
        health.RegisterObserver(observer);
    }
}
