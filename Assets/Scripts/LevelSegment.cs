using UnityEngine;
using System.Collections.Generic;
public enum RoomType
{
    None = 0,
    Crossing = 1,
    Item1Room = 1 << 1,
    Item1Challange = 1 << 2,
    Iteam2Room = 1 << 3,
    Item2Challange = 1 << 4,
    Goal = 1 << 5,
}
public class LevelSegment : MonoBehaviour
{
    public Door Entrance;
    public Door[] Exits;
    public RoomType RoomTypes;
    public LayerMask GroundLayers;

    //private stuff
    private Collider2D[] _fetchedGroundColliders = new Collider2D[0];
    public Collider2D[] GroundColliders
    {
        get {
            if (_fetchedGroundColliders.Length <= 0)
                _fetchedGroundColliders = GetComponentsInChildren<Collider2D>();
            return _fetchedGroundColliders;
        }
    }
    public GameObject KeyPickupSlot;

    [HideInInspector] public LevelSegment Prefab;

    [ContextMenu("Check Overlap")]
    public void PrintOverlaps()
    {
        Debug.Log($"Overlaps something: {Overlaps()}");
    }
    public bool Overlaps()
    {
        List<Collider2D> collisions = new();
        ContactFilter2D contactFilter = new();
        contactFilter.NoFilter();
        contactFilter.SetLayerMask(GroundLayers);
        foreach(Collider2D ground in GroundColliders)
        {
            int n = ground.Overlap(contactFilter, collisions);
            bool ignoreCollision = true;
            if (n > 0)
            {
                for(int i = 0; i < n; i++)
                {
                    if (collisions[i].GetComponentInParent<LevelSegment>() != this)
                    {
                        ignoreCollision = false;
                        break;
                    }
                }
                return !ignoreCollision;
            }
        }
        return false;
    }
}
