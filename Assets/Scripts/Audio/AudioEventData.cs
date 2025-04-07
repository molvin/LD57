using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioEvent_", menuName = "Scriptable Objects/AudioEventData")]
public class AudioEventData : ScriptableObject
{
    public List<AudioClip> m_Clips;
    [Range(0, 1)]
    public float m_PitchRange;
    [Range(0, 1)]
    public float m_Volume = 1.0f;
    [Range(0, 1)]
    public float m_AmplitudeRange;
    public AudioMixerGroup m_MixerGroup;

    public bool m_IsLooping;
    public bool m_DontDestroyOnLoad;

    public float m_FadeInAndOutTime = -1;
    public void Stop(AudioSource audioSource)
    {
        AudioManager.StopAudio(audioSource, this);
    }
    public AudioSource Play(Transform attachPoint = null)
    {
        return AudioManager.PlayAudio(this, attachPoint);
    }
}
