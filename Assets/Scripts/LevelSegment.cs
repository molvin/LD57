using UnityEngine;
using System.Collections.Generic;
public enum RoomType
{
    None,
    Crossing,
    Item1Room,
    Item1Challange,
    Iteam2Room,
    Item2Challange,
    Goal,
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
    public GameObject SpawnSlot;

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
