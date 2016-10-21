using UnityEngine;
using System;
using System.Collections;
using Assets.Scripts;

public class Spawner : MonoBehaviour, IObserver
{
    public GameObject SpawnObject;
    public bool IsFree { get { return _spawnee == null; } }
    private GameObject _spawnee;
    void Start()
    {
        Clear();
    }

    /// <summary>
    /// Spawns the current SpawnObject in the scene.
    /// Will throw an exception if no spawnObject is set.
    /// </summary>
    /// <returns>the newly spawned object.</returns>
    public GameObject Spawn()
    {
        //SpawnObject should be instantiated.
        if (SpawnObject == null)
            throw new Exception("Spawner's Spawnobject not set to an existing prefab");

        //spawn it in the scene, register as observer and return it.
        GameObject obj = Instantiate(SpawnObject, transform.position, transform.rotation) as GameObject;
        obj.GetComponent<IObserveSubject>().RegisterObserver(this);
        _spawnee = obj;
        return obj;
    }

    /// <summary>
    /// Spawns the given prefab, and sets it as this spawners spawnobject for later spawn calls.
    /// </summary>
    /// <param name="prefab">prefab to be spawned.</param>
    /// <returns>the newly spawned object.</returns>
    public GameObject Spawn(GameObject prefab)
    {
        SpawnObject = prefab;
        return Spawn();
    }

    public void Clear()
    {
        _spawnee = null;
    }

    public void Notify()
    {
        Clear();
    }
}
