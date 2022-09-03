using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _audioMixer;
    [SerializeField] private Sound[] _soundsToLoad;

    // Makes this class usable in the Unity editor
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;

        private AudioSource _source;

        public AudioSource Source
        {
            get { return _source; }
            set { _source = value; }
        }
        
        public void Play()
        {
            Source.Play();
        }
    }

    private Dictionary<string, Sound> _sounds;

    private void Start()
    {
        _sounds = new Dictionary<string, Sound>();
        foreach (var sound in _soundsToLoad)
        {
            // Add the sound first into the dictionary
            _sounds.Add(sound.name, sound);

            // Create new audio source and setup based on the settings
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.outputAudioMixerGroup = _audioMixer;

            _sounds[sound.name].Source = audioSource;
        }
    }

    public void PlaySound(string name)
    {
        _sounds[name].Play();
    }
}
