using UnityEngine;

public enum KeyItemType
{
    Item1,
    Item2
}

public class KeyItemPickup : MonoBehaviour
{
    public float PickupRange = 1.0f;
    public KeyItemType ItemType;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (Vector2.Distance(playerController.transform.position, transform.position) < PickupRange)
        {
            playerController.GrantedItems |= 1 << (int)ItemType;
        }
    }
}
