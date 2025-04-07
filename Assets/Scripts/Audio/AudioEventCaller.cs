using UnityEngine;

public class AudioEventCaller : MonoBehaviour 
{
    public AudioEventData m_AudioEvent;
    private void Start()
    {
        AudioManager.PlayAudio(m_AudioEvent);
    }

}
