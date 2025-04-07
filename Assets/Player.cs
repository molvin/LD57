using UnityEngine;

public struct HitData
{
    public bool Hit;
    public Vector2 Normal;
    public Vector2 AlongGround;
    public float Time;
}

public class Player : MonoBehaviour
{
    public State CurrentState;
    public Animator Anim;

    public Vector2 Velocity;
    public float BonkVelocityMin;
    public float HardLandVelocityMin;

    [Header("Collision Settings")]
    public LayerMask CollisionLayer;
    public float CollisionRadius = 0.5f;

    public float SafetyMargin = 0.02f;
    public float walkableAngle = 70;

    private Vector2 groundNormal;
    private new CircleCollider2D collider;

    public HitData LastHit;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        CurrentState.Tick();

        PreventCollision(Time.deltaTime);
        // velocity = Vector2.ClampMagnitude(velocity, Speed * 10f);
        transform.position += (Vector3)Velocity * Time.deltaTime;

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

    public Vector2 InputCheck(Vector2 input)
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
        while ((hit = Physics2D.CircleCast(transform.position, 0.5f, Velocity.normalized, 10000f, CollisionLayer)))
        {
            float distanceToCorrect = SafetyMargin / Vector2.Dot(Velocity.normalized, hit.normal);
            float distanceToMove = hit.distance + distanceToCorrect;

            if (distanceToMove <= Velocity.magnitude * deltaTime)
            {
                LastHit = new HitData { Hit = true, Normal = hit.normal, Time = Time.time, AlongGround = Vector2.Perpendicular(hit.normal)};
                if (CurrentState is AirState || CurrentState is GroundState)
                {
                    float velocityIntoCollision = Vector2.Dot(-hit.normal, Velocity);
                    float angle = Vector2.Angle(Vector2.up, hit.normal);

                    if (angle < 75)
                    {
                        if (velocityIntoCollision > HardLandVelocityMin)
                        {
                            TransitionTo(GetComponent<HardLandState>());
                        }
                    }
                    else
                    {
                        if (velocityIntoCollision > BonkVelocityMin)
                        {
                            TransitionTo(GetComponent<BonkState>());
                        }
                    }
                }

                if (distanceToMove > 0.0f)
                {
                    transform.position += (Vector3)Velocity.normalized * distanceToMove;
                }
                Vector2 normalForce = CalculateNormalForce(hit.normal, Velocity);
                Velocity += normalForce;
            }
            else
            {
                break;
            }

            // Escapes
            if (Velocity.magnitude <= 0.001f || iterator > 100)
            {
                Velocity = Vector2.zero;
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

    public HitData GroundCheck(float distance)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, CollisionRadius * .99f, Vector2.down, 2.01f + SafetyMargin, CollisionLayer);
        if(hit && hit.distance <= distance)
        {
            Vector2 dirAlongGround = Vector2.Perpendicular(hit.normal);
            return new HitData
            {
                Hit = hit.collider != null,
                Normal = hit.normal,
                AlongGround = dirAlongGround,
                Time = Time.time
            };
        }
        else
        {
            return new HitData{ };
        }
    }

    public void TransitionTo(State s)
    {
        s.Enter();
        CurrentState = s;
    }
}
