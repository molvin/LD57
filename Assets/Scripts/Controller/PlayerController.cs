using UnityEngine;
using UnityEngine.Assertions;

public enum PlayerState
{
    Ground,
    Air,
    Jump,
}

public class PlayerController : MonoBehaviour
{
    [Header("Collision Settings")]
    public LayerMask CollisionLayer;
    public float CollisionRadius = 0.5f;

    public float Speed = 10f;
    public float SafetyMargin = 0.02f;
    public float Gravity = 22f;
    public float StaticFrictionCoefficient = 0.85f;
    public float FrictionCoefficient = 0.4f;
    public float LandingCoefficient = 0.3f;
    public float GroundedGravity = 0.5f;
    public float Jump = 10f;
    public float JumpHoldBoost = 20f;
    public float FastFall = 5f;
    public int GodFrames = 4;
    public float JumpTime = 1f;
    public float Deceleration = 0.5f;
    public float AirControl = 0.2f;
    public float InputFriction = 0.3f;
    public float walkableAngle = 70;
    [HideInInspector] public Vector2 velocity;
    private Vector2 groundNormal;
    private float distanceToGround = 0f;
    private Vector2 ceilingNormal;
    private float distanceToCeiling = 0f;

    public float groundCheckDist = 0.2f;

    private new CircleCollider2D collider;

    private Vector2 inputCache;
    private int godFrameCount = 0;
    private float jumpTimer = 0f;

    private PlayerState currentState = PlayerState.Ground;

    public int GrantedItems = 0;

    // Environment Query
    bool wasGrounded;
    bool isGrounded;

    private void Start()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    bool CanJump()
    {
        bool press = Input.GetButtonDown("Jump");
        bool god = godFrameCount > 0;
        bool hasSpace = distanceToCeiling > CollisionRadius * 0.5f;

        return press && god && hasSpace;
    }

    // TODO: Move to fixed update for server logic to work
    private void Update()
    {
        StateUpdate(Time.deltaTime);

        GroundCheck();
        AboveCheck();

        Transition();

        PreventCollision(Time.deltaTime);
        velocity = Vector2.ClampMagnitude(velocity, Speed * 10f);
        transform.position += (Vector3)velocity * Time.deltaTime;

        Collider2D overlap = Physics2D.OverlapCircle((Vector2)transform.position, 0.5f, CollisionLayer);
        if (overlap != null)
        {
            ColliderDistance2D dist = Physics2D.Distance(collider, overlap);
            if (dist.isValid)
            {
                groundNormal = dist.normal;
                transform.position += (Vector3)dist.normal * Mathf.Min(dist.distance, SafetyMargin);
            }
        }
    }

