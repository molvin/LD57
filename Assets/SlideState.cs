using UnityEngine;

public class SlideState : State
{
    public float Friction;
    public float Gravity;
    public float GroundCheckDistance = 0.2f;
    public AirState Air;
    public GroundState Ground;

    public override void Enter()
    {
    }

    public override void Tick()
    {
        Owner.Anim.SetBool("Sliding", true);
        HitData groundHit = Owner.GroundCheck(GroundCheckDistance);
        if(groundHit.Hit)
        {
            Owner.Velocity -= Owner.Velocity.normalized * Mathf.Min(Friction * Time.deltaTime, Owner.Velocity.magnitude);
            Owner.Velocity += Vector2.down * Gravity * Time.deltaTime;

            float vertical = Input.GetAxisRaw("Vertical");
            if(vertical > -0.3f)
            {
                Owner.Anim.SetBool("Sliding", false);
                Owner.TransitionTo(Ground);
            }

            if(Ground.Jump())
            {
                Owner.Anim.SetBool("Sliding", false);
            }
        }
        else
        {
            Owner.Anim.SetBool("Sliding", false);
            Owner.TransitionTo(Air);
        }
    }
}
