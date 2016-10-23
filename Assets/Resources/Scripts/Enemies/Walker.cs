using UnityEngine;
using System.Collections;
using Assets.Resources.Scripts;
using Assets.Scripts.Enemies;
using UnityEditor;

public class Walker : MonoBehaviour
{
    public string FootStepsPath = "Footsteps";
    public float StepInterval = .8f;
    public float MinVolume = .1f, MaxVolume = 1;
    private SoundClipChooser _footSteps;

    private float _totalTime;
    private float _elapsedTime;
	// Use this for initialization
	void Start ()
	{
	    _totalTime = GetComponent<HitBoxController>().TimeUntilAttack;
	    _footSteps = SoundManager.Instance.GetClipChooser(FootStepsPath);
        Invoke("SpawnFootStep", StepInterval);
	}

    void Update()
    {
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
