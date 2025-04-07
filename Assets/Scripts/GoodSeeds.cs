using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GoodSeed
{
    public int Seed;
    public float Author = 20;
    public float Gold = 30;
    public float Silver = 40;
    public float Bronze = 50;

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
