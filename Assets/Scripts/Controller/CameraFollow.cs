using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float ZoomLevel = 12;
    public float WeZoomin = 20;
    public float OffsetY = 1;
    public float SmoothSpeed = 0.5f;
    public float VerticalUpSmooth = 0.3f;
    public float VerticalDownSmooth = 0.4f;
    public float MaxDistanceFromTarget = 6;

    private Vector2 cameraPos;
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        cameraPos = (Vector2)player.transform.position + Vector2.up * OffsetY;
    }

    void LateUpdate()
    {
        player = GetComponentInParent<Player>();

        Vector2 targetPos = (Vector2)player.transform.position + Vector2.up * OffsetY;
        float verticalSmooth = player.Velocity.y > 0 ? VerticalUpSmooth : VerticalDownSmooth;
        Vector2 smoothVelocity = new Vector2(player.Velocity.x, player.Velocity.y * verticalSmooth);
        targetPos += smoothVelocity;

        targetPos = (Vector2)transform.position + (targetPos - cameraPos) * SmoothSpeed * Time.deltaTime;
        Vector2 deltaPos = (targetPos - (Vector2)player.transform.position);
        deltaPos = Vector2.ClampMagnitude(deltaPos, MaxDistanceFromTarget);
        targetPos = (Vector2)player.transform.position + deltaPos;

        float zoom = Mathf.Clamp01(3f - Mathf.Log(1 + player.Velocity.magnitude));
        zoom = Mathf.Lerp(WeZoomin, ZoomLevel, zoom);
        float currentZoom = Mathf.Abs(transform.position.z);
        float deltaZoom = (zoom - currentZoom) * SmoothSpeed * Time.deltaTime;
        zoom = currentZoom + deltaZoom;

        Vector3 pos = new Vector3(targetPos.x, targetPos.y, -zoom);
        transform.position = pos;
        cameraPos = pos;
    }
}
