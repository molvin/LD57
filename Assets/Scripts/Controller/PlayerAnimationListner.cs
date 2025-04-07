using UnityEngine;

public class PlayerAnimationListner : MonoBehaviour
{
    public AudioEventData m_StepAudio;

    public void Footstep()
    {
        m_StepAudio.Play();
    }
}
