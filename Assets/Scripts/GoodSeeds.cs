using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GoodSeed
{
    public int Seed;
    public float Author = 65;
    public float Gold = 85;
    public float Silver = 100;
    public float Bronze = 120;

    public float[] Times => new float[] { Author, Gold, Silver, Bronze };
}

[CreateAssetMenu]
public class GoodSeeds : ScriptableObject
{
    public GoodSeed[] Seeds;

    private Queue<GoodSeed> queue;

    public void Init()
    {
        queue = new Queue<GoodSeed>(Seeds.OrderBy(x => Random.value));
    }

    public GoodSeed GetNext()
    {
        return queue.Count == 0 ? null : queue.Dequeue();
    }

}
