using UnityEngine;

public class PlayerSpeedFeedback : MonoBehaviour
{
    public AudioEventData m_SpeedAudioData;
    public float m_IsFastFactor = 0.8f;
    
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
    private float GetSpeedFactor()
    {
        if(m_Player.CurrentState == m_GroundState)
        {
            return m_GroundState.MaxSpeed;
        }
        else if(m_Player.CurrentState == m_AirState)
        {
            return m_AirState.MaxSpeed;
        }

        return 0;
    }
    // Update is called once per frame
    void Update()
    {
        float speed = GetSpeedFactor();

        if (!m_IsFast && speed >= m_IsFastFactor)
        {
            OnIsFast();
        }
        else if(m_IsFast && speed < m_IsFastFactor)
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
