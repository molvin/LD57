using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public LevelSegment StartSegment;
    public LevelSegment[] AllSegments;

    public int MinSegments = 15;
    public int MaxSegments = 30;
    public int MinSplitters = 1;
    public int MaxSplitters = 3;

    public int Seed;

    private List<LevelSegment> Level = new();
    private Dictionary<string, List<LevelSegment>> SegmentsByEntrance = new();

    private void Start()
    {
        Level.Add(StartSegment);
        foreach(LevelSegment segment in AllSegments)
        {
            if(!SegmentsByEntrance.ContainsKey(segment.Entrance.Type))
            {
                SegmentsByEntrance.Add(segment.Entrance.Type, new());
            }

            SegmentsByEntrance[segment.Entrance.Type].Add(segment);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Seed += 1;
            StartCoroutine(BuildRandomPath(Seed));
        }
    }
    private IEnumerator BuildRandomPath(int seed)
    {
        foreach (Transform child in StartSegment.transform)
        {
            if (child.GetComponent<LevelSegment>() != null)
            {
                Destroy(child.gameObject);
            }
        }
        yield return null;
        Physics2D.SyncTransforms();

        Random.InitState(seed);

        // TODO: attempt x times
        for (int i = 0; i < 100; i++)
        {
            bool success = true;
            Queue<LevelSegment> pending = new();
            pending.Enqueue(StartSegment);
            int count = 0;
            int numSplitters = 0;
            while (pending.Count > 0)
            {
                if(count >= MaxSegments)
                {
                    // TODO: add ends to the thing
                    break;
                }

                LevelSegment segment = pending.Dequeue();
                List<LevelSegment> spawned = AddSegments(segment, numSplitters);
                yield return new WaitForSeconds(0.1f);
                if (spawned == null)
                {
                    yield return null;
                    // TODO: this whole run is invalid
                    foreach (Transform child in StartSegment.transform)
                    {
                        if (child.GetComponent<LevelSegment>() != null)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    Physics2D.SyncTransforms();
                    success = false;
                    break;
                }
                else
                {
                    count += spawned.Count;
                    foreach(LevelSegment nextSegment in spawned)
                    {
                        if (nextSegment.Exits.Length > 1)
                        {
                            numSplitters += 1;
                        }
                        pending.Enqueue(nextSegment);
                    }
                }
            }

            if(success)
            {
                Debug.Log("Finished generating");
                break;
            }
            else
            {
                Debug.Log($"Failed at {count}");
            }
        }
    }

    private List<LevelSegment> AddSegments(LevelSegment segment, int splitterCount)
    {
        List<LevelSegment> result = new();
        foreach (Door exit in segment.Exits)
        {
            List<LevelSegment> validSegments = SegmentsByEntrance[exit.Type];
            // Make more unlikely to repeat segments
            var bucket = validSegments.OrderBy(x => x == segment.Prefab ? 0.8f : Random.value);

            foreach (LevelSegment next in bucket)
            {
                if(splitterCount >= MaxSplitters && next.Exits.Length > 1)
                {
                    continue;
                }
                Vector3 offset = next.transform.position - next.Entrance.transform.position;
                Vector3 targetPos = exit.transform.position + offset;

                LevelSegment spawned = Instantiate(next, segment.transform);
                spawned.transform.position = targetPos;

                Physics2D.SyncTransforms();

                if (spawned.Overlaps())
                {
                    Destroy(spawned.gameObject);
                    spawned = null;
                }
                else
                {
                    spawned.Prefab = next;
                    exit.Connection = spawned;
                    result.Add(spawned);
                    break;
                }
            }
        }
        if (result.Count != segment.Exits.Length)
            return null;

        return result;
    }
}