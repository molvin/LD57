using UnityEngine;

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

    public GoodSeed GetRandom()
    {
        return Seeds[Random.Range(0, Seeds.Length)];
    }

}
