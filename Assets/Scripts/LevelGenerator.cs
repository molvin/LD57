using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public LevelSegment[] StartSegments;

    public LevelSegment[] FillerSegments;
    public LevelSegment[] SplitterSegments;
    public LevelSegment[] GoalSegments;
    public LevelSegment[] DoubleJumpChallenges;
    public LevelSegment[] SlideChallenges;

    public int TargetLeafAmount = 2;
    public float EndProbabilityGrowthRate = 0.1f;
    public float SplitterProbabilityGrowthRate = 0.2f;
    public float ChallengeProbabilityGrowthRate = 0.2f;
    public int RetryAttemptsPerPath = 10;

    public int Seed;

    public Dictionary<Abilities, LevelSegment[]> ChallengeSegments;

    public Vector2 StartPosition => root.SpawnSlot.transform.position;
    private LevelSegment root;
    public bool debugMode = false;

    List<Transform> parents = new();


    private void Awake()
    {
        ChallengeSegments = new();
        ChallengeSegments.Add(Abilities.DoubleJump, DoubleJumpChallenges);
        ChallengeSegments.Add(Abilities.Slide, SlideChallenges);
    }

    private void Update()
    {
        if(debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            GenerateGraph();
        }
    }

    public void GenerateGraph(int? overrideSeed = null)
    {
        for(int i = 0; i < 100; i++)
        {
            if(root != null)
            {
                DestroyImmediate(root.gameObject);
            }
            LevelSegment startPrefab = StartSegments[Random.Range(0, StartSegments.Length)];
            LevelSegment start = Instantiate(startPrefab, Vector3.zero, Quaternion.identity);
            root = start;
            foreach (Transform parent in parents)
            {
                DestroyImmediate(parent.gameObject);
            }
            parents.Clear();

            bool success = GenerateLevel(overrideSeed ?? Seed);

            Seed += 1;

            if (success)
            {
                break;
            }
            Debug.Log($"Failed: Retrying with seed {Seed}");
        }

        foreach(Transform parent in parents)
        {
            //ZeroZ(parent);
        }
    }

    private void ZeroZ(Transform trans)
    {
        Vector3 localPos = trans.localPosition;
        localPos.z = 0;
        trans.localPosition = localPos;

        foreach (Transform t in trans)
        {
            ZeroZ(t);
        }
    }
    
    private class GenerationState
    {
        public Door Start = null;
        public LevelSegment LastPlaced = null;
        public int Placed = 0;
        public bool MustPlaceSplitter = false;
        public float EndProbability = 0.0f;
        public float SplitterProbability = 0.0f;
        public Abilities? Ability;

        public float ChallengeProbability = 0.0f;
        public bool MustPlaceChallenge = false;
        public List<Abilities> PlacedAbilities;

        public bool ShouldEnd()
        {
            if (MustPlaceSplitter)
                return false;
            if (MustPlaceChallenge)
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

        public bool ShouldPlaceChallenge()
        {
            if (!MustPlaceChallenge)
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
            ChallengeProbability += gen.ChallengeProbabilityGrowthRate;
        }
    }

    private bool GenerateLevel(int seed)
    {
        Debug.Log($"Generating with seed {seed}");
        Random.InitState(seed);
        int attempts = 0;

        int availableLeafs = root.Exits.Length;
        Queue<Door> remainingPaths = new();
        foreach(Door exit in root.Exits)
        {
            remainingPaths.Enqueue(exit);
        }

        List<Abilities> allAbilities = new();
        foreach(Abilities ab in System.Enum.GetValues(typeof(Abilities)))
        {
            allAbilities.Add(ab);
        }
        allAbilities = allAbilities.OrderBy(x => Random.value).ToList();
        int nextAbility = 0;

        Abilities? abilityForChallenge = null;

        remainingPaths = new Queue<Door>(remainingPaths.OrderBy(x => Random.value));
        while (remainingPaths.Count > 0)
        {
            Door exit = remainingPaths.Dequeue();

            bool success = true;
            for (int i = 0; i < RetryAttemptsPerPath; i++)
            {
                GenerationState state = new()
                {
                    Start = exit,
                    MustPlaceSplitter = availableLeafs < TargetLeafAmount,
                    MustPlaceChallenge = abilityForChallenge != null,
                    Ability = abilityForChallenge ?? (nextAbility < 2 ? allAbilities[Mathf.Min(nextAbility, allAbilities.Count - 1)] : null),
                };
                Transform parent = new GameObject($"Path: Split: {state.MustPlaceSplitter}, Challenge: {state.MustPlaceChallenge}, Ability: {state.Ability}").transform;
                LevelSegment end = GeneratePath(state, parent);
                if (end == null)
                {
                    success = false;
                    DestroyImmediate(parent.gameObject);
                    Physics2D.SyncTransforms();
                    attempts += 1;
                }
                else
                {
                    success = true;
                    foreach(Door newExit in end.Exits)
                    {
                        remainingPaths.Enqueue(newExit);
                    }
                    parents.Add(parent);

                    if(end.Exits.Length > 1) // Placed Splitter
                    {
                        availableLeafs += end.Exits.Length - 1;
                    }
                    else if(end.Exits.Length == 0) // Placed end
                    {
                        abilityForChallenge = state.Ability;
                        nextAbility += 1;
                    }
                    else // Placed Challenge
                    {
                        abilityForChallenge = null;
                    }
                    break;
                }
            }

            if (!success)
            {
                return false;
            }

            remainingPaths = new Queue<Door>(remainingPaths.OrderBy(x => Random.value));
        }

        Debug.Log($"Succeded after {attempts} attemps");
        return true;
    }

    private LevelSegment GeneratePath(GenerationState state, Transform parent)
    {
        bool isEnd = false;
        bool isChallenge = false;
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
                else if(state.ShouldPlaceChallenge())
                {
                    isChallenge = true;
                    validSegments = ChallengeSegments[state.Ability.Value];
                }
                if(state.LastPlaced != null)
                {
                    bucket = validSegments.OrderBy(x => x.Prefab == state.LastPlaced.Prefab ? 0.8f : Random.value).ToList();
                }
                else
                {
                    bucket = validSegments.OrderBy(x => Random.value).ToList();
                }
            }

            // Try to place each available segment, if all fails this path fails
            LevelSegment spawned = null;
            foreach (LevelSegment next in bucket)
            {
                Vector3 offset = next.transform.position - next.Entrance.transform.position;
                Vector3 targetPos = exit.transform.position + offset;

                spawned = Instantiate(next, parent);
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
                    state.LastPlaced = spawned;
                    exit.gameObject.SetActive(false);
                    break;
                }
            }

            // This path has failed to generate
            if (spawned == null)
            {
                Debug.LogWarning($"Failed to find fitting segment for: {exit.GetComponentInParent<LevelSegment>()}");
                return null;
            }

            // Evaluate the next segment
            if (!isEnd && !isChallenge && spawned.Exits.Length == 1)
            {
                state.Place(this);
                pending.Enqueue(spawned.Exits[0]);
            }
            // We have reached a split or an end, they need to be evaluated as separate paths
            else
            {
                if(isEnd)
                {
                    spawned.GetComponentInChildren<KeyItemPickup>().ItemType = state.Ability;
                }

                return spawned;
            }
        }

        // Should never happen
        Debug.Log("Failed to generate path unexpectedly");
        return null;
    }
}