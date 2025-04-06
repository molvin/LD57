using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class LevelGenerator : MonoBehaviour
{
    public LevelSegment[] StartSegments;

    public LevelSegment[] FillerSegments;
    public LevelSegment[] SplitterSegments;
    public LevelSegment[] GoalSegments;

    public int TargetLeafAmount = 2;
    public float EndProbabilityGrowthRate = 0.1f;
    public float SplitterProbabilityGrowthRate = 0.2f;
    public int RetryAttemptsPerPath = 10;

    public int Seed;

    private LevelSegment root;
    private bool generating;

    private void Start()
    {
        LevelSegment startPrefab = StartSegments[Random.Range(0, StartSegments.Length)];
        LevelSegment start = Instantiate(startPrefab);
        root = start;
    }

    private void Update()
    {
        if(!generating && Input.GetKeyDown(KeyCode.Space))
        {
            Seed += 1;

            DestroyPath(root);
            for(int i = 0; i < 100; i++)
            {
                Debug.Log($"Building with seed {Seed}");

                bool success = GenerateLevel(Seed);
                if(success)
                {
                    break;
                }
                Seed += 1;
                Debug.Log($"Failed: Retrying with seed {Seed}");
            }
        }
    }
    
    private class GenerationState
    {
        public Door Start = null;
        public int Placed = 0;
        public bool MustPlaceSplitter = false;
        public float EndProbability = 0.0f;
        public float SplitterProbability = 0.0f;

        public bool ShouldEnd()
        {
            if (MustPlaceSplitter)
                return false;
            float rand = Random.value;
            if (rand < EndProbability)
            {
                return true;
            }
            return false;
        }

        public bool ShouldSplit()
        {
            if (!MustPlaceSplitter)
                return false;

            float rand = Random.value;
            if (rand < SplitterProbability)
            {
                return true;
            }
            return false;
        }

        public void Place(LevelGenerator gen)
        {
            Placed += 1;
            EndProbability += gen.EndProbabilityGrowthRate;
            SplitterProbability += gen.SplitterProbabilityGrowthRate;
        }
    }

    private bool GenerateLevel(int seed)
    {
        Random.InitState(seed);
        int attempts = 0;

        int availableLeafs = root.Exits.Length;
        Queue<LevelSegment> roots = new();
        roots.Enqueue(root);

        while (roots.Count > 0)
        {
            LevelSegment start = roots.Dequeue();
            foreach (Door exit in start.Exits)
            {
                bool success = true;
                for (int i = 0; i < RetryAttemptsPerPath; i++)
                {
                    GenerationState state = new GenerationState
                    {
                        Start = exit,
                        MustPlaceSplitter = availableLeafs < TargetLeafAmount
                    };

                    LevelSegment end = GeneratePath(state);
                    if (end == null)
                    {
                        success = false;
                        DestroyPath(exit);
                        Physics2D.SyncTransforms();
                        attempts += 1;
                    }
                    else
                    {
                        success = true;
                        roots.Enqueue(end);

                        if(end.Exits.Length > 1)
                        {
                            availableLeafs += end.Exits.Length - 1;
                        }
                        break;
                    }
                }

                if (!success)
                {
                    return false;
                }
            }
        }

        Debug.Log($"Succeded after {attempts} attemps");
        return true;
    }

    private LevelSegment GeneratePath(GenerationState state)
    {
        bool isEnd = false;
        Queue<Door> pending = new();
        pending.Enqueue(state.Start);
        while (pending.Count > 0)
        {
            Door exit = pending.Dequeue();
            List<LevelSegment> bucket;
            
            if (state.ShouldEnd()) // TODO: and we have filled requirements for ending
            {
                // Pick the end segment
                bucket = GoalSegments.ToList();
                isEnd = true;
            }
            else
            {
                IEnumerable<LevelSegment> validSegments = FillerSegments;
                if(state.ShouldSplit())
                {
                    validSegments = SplitterSegments;
                }
                // Make more unlikely to repeat segments
                // TODO: make less likely to repeat segments
                bucket = validSegments.OrderBy(x => Random.value).ToList();
            }

            // Try to place each available segment, if all fails this path fails
            LevelSegment spawned = null;
            foreach (LevelSegment next in bucket)
            {
                Vector3 offset = next.transform.position - next.Entrance.transform.position;
                Vector3 targetPos = exit.transform.position + offset;

                spawned = Instantiate(next, exit.transform);
                spawned.transform.position = targetPos;

                Physics2D.SyncTransforms();
                if (spawned.Overlaps())
                {
                    DestroyImmediate(spawned.gameObject);
                    spawned = null;
                }
                else
                {
                    spawned.Prefab = next;
                    exit.Connection = spawned;
                    break;
                }
            }

            // This path has failed to generate
            if (spawned == null)
            {
                return null;
            }

            // Evaluate the next segment
            if (!isEnd && 
                spawned.Exits.Length == 1)
            {
                state.Place(this);
                pending.Enqueue(spawned.Exits[0]);
            }
            // We have reached a split or an end, they need to be evaluated as separate paths
            else
            {
                return spawned;
            }
        }

        // Should never happen
        Debug.Log("Failed to generate path unexpectedly");
        return null;
    }

    private void DestroyPath(Door start)
    {
        foreach (Transform child in start.transform)
        {
            LevelSegment segment = child.GetComponentInChildren<LevelSegment>();
            if(segment != null)
            {
                DestroyImmediate(segment.gameObject);
            }
        }
    }
    private void DestroyPath(LevelSegment start)
    {
        foreach (Door exit in start.Exits)
        {
            DestroyPath(exit);
        }
    }
}