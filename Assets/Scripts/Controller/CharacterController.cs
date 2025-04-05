using UnityEngine;
using UnityEngine.Assertions;

public class CharacterController : MonoBehaviour
{
    public LayerMask collisionLayer;
    public float Speed = 15f;
    public float SafetyMargin = 0.02f;
    public float Gravity = 9.81f;
    public float FrictionCoefficient = 0.3f;
    public float Jump = 50f;
    public float LandingSpeedBoost = 200f;
    public int GodFrames = 3;
    public float Deceleration = 1;
    public float AirControl = 0.3f;
    private Vector2 velocity;
    private Vector2 groundNormal;
    private float inAir = 0f;

    private new CircleCollider2D collider;

    private Vector2 inputCache;
    private int godFrameCount = 0;

    private void Start()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    // TODO: Move to fixed update for server logic to work
    private void Update()
    {
        bool wasGrounded = groundNormal != Vector2.zero;
        GroundCheck();
        bool IsGrounded = groundNormal != Vector2.zero;

        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (!IsGrounded && godFrameCount <= 0)
        {
            rawInput.x *= AirControl;
            if (rawInput.y > 0)
            {
                rawInput.y *= AirControl;
            }
        }

        Vector2 input = Vector2.ClampMagnitude(rawInput, 1f);
        if (IsGrounded)
        {
            float inputDot = Vector2.Dot(input, groundNormal);
            if (inputDot < -0.1f)
            {
                input = Vector2.Lerp((input - inputDot * groundNormal).normalized, input, 0.3f);
            }
        }
        inputCache = input;
        Assert.IsTrue(input.magnitude <= 1.001f);

        Vector2 acceleration = Speed * input;
        acceleration += Vector2.down * Gravity;
        if (!wasGrounded && IsGrounded)
        {
            Vector2 boostDir = Vector3.ProjectOnPlane(new Vector2(input.x, 0), groundNormal);
            acceleration += boostDir * Mathf.Log(1f + inAir) * LandingSpeedBoost;
        }
        if (IsGrounded)
        {
            inAir = 0;
            godFrameCount = GodFrames;
        }
        else
        {
            inAir += Time.deltaTime;
            godFrameCount--;
        }

        velocity += acceleration * Time.deltaTime;
        if (godFrameCount > 0 && Input.GetKeyDown(KeyCode.Space) && groundNormal.y > 0.0f)
        {
            velocity += Vector2.up * Jump;
            godFrameCount = 0;
        }
        velocity = Vector2.ClampMagnitude(velocity, Speed * 10f);

        PreventCollision(Time.deltaTime);
        transform.position += (Vector3)velocity * Time.deltaTime;

        Collider2D overlap = Physics2D.OverlapCircle((Vector2)transform.position, 0.5f, collisionLayer);
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
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.49f, Vector2.down, 0.1f, collisionLayer);
        groundNormal = hit.normal;
    }

    void PreventCollision(float deltaTime)
    {
        RaycastHit2D hit;
        int iterator = 0;
        while ((hit = Physics2D.CircleCast(transform.position, 0.5f, velocity.normalized, 10000f, collisionLayer)))
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
                velocity += normalForce;
                velocity -= velocity.normalized * normalForce.magnitude * FrictionCoefficient;
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
}
