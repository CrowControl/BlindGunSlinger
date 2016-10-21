using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public GameObject AudioSourcePrefab;
        public static SoundManager Instance;
        //holds all clip choosers.
        private readonly Dictionary<string, SoundClipChooser> _clipCollections = new Dictionary<string, SoundClipChooser>();

        private const string PathBase = "Audio Clips/";
        private const string SourcePrefabPath = "Prefabs/Audio";
        // Use this for initialization
        void Awake()
        {
            ApplySingleton();
        }

        /// <summary>
        /// applies the singleton pattern to this object.
        /// </summary>
        private void ApplySingleton()
        {
            if (Instance == null)
                Instance = this;

            if (Instance != this)
                Destroy(gameObject);
        }

        /// <summary>
        /// Get a random clip from path
        /// </summary>
        /// <param name="path">path in resources/Audio Clips/ where we need to get a clip from</param>
        /// <returns>random clip from path.</returns>
        public AudioClip GetRandomClip(string path)
        {
            CheckSoundChooser(path);
            return _clipCollections[path].GetRandomClip();
        }

        /// <summary>
        /// Returns random clips from the path.
        /// </summary>
        /// <param name="path">path in the resources/Audio Clips/ folder from where the clips must come.</param>
        /// <param name="amount">Amount of clips to return.</param>
        /// <returns></returns>
        public AudioClip[] GetRandomClips(string path, int amount)
        {
            //Choose as many clips as we need.
            AudioClip[] clips = new AudioClip[amount];
            for (int i = 0; i < amount; i++)
                clips[i] = GetRandomClip(path);

            return clips;
        }

        /// <summary>
        /// gets all clips at path
        /// </summary>
        /// <param name="path">path to get clips from</param>
        /// <returns>all clips found at path.s</returns>
        public AudioClip[] GetAllClips(string path)
        {
            CheckSoundChooser(path);
            return _clipCollections[path].GetAllClips();
        }

        /// <summary>
        /// checks if the soundchooser for the given path exists yet. Creates it if not.
        /// </summary>
        private void CheckSoundChooser(string path)
        {
            if (!_clipCollections.ContainsKey(path))
                _clipCollections[path] = new SoundClipChooser(PathBase + path);
        }

        public static void SpawnAudioSource(AudioClip clip, Vector3 pos)
        {
            GameObject prefab = Instance.AudioSourcePrefab;
            //spawn the audiosource prefab and pass it the clip.
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
            obj.GetComponent<AudioSpawn>().SetClip(clip);
        }
    }

    /// <summary>
    /// Class to encapsulate choosing random clips.
    /// </summary>
    public class SoundClipChooser
    {
        private readonly AudioClip[] _clips;    //the clips
        private int _lastIndex;                 //Index of the last clip we returned.
        public SoundClipChooser(AudioClip[] clips)
        {
            _clips = clips;
        }

        public SoundClipChooser(string path)
        {
            _clips = UnityEngine.Resources.LoadAll<AudioClip>(path);
        }

        /// <summary>
        /// returns a random clip from this choosers' collection.
        /// </summary>
        /// <returns>A random clip from this choosers'collections</returns>
        public AudioClip GetRandomClip()
        {
            //choose a clip, not same as last.
            int index = _lastIndex;
            while (index == _lastIndex)
                index = Random.Range(0, _clips.Length);

            //save choice for next time, and return.
            _lastIndex = index;
            return _clips[index];
        }

        public AudioClip[] GetAllClips()
        {
            return _clips;
        }
    }
}