using UnityEngine;

public class AirState : State
{
    public float Gravity = 10.0f;
    public float SlowFallGravity = 7.0f;
    public float MaxSpeed;
    public float AccelerationMaxSpeed;
    public float AirResistance;
    public float FastFallBoost;
    public float HardLandVelocityMin;
    public float GroundCheckDistance = 0.2f;

    public bool Jumped;
    private bool fastFalling;
    internal float JumpBoost;
    public float PerfectLandingFactor;
    public float PerfectLandingMinSpeed;
    public float PerfectLandingMinFallSpeed;
    public float PerfectLandingMinAirTime;

    public float Acceleration;

    public GroundState Ground;

    public AudioEventData m_DubbleJumpAudio;

    public bool HasDoubleJump => Owner.CurrentAbilities.Contains(Abilities.DoubleJump);

    private bool canJump;

    public float EnterTime;

    public override void Enter()
    {
        Owner.Anim.speed = 1f;
        Owner.Anim.gameObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        EnterTime = Time.time;
        fastFalling = false;
        canJump = Jumped && HasDoubleJump;
    }

    public override void Tick()
    {
        Owner.Anim.SetBool("OnGround", false);
        float currentPlaceInAnimation = (Owner.Velocity.y / Ground.JumpBoost)*2f - 1f;
        Owner.Anim.Play("Jump", -1, currentPlaceInAnimation);
       
        // Gravity
        float gravity = Gravity;
        if (Jumped)
        {
            gravity = Mathf.Lerp(Gravity, SlowFallGravity, Mathf.Min(Owner.Velocity.y, JumpBoost) / JumpBoost);
            if(!Input.GetButton("Jump") || Owner.Velocity.y < 0.0f)
            {
                Jumped = false;
            }
        }
        Owner.Velocity += Vector2.down * gravity * Time.deltaTime;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        Vector2 veloDelta = input * Acceleration * Time.deltaTime;
        float d = Vector2.Dot(veloDelta.normalized, Owner.Velocity.normalized);
        if(d >= 0)
        {
            if((Owner.Velocity + veloDelta).magnitude < AccelerationMaxSpeed)
            {
                Owner.Velocity += veloDelta;
            }
        }
        else
        {
            Owner.Velocity += veloDelta;
        }

        // Air Resistance
        if (Owner.Velocity.magnitude < MaxSpeed)
        {
            
        }
        else
        {
            Owner.Velocity -= Owner.Velocity.normalized * AirResistance * Time.deltaTime;
        }

        Owner.Anim.transform.forward = Vector2.right * Mathf.Sign(Owner.Velocity.x);

        float vertical = Input.GetAxisRaw("Vertical");
        if (!fastFalling && vertical < -0.7f && Owner.Velocity.y < 0f && Ground.HasSlidePower)
        {
            fastFalling = true;
            Owner.Velocity.x /= 2f;
            Owner.Velocity += Vector2.down * FastFallBoost;
        }

        if(canJump && !fastFalling)
        {
            if (Input.GetButtonDown("Jump") && Owner.Velocity.y < Ground.MaxJumpBoost)
            {
                 Owner.particleController.PlayJumpParticle(Owner.Velocity);
                float jumpSpeed = Mathf.Min(Mathf.Max(Owner.Velocity.y, 0) + JumpBoost, Ground.MaxJumpBoost);
                Owner.Velocity.y = jumpSpeed;
                canJump = false;
                m_DubbleJumpAudio.Play();
            }
        }

        if(!Jumped)
        {
            Ground.Jump();
        }
    }

    public override void Exit()
    {
    }
}
