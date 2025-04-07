using UnityEngine;


public class KeyItemPickup : MonoBehaviour
{
    public float PickupRange = 1.0f;
    public Abilities? ItemType = null;

    private Vector2 worldPosition;

    void Start()
    {
        worldPosition = Vector2.zero;
        Transform t = transform;
        while (t != null)
        {
            worldPosition += (Vector2)t.localPosition;
            t = t.parent;
        }
    }

    private void Update()
    {
        if (GameLoop.instance && Vector2.Distance(GameLoop.Player.transform.position, worldPosition) < PickupRange)
        {
            Destroy(gameObject);
            GameLoop.PickupItem(ItemType, transform.position);
        }
    }
}
