using UnityEngine;

public class PlayerSpeedFeedback : MonoBehaviour
{
    public AudioEventData m_SpeedAudioData;
    public float m_IsFastFactor = 0.8f;
    public float m_GuessedMaxSpeed = 25f;
    private bool m_IsFast;
    private GroundState m_GroundState;
    private AirState m_AirState;
    private Player m_Player;
    private AudioSource m_LoopingAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_GroundState = GetComponent<GroundState>();
        m_AirState = GetComponent<AirState>();
        m_Player = GetComponent<Player>();
    }
    private bool IsInAValidState()
    {
        return m_Player.CurrentState == m_GroundState || m_Player.CurrentState == m_AirState;
    }
    // Update is called once per frame
    void Update()
    {
        float speedFactor = m_Player.Velocity.magnitude / m_GuessedMaxSpeed;

        if (!m_IsFast && speedFactor >= m_IsFastFactor)
        {
            OnIsFast();
        }
        else if(m_IsFast && speedFactor < m_IsFastFactor)
        {
            OnToSlow();
        }
    }

    private void OnToSlow()
    {
        m_SpeedAudioData.Stop(m_LoopingAudioSource);
        m_IsFast = false;
    }
    private void OnIsFast()
    {
        m_LoopingAudioSource = m_SpeedAudioData.Play();
        m_IsFast = true;
    }

    private void OnDestroy()
    {
        if(m_LoopingAudioSource)
            m_SpeedAudioData.Stop(m_LoopingAudioSource);
    }
}
