using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurfaceAudio
{
    public string tag; // Tag to identify the type of surface
    public AudioClip[] walkingSounds; // Array of walking sound effects
    public AudioClip[] landingSounds; // Array of landing sound effects
    public AudioClip[] jumpingSounds; // Array of jumping sound effects
    public float pitchRange = 0.2f; // The range in which the pitch of the sound effects can vary
    public float cooldown = 0.2f; // Cooldown between sounds
}

