using UnityEngine;

public class GroundState : State
{
    public AnimationCurve MaxSpeedBySlopeAngle;
    public float Acceleration;
    public float StoppingPower;
    public float FrictionAboveMax;
    public float Gravity = 50.0f;
    public float JumpBoost;
    public float MaxJumpBoost;
    public AirState Air;
    public SlideState Slide;
    public float GroundCheckDistance = 0.2f;
    public bool PerfectLanding;
    public float PerfectLandingBoost;
    public bool HasSlidePower;


    public override void Enter()
    {
        if(PerfectLanding)
        {
            PerfectLanding = false;
            Owner.Velocity += Owner.Velocity.normalized * PerfectLandingBoost;
            Debug.Log("Perfect landing");
            Owner.particleController.PlayPerfectParticle(Owner.Velocity);
            
        }
        Owner.particleController.PlayLandParticle();

    }

    public override void Tick()
    {
        Owner.Anim.SetBool("OnGround", true);
        Owner.Anim.SetFloat("XVelocity", Mathf.Abs(Owner.Velocity.x));
        HitData groundHit = Owner.GroundCheck(GroundCheckDistance);

        if (groundHit.Hit)
        {
            Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            Vector2 input = Vector2.ClampMagnitude(rawInput, 1f);

            if(Mathf.Abs(input.x) > 0.1f)
            {
                Owner.Anim.transform.forward = Vector2.right * Mathf.Sign(input.x);
            }

            Vector2 projection = Vector3.ProjectOnPlane(input, groundHit.Normal).normalized;
            input = projection * input.magnitude;

            float inputAlongVelocity = Vector2.Dot(Owner.Velocity.normalized, input);
            {
                float slopeAngle = Vector2.SignedAngle(Vector2.up, groundHit.Normal) * -Mathf.Sign(rawInput.x);
                float maxSpeed = MaxSpeedBySlopeAngle.Evaluate(slopeAngle);
                Owner.Anim.SetBool("InputMove", true);
                // No Friction here
                if (Owner.Velocity.magnitude < maxSpeed)
                {
                    Owner.Velocity += input * Acceleration * Time.deltaTime;
                }
                else
                {
                    Owner.Velocity -= Owner.Velocity.normalized * FrictionAboveMax * Time.deltaTime;
                }
            }
            if (inputAlongVelocity < 0.0f || Mathf.Abs(rawInput.x) < 0.1f)
            {
                // Friction when input is against or no input
                Owner.Anim.SetBool("InputMove", false);
                Owner.Velocity -= Owner.Velocity.normalized * Mathf.Min(StoppingPower * Time.deltaTime, Owner.Velocity.magnitude);
            }

            Jump();

            float vertical = Input.GetAxisRaw("Vertical");
            if (vertical < -0.7f && HasSlidePower)
            {
                Owner.TransitionTo(Slide);
            }
        }
        else
        {
            Air.Enter();
            Owner.CurrentState = Air;
        }
    }

    public bool Jump()
    {
        if (Input.GetButtonDown("Jump") && Owner.Velocity.y < MaxJumpBoost)
        {
            float jumpSpeed = Mathf.Min(Mathf.Max(Owner.Velocity.y, 0) + JumpBoost, MaxJumpBoost);
            Owner.Velocity.y = jumpSpeed;
            Air.JumpBoost = JumpBoost;
            Air.Jumped = true;
            Owner.TransitionTo(Air);
            return true;
        }
        return false;
    }

    public override void Exit()
    {
    }
}
