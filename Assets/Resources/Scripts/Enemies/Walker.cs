using UnityEngine;
using System.Collections;
using Assets.Resources.Scripts;
using Assets.Scripts.Enemies;
using UnityEditor;

public class Walker : MonoBehaviour
{
    //footsteps.
    public string FootStepsPath = "Footsteps";      //path at Resources/Audio Clips/ where the footstep audio clips are found.
    public float StepInterval = .8f;                //time between footsteps.
    public float MinVolume = .1f, MaxVolume = 1;    //min and max volumes of the footsteps.
    private SoundClipChooser _footSteps;            //Chooses random footsteps.

    //time
    private float _totalTime;                       //total time of the object this is attached to before the object expires.
    private float _elapsedTime;                     //time elapsed since start.

	// Use this for initialization
	void Start ()
	{
        //Get necessary components
	    _totalTime = GetComponent<HitBoxController>().TimeUntilAttack;
	    _footSteps = SoundManager.Instance.GetClipChooser(FootStepsPath);

        //Start spawning foots
        Invoke("SpawnFootStep", StepInterval);
	}

    void Update()
    {
        //update the time.
        _elapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// Spawns a footstep sound.
    /// </summary>
    private void SpawnFootStep()
    {
        //choose clip and spawn.
        AudioClip footstep = _footSteps.GetRandomClip();
        //todo pitch shift.
        AudioSpawn spawn = SoundManager.SpawnAudioSource(footstep, transform.position);

        //adjust volume.
        float age = _elapsedTime / _totalTime;
        spawn.VolumeFactor = Mathf.Lerp(MinVolume, MaxVolume, age);
        
        //invoke next.
        Invoke("SpawnFootStep", StepInterval);
    }
}
