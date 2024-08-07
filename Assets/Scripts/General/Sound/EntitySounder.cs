using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General.Sound
{
    public class EntitySounder : MonoBehaviour
    {
        [SerializeField] private Sound[] sounds;

        [SerializeField] private AudioSource[] _distanceBasedAudioSources;

        private void Start()
        {
            int i = 0;
            foreach (var sound in sounds)
            {
                sound.config.source = _distanceBasedAudioSources[i];
                sound.config.source.clip = sound.config.clip;
                sound.config.source.volume = sound.config.volume;
                sound.config.source.pitch = sound.config.pitch;
                sound.config.source.loop = sound.config.loop;
                if (sound.config.mixerGroup.Equals(AudioMixerGroup.FX))
                    sound.config.source.outputAudioMixerGroup =
                        AudioManager.Instance.AudioMixer.FindMatchingGroups("FX")[0];
                else if (sound.config.mixerGroup.Equals(AudioMixerGroup.MUSIC))
                    sound.config.source.outputAudioMixerGroup =
                        AudioManager.Instance.AudioMixer.FindMatchingGroups("MUSIC")[0];
                else if (sound.config.mixerGroup.Equals(AudioMixerGroup.UI))
                    sound.config.source.outputAudioMixerGroup =
                        AudioManager.Instance.AudioMixer.FindMatchingGroups("UI")[0];
                i++;
            }
        }


        public void StartSounding()
        {
            float rndTime = Random.Range(5f, 25f);

            StartCoroutine(WaitToPlay(rndTime));
        }

        private IEnumerator WaitToPlay(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            PlayAughSound();
            StartSounding();
        }

        public void PlayLocalOneShot(string name)
        {
            var s = Array.Find(sounds, sound => sound.name == name);
            s.config.source.PlayOneShot(s.config.clip);
        }

        public void StopSounding()
        {
            StopCoroutine(WaitToPlay(0));
        }

        public void PlayDeadSound()
        {
            PlayLocalOneShot("ZombieDie");
        }

        public void PlayAttackSound()
        {
            int id = Random.Range(sounds.Length - 2, sounds.Length);
            PlayLocalOneShot(sounds[id].name);
        }

        private bool IsPlaying(string name)
        {
            var s = Array.Find(sounds, sound => sound.name == name);
            return s.config.source.isPlaying;
        }


        public void PlayDistanceCloseSound()
        {
            if (IsPlaying("Zombie1") || IsPlaying("Zombie2") || IsPlaying("Zombie3") || IsPlaying("Zombie4") ||
                IsPlaying("Zombie5") || IsPlaying("Zombie6") || IsPlaying("Zombie7") || IsPlaying("Zombie8") ||
                IsPlaying("Zombie9")) return;
            PlayAughSound();
        }

        public void PlayAughSound()
        {
            int id = Random.Range(0, sounds.Length - 2);
            PlayLocalOneShot(sounds[id].name);
        }
    }
}