using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundPlayer : SoundPlayer
{

    public void PlayRandomAttackSound()
    {
        PlayAudioClip(6);
    }

    public void PlayRandomOofSound()
    {
        PlayRandomAudioClip(7, 12);
    }

    public void PlayRandomStoneHitSound()
    {
        PlayRandomAudioClip(13, 16);
    }

    public void PlayRandomDeathSound()
    {
        PlayRandomAudioClip(0, 5);
    }
}
