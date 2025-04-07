using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioEvent_", menuName = "Scriptable Objects/AudioEventData")]
public class AudioEventData : ScriptableObject
{
    public List<AudioClip> m_Clips;
    [Range(0, 1)]
    public float m_PitchRange;
    [Range(0, 1)]
    public float m_AmplitudeRange;
    public bool m_IsLooping;
    public bool m_DontDestroyOnLoad;
}
