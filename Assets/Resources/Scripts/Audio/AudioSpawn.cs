using UnityEngine;
using System.Collections;

public class AudioSpawn : MonoBehaviour
{
    private AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void SetClip(AudioClip clip)
    {
        //play the clip.
        _source.clip = clip;
        _source.Play();

        //delete this object after we're finished.
        Invoke("Destroy", clip.length);
    }

    /// <summary> 
    /// detroys this game object.
    /// </summary>
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
