using UnityEngine;
using UnityEngine.Windows;

public class BonkState : State
{
    public float StickDuration;
    public float RecoverDuration;
    public float MaxDuration = 3f;
    public GroundState Ground;
    public float Gravity;
    public float MaxAngle;
    public float RecoverFriction;
    public float GroundCheckDistance = 0.2f;
    public HitData HitThatGotMe;
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
        Owner.Anim.transform.forward = -HitThatGotMe.Normal;
        Owner.Anim.transform.position = (Vector3)HitThatGotMe.HitPoint;
    }

    public override void Tick()
    {
        switch(substate)
        {
            case SubState.Stuck:
            {
                Owner.Velocity = Vector2.zero;
                if (Time.time - enterTime > StickDuration)
                {
                    Owner.Anim.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                    substate = SubState.Falling;
                    Owner.Anim.ResetTrigger("Bonk");
                    Owner.Anim.SetTrigger("Falling");
                }
                break;
            }
            case SubState.Falling:
            {
                Owner.Anim.transform.forward = new Vector2(Owner.LastHit.Normal.x, 0F).normalized;
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

                if (Time.time - enterTime > MaxDuration)
                {
                    Owner.Anim.SetTrigger("Bonk-Recovery");
                    Owner.Anim.SetTrigger("LeaveBonk");
                    Owner.TransitionTo(GetComponent<AirState>());
                }
                break;
            }
            case SubState.Recovering:
            {
                Owner.Velocity -= Owner.Velocity.normalized * Mathf.Min(RecoverFriction * Time.deltaTime, Owner.Velocity.magnitude);

                if (Time.time - enterTime > RecoverDuration)
                {
                    Owner.TransitionTo(Ground);
                    Owner.Anim.SetTrigger("LeaveBonk");
                }
            }
            break;
                
        }

    }

    public override void Exit()
    {
    }
}
