using UnityEngine;
using System.Collections;
using System.Security.Permissions;
using Assets.Scripts;
using Assets.Scripts.Enemies;
using UnityEditor;

public class GameManager : MonoBehaviour, IObserver
{
    //singleton
    public static GameManager Instance;

    //other components
    public Player Player;
    private EnemyManager _enemyManager;
	// Use this for initialization
    void Start()
    {
            ApplySingleton();
        RegisterAsObserver();
        GetNecessaryComponents();
        Restart();
    }

    private void ApplySingleton()
    {
        if (Instance == null)
            Instance = this;

        if(Instance != this)
            Destroy(this);
    }

    private void RegisterAsObserver()
    {
        Player.RegisterHealthObserver(this);
    }

    private void GetNecessaryComponents()
    {
        _enemyManager = GetComponentInChildren<EnemyManager>();
    }

    private void Restart()
    {
        Player.Reset();
        _enemyManager.Restart();
        //todo sound
    }

    public void GameOVer()
    {
        foreach (IObserveSubject gameOverInput in Player.SetInActive())
            gameOverInput.RegisterObserver(this);
        _enemyManager.Clear();
        //todo sound
    }

    public void Notify()
    {
        //todo
        Debug.Log("Playr is dead");
    }
}
