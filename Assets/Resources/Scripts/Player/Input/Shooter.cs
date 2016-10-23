using Assets.Scripts.Enemies;
using UnityEngine;

namespace Assets.Resources.Scripts.Player.Input
{
    public class Shooter : MonoBehaviour
    {
        private SoundClipChooser _clipChooser;
        private const string RevolverPath = "Revolver Shots";
        void Start()
        {
            //load sounds.
            _clipChooser = SoundManager.Instance.GetClipChooser(RevolverPath);
        }

        public void Shoot(Ray ray)
        {
            //play sound
            PlayShotSound();

            //Cast the ray, it should hit something.
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) return;

            //the collider the ray hit should be an enemy.
            Collider other = hit.collider;
            if (other.tag != "Enemy") return;
            
            //let the enemy know it got hit.
            HitBoxController enemy = other.GetComponent<HitBoxController>();
            enemy.GetHit();
        }

        /// <summary>
        /// Plays shooting sound.
        /// </summary>
        private void PlayShotSound()
        {
            //choose clip, het position
            AudioClip clip = _clipChooser.GetRandomClip();
            Vector3 pos = transform.position;

            //spawn audiosource at pos with clip.
            SoundManager.SpawnAudioSource(clip, pos);
        }
    }
}
