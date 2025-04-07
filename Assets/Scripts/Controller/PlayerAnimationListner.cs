using UnityEngine;

public class PlayerAnimationListner : MonoBehaviour
{
    public AudioEventData m_StepAudio;

    public void Footstep(AnimationEvent animationEvent)
    {
        m_StepAudio.Play();
    }
}
