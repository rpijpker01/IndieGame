using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemySoundPlayer : SoundPlayer
{

    public void PlayRandomSpittingSound()
    {
        PlayRandomAudioClip(17, 22);
    }

    public void PlayRandomOofSound()
    {
        PlayRandomAudioClip(7, 11);
    }

    public void PlayRandomOrganicHitSound()
    {
        PlayRandomAudioClip(12, 16);
    }

    public void PlayRandomDeathSound()
    {
        PlayRandomAudioClip(0, 6);
    }
}
