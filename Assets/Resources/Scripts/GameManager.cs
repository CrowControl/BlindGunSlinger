using UnityEngine;
using System.Collections;
using System.Security.Permissions;
using Assets.Resources.Scripts;
using Assets.Resources.Scripts.Player;
using Assets.Scripts;
using Assets.Scripts.Enemies;
using UnityEditor;

public class GameManager : MonoBehaviour, IObserver, IPlayerHealtObserver
{
    //singleton
    public static GameManager Instance;

    //other components
    public Player Player;
    private EnemyManager _enemyManager;

    public string StartAudioPath = "StartSound";
    private AudioClip _startAudio;

    private float _elapsedTime;
	// Use this for initialization
    void Awake()
    {
        ApplySingleton();
    }

    void Start()
    {
        RegisterAsObserver();
        GetNecessaryComponents();
        GameOver();
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
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
        _startAudio = SoundManager.Instance.GetRandomClip(StartAudioPath);
    }

    private void Restart()
    {
        Player.Reset();
        SoundManager.SpawnAudioSource(_startAudio, transform.position);
        StartGame();    
    }

    private void StartGame()
    {
        _enemyManager.Restart();
        _elapsedTime = 0;
    }

    public void GameOver()
    {
        foreach (IObserveSubject gameOverInput in Player.SetInActive())
            gameOverInput.RegisterObserver(this);
        _enemyManager.Clear();
    }

    public void Notify()
    {
        Restart();
    }

    //called when the player dies.
    public void PlayerDeathNotify()
    {
        Debug.Log("Player is dead");
        int enemiesKilled = _enemyManager.EnemiesKilled;
        float timeSurvived = _elapsedTime;
        //todo tell the player there score.
        GameOver();
    }
}
