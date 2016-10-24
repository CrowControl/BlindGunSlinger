using UnityEngine;
using System.Collections;
using Assets.Resources.Scripts.Player;

public class AudioSpawn : MonoBehaviour
{
    public float MinVolume = 0.2f;          //Minimum volume, heard when the playe has back towards this source.
    public float MaxVolume = 1;             //Maximum volume, heard when the player directly faces the source.
    public float MaxVolumeConeAngle = 45;   //angle in front where the maxVolume should occur.
    public float PanAtMaxAngle = 160;       //Angle in the back at which maimum panning should occur.
    
    public float Volume;                    //current volume.
    public float VolumeFactor = 1;          //factor with which to multiply the volume.

    private AudioSource _source;            //the source we're using.

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
        //get the angle with the player.forward.
        float angle = Player.Instance.GetAngleWithView(transform.position);
        float absAngle = Mathf.Abs(angle);

        //update 3d settings.
        Update3DVolume(absAngle);
        UpdateAudioPanning(angle, absAngle);
    }

    //updates the volume/
    private void Update3DVolume(float absAngle)
    {
        //calculate the volume with the angle of the player.forward.
        float percentage = absAngle / 180;
        Volume = Mathf.Lerp(MaxVolume, MinVolume, percentage);
        _source.volume = Volume * VolumeFactor;
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

    /// <summary>
    /// Set's the audiosources clip and plays it after delay
    /// </summary>
    /// <param name="clip">Audio clip to use.</param>
    /// <param name="delay">Time to wait before playing.</param>
    public void SetClipAndPlay(AudioClip clip, float delay = 0)
    {
        _source.clip = clip;
        Invoke("Play", delay);
    }

    private void Play()
    {
        //play the clip.
        Update3DSettings();
        _source.Play();

        //delete this object after we're finished.
        Invoke("Destroy", _source.clip.length);
    }

    /// <summary>
    /// Cancels the audiosource.
    /// </summary>
    public void Cancel()
    {
        _source.Stop();
        CancelInvoke();
        Destroy();
    }

    /// <summary> 
    /// detroys this game object.
    /// </summary>
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
