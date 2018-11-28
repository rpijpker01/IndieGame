using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> _audioClips = new List<AudioClip>();
    private AudioSource _audioSource;

    // Use this for initialization
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
    }

    public void PlayAudioClip(int clipIndex)
    {
        _audioSource.clip = _audioClips[clipIndex];
        _audioSource.Play();
    }
}
