using UnityEngine;
using UnityEngine.UIElements;

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
    public bool HasSlidePower => Owner.CurrentAbilities.Contains(Abilities.Slide);
    public AudioEventData AudioEvent;

    private float angleRotate= 0f;
    public override void Enter()
    {
        angleRotate = 0f;
        Owner.Anim.speed = 1f;
        if (PerfectLanding)
        {
            PerfectLanding = false;
            Owner.Velocity += Owner.Velocity.normalized * PerfectLandingBoost;
            // Debug.Log("Perfect landing");
            Owner.particleController.PlayPerfectParticle(Owner.Velocity);
            AudioEvent.Play();

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
                float slopeAngle = Vector2.SignedAngle(Vector2.up, groundHit.Normal) * -Mathf.Sign(rawInput.x);
                angleRotate = Mathf.Lerp(angleRotate, slopeAngle, Time.deltaTime * 10f);
                Owner.Anim.transform.forward = (Vector2.right * Mathf.Sign(input.x) + (Vector2.down * angleRotate / 75f) / 2f).normalized;
            }
            
            Vector2 projection = Vector3.ProjectOnPlane(input, groundHit.Normal).normalized;
            input = projection * input.magnitude;

            float inputAlongVelocity = Vector2.Dot(Owner.Velocity.normalized, input);
            {
                float angle = Vector2.SignedAngle(Vector2.up, groundHit.Normal) * -Mathf.Sign(rawInput.x);
                float maxSpeed = MaxSpeedBySlopeAngle.Evaluate(angle);
                Owner.Anim.SetBool("InputMove", true);
                Owner.Anim.speed = (Owner.Velocity.magnitude / maxSpeed) + 0.5f;
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
            Owner.particleController.PlayJumpParticle(Owner.Velocity);

            return true;
        }
        return false;
    }

    public override void Exit()
    {
    }
}
