using UnityEngine;

public class BonkState : State
{
    public float StickDuration;
    public float RecoverDuration;
    public GroundState Ground;
    public float Gravity;
    public float MaxAngle;
    public float RecoverFriction;
    public float GroundCheckDistance = 0.2f;

    public AudioEventData m_BonkSound;

    private float enterTime;
    private Vector2 enterDir;

    private enum SubState
    {
        Stuck,
        Falling,
        Recovering
    }
    private SubState substate;

    public override void Enter()
    {
        substate = SubState.Stuck;
        enterTime = Time.time;
        Owner.Anim.SetTrigger("Bonk");
        enterDir = Owner.transform.forward;
        Owner.particleController.playBonk();
        m_BonkSound.Play();
    }

    public override void Tick()
    {
        // Owner.Anim.transform.forward = -Owner.LastHit.Normal; 

        switch(substate)
        {
            case SubState.Stuck:
            {
                Owner.Velocity = Vector2.zero;
                if (Time.time - enterTime > StickDuration)
                {
                    substate = SubState.Falling;
                }
                break;
            }
            case SubState.Falling:
            {
                Owner.Velocity += Vector2.down * Gravity * Time.deltaTime;
                HitData hit = Owner.GroundCheck(GroundCheckDistance);
                if(hit.Hit)
                {
                    float angle = Vector2.Angle(Vector2.up, hit.Normal);
                    if(angle < MaxAngle)
                    {
                        substate = SubState.Recovering;
                        enterTime = Time.time;
                        Owner.Anim.SetTrigger("Bonk-Recovery");
                        Owner.particleController.playedBonk();
                    }
                }
                break;
            }
            case SubState.Recovering:
            {
                Owner.Velocity -= Owner.Velocity.normalized * Mathf.Min(RecoverFriction * Time.deltaTime, Owner.Velocity.magnitude);

                if (Time.time - enterTime > RecoverDuration)
                {
                    // Owner.Anim.transform.forward = enterDir;
                    Owner.TransitionTo(Ground);
                }
            }
            break;
                
        }

    }

    public override void Exit()
    {
    }
}
