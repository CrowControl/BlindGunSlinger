using UnityEngine;
using System.Collections;

public class AudioSpawn : MonoBehaviour
{
    public float MinVolume = 0.2f;
    public float MaxVolume = 1;
    public float MaxVolumeConeAngle = 45;
    public float PanAtMaxAngle = 160;

    public float percentage;
    public float volume;

    private AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    void Update()
    {
        Update3DSettings();
    }

    /// <summary>
    /// Updates all 3d audio values on the audiosource
    /// </summary>
    private void Update3DSettings()
    {
        float angle = Player.Instance.GetAngleWithView(transform.position);
        float absAngle = Mathf.Abs(angle);

        Update3DVolume(absAngle);
        UpdateAudioPanning(angle, absAngle);
    }

    //updates the volume/
    private void Update3DVolume(float absAngle)
    {
        percentage = absAngle / 180;
        volume = Mathf.Lerp(MaxVolume, MinVolume, percentage);
        _source.volume = volume;
    }

    //updates the panning.
    private void UpdateAudioPanning(float angle, float absAngle)
    {
        //calculte panning value.
        float panPercentage = absAngle/(PanAtMaxAngle/100);
        float pan = Mathf.Lerp(0, 1, panPercentage);

        //choose side and apply
        if (angle > 0)
            _source.panStereo = pan;
        else
            _source.panStereo = -pan;
    }



    public void SetClip(AudioClip clip)
    {
        //play the clip.
        _source.clip = clip;
        Update3DSettings();
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
