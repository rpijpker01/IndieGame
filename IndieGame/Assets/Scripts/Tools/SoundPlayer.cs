using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    private int _audioSourceBuffer = 3;
    [SerializeField]
    private List<AudioClip> _audioClips = new List<AudioClip>();
    private AudioSource[] _audioSources;

    // Use this for initialization
    private void Start()
    {
        for (int i = 0; i < _audioSourceBuffer; i++)
        {
            gameObject.AddComponent<AudioSource>();
        }
        _audioSources = GetComponents<AudioSource>();
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.loop = false;
        }
    }

    public void PlayAudioClip(int clipIndex)
    {
        PlayClip(clipIndex);
    }

    public void PlayRandomAudioClip(int range)
    {
        int randomIndex = Random.Range(0, range - 1);
        PlayClip(randomIndex);
    }

    public void PlayRandomAudioClip(int startIndex, int endIndex)
    {
        int randomIndex = Random.Range(startIndex, endIndex);
        PlayClip(randomIndex);
    }


    private void PlayClip(int clipIndex)
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.isPlaying == false)
            {
                audioSource.clip = _audioClips[clipIndex];
                audioSource.Play();
                return;
            }
        }
        _audioSources[0].clip = _audioClips[clipIndex];
        _audioSources[0].Play();
    }
}
