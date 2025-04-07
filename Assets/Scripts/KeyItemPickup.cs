using UnityEngine;


public class KeyItemPickup : MonoBehaviour
{
    public float PickupRange = 1.0f;
    public Abilities? ItemType = null;

    private Vector2 worldPosition;
    private SpriteRenderer sprite;


    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
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
        bool playerHasThis = ItemType != null && GameLoop.Player.CurrentAbilities.Contains(ItemType.Value);

        if (playerHasThis)
        {
            sprite.enabled = false;
            return;
        }
        sprite.enabled = true;

        if (GameLoop.instance && Vector2.Distance(GameLoop.Player.transform.position, worldPosition) < PickupRange)
        {
            if(ItemType == null)
            {
                Destroy(gameObject);
            }
            GameLoop.PickupItem(ItemType, transform.position);
        }
    }
}