    void GroundCheck()
    {
        wasGrounded = isGrounded;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, CollisionRadius * .99f, Vector2.down, 2.01f + SafetyMargin, CollisionLayer);
        if (hit && hit.distance <= groundCheckDist)
        {
            isGrounded = true;
            groundNormal = hit.normal;
            distanceToGround = hit.distance;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector2.zero;
            distanceToGround = 2.01f + SafetyMargin;
        }
    }

    void AboveCheck()
    {
        distanceToCeiling = 2.01f + SafetyMargin;
        ceilingNormal = Vector2.zero;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, CollisionRadius * .99f, Vector2.up, distanceToCeiling, CollisionLayer);
        if (hit)
        {
            distanceToCeiling = hit.distance;
            ceilingNormal = hit.normal;
        }
    }

    Vector2 InputCheck(Vector2 input)
    {
        Vector2 normal = groundNormal;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.49f, input.normalized, 1f, CollisionLayer);
        if (hit)
        {
            normal = (normal + hit.normal).normalized;
        }
        float mag = input.magnitude;
        Vector2 projection = Vector3.ProjectOnPlane(input, normal).normalized;
        input = projection * mag;

        return input;
    }

    void PreventCollision(float deltaTime)
    {
        RaycastHit2D hit;
        int iterator = 0;
        while ((hit = Physics2D.CircleCast(transform.position, 0.5f, velocity.normalized, 10000f, CollisionLayer)))
        {
            float distanceToCorrect = SafetyMargin / Vector2.Dot(velocity.normalized, hit.normal);
            float distanceToMove = hit.distance + distanceToCorrect;

            if (distanceToMove <= velocity.magnitude * deltaTime)
            {
                if (distanceToMove > 0.0f)
                {
                    transform.position += (Vector3)velocity.normalized * distanceToMove;
                }
                Vector2 normalForce = CalculateNormalForce(hit.normal, velocity);
                Vector2 optimalDirection = (Vector2)Vector3.ProjectOnPlane(velocity, hit.normal).normalized;

                float walkAngle = 1f - Mathf.Cos(walkableAngle * Mathf.Deg2Rad);
                float landingBoost = 0f;
                if (optimalDirection.magnitude > 0.1f && Vector2.Dot(hit.normal, Vector2.up) > walkAngle)
                {
                    float dot = Vector2.Dot(velocity.normalized, optimalDirection);
                    landingBoost = Mathf.Clamp01(dot) * LandingCoefficient;
                }

                float frictionMod = 1f;
                if (inputCache.magnitude > 0.1f)
                {
                    if (Vector2.Dot(inputCache, optimalDirection) > 0f)
                    {
                        frictionMod = InputFriction;
                    }
                }

                velocity += normalForce;

                Assert.IsTrue(StaticFrictionCoefficient > FrictionCoefficient);
                float staticFrictionForce = normalForce.magnitude * StaticFrictionCoefficient * frictionMod;
                float frictionForce = normalForce.magnitude * FrictionCoefficient * frictionMod;
                frictionForce -= frictionForce * landingBoost;

                if (staticFrictionForce > velocity.magnitude)
                {
                    velocity = Vector2.zero;
                }
                else
                {
                    velocity -= velocity.normalized * frictionForce;
                }
            }
            else
            {
                break;
            }

            // Escapes
            if (velocity.magnitude <= 0.001f || iterator > 100)
            {
                velocity = Vector2.zero;
                break;
            }
            iterator++;
        }
    }

    Vector2 CalculateNormalForce(Vector2 normal, Vector2 velocity)
    {
        float dot = Vector2.Dot(velocity, normal.normalized);
        return -normal.normalized * (dot > 0.0f ? 0 : dot);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)inputCache * 2f);
    }

    private void StateUpdate(float deltaTime)
    {
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 input = Vector2.ClampMagnitude(rawInput, 1f);

        Vector2 gravityForce = Vector2.down * Gravity;
        Vector2 airResistance = Deceleration * velocity;
        switch (currentState)
        {
            case PlayerState.Ground:
            {
                godFrameCount = GodFrames;
                input = InputCheck(input);
                gravityForce *= GroundedGravity;
            } break;
            case PlayerState.Air:
            {
                godFrameCount--;
                input.x *= AirControl;
                if (input.y > 0)
                {
                    input.y *= AirControl;
                }
                else
                {
                    input.y *= FastFall;
                }
            } break;
            case PlayerState.Jump:
            {
                jumpTimer += deltaTime;
                if (jumpTimer < JumpTime)
                {
                    velocity += Vector2.up * JumpHoldBoost * deltaTime;
                }

                input.x *= AirControl;
                input.y = 0;
            } break;
        }
        inputCache = input;

        Vector2 acceleration = Speed * input;
        acceleration += gravityForce;
        acceleration -= airResistance;

        velocity += acceleration * Time.deltaTime;
    }

    private void Transition()
    {
        PlayerState newState = currentState;
        switch (currentState)
        {
            case PlayerState.Ground:
            {
                newState = TransitionFromGround();
            } break;
            case PlayerState.Air:
            {
                newState = TransitionFromAir();
            } break;
            case PlayerState.Jump:
            {
                newState = TransitionFromJump();
            } break;
        }

        if (newState != currentState)
        {
            currentState = newState;
            // Enter
            Debug.Log("Enter:" + currentState);
            switch (currentState)
            {
                case PlayerState.Ground:
                {
                    EnterGround();
                } break;
                case PlayerState.Air:
                {
                    EnterAir();
                } break;
                case PlayerState.Jump:
                {
                    EnterJump();
                } break;
            }
        }
    }

    // Transition
    PlayerState TransitionFromGround()
    {
        if (CanJump())
        {
            return PlayerState.Jump;
        }
        if (!isGrounded)
        {
            return PlayerState.Air;
        }

        return PlayerState.Ground;
    }
    PlayerState TransitionFromAir()
    {
        if (CanJump())
        {
            return PlayerState.Jump;
        }
        if (isGrounded)
        {
            return PlayerState.Ground;
        }

        return PlayerState.Air;
    }
    PlayerState TransitionFromJump()
    {
        if (isGrounded)
        {
            return PlayerState.Ground;
        }
        if (!Input.GetButton("Jump") || jumpTimer >= JumpTime || velocity.y <= 0f)
        {
            return PlayerState.Air;
        }

        return PlayerState.Jump;
    }

    // Enter
    void EnterGround()
    {
        godFrameCount = GodFrames;
    }
    void EnterAir()
    {
    }
    void EnterJump()
    {
        godFrameCount = 0;
        transform.position += (Vector3)Vector2.up * groundCheckDist;
        velocity += Vector2.up * Jump;
        jumpTimer = 0f;
    }

}
