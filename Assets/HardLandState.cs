using UnityEngine;

public class HardLandState : State
{
    public float Duration;
    public GroundState Ground;
    public float Friction;
    public AudioEventData BadLandingSound;

    private float enterTime;


    public override void Enter()
    {
        enterTime = Time.time;
        Owner.Anim.SetBool("HardLand", true);
        BadLandingSound?.Play();
    }

    public override void Exit()
    {
        
    }

    public override void Tick()
    {
        Owner.Velocity -= Owner.Velocity.normalized * Mathf.Min(Friction * Time.deltaTime, Owner.Velocity.magnitude);

        if (Time.time - enterTime > Duration)
        {
            Owner.TransitionTo(Ground);
            Owner.Anim.SetBool("HardLand", false);
        }
    }
}
