using UnityEngine;
using Exit = LevelSection.Exit;

public class Tunnel : MonoBehaviour
{
    public EdgeCollider2D Top;
    public EdgeCollider2D Bot;

    public int Steps = 10;
    public float Radius = 3;
    // TODO: for now the steps are taken linearly, but it should be following some sort of curve

    public void Connect(LevelSection first, LevelSection second)
    {
        (Exit, Exit) closest = (null, null);
        float minDistance = float.MaxValue;

        foreach(Exit firstExit in first.Exits)
        {
            foreach(Exit secondExit in second.Exits)
            {
                float distance = Vector2.Distance(firstExit.Center, secondExit.Center);
                if (distance < minDistance)
                {
                    closest = (firstExit, secondExit);
                    minDistance = distance;
                }
            }
        }
        
        (Exit start, Exit end) = closest;
        Vector2[] topPoints = new Vector2[Steps];
        Vector2[] botPoints = new Vector2[Steps];
        topPoints[0] = Top.transform.InverseTransformPoint(start.Top.Point);
        botPoints[0] = Bot.transform.InverseTransformPoint(start.Bot.Point);
        float radius = (topPoints[0] - botPoints[0]).magnitude * 0.5f;
        /*
        for (int i = 1; i < Steps - 1; i++)
        {
            Vector2 position = Vector2.Lerp(start.Center, end.Center, i / (float)Steps);
            Vector2 direction = (end.Center - start.Center).normalized;
            topPoints[i] = position + Vector2.Perpendicular(direction) * radius;
            botPoints[i] = position + Vector2.Perpendicular(direction) * radius;
        }
        */
        topPoints[Steps - 1] = Top.transform.InverseTransformPoint(end.Top.Point);
        botPoints[Steps - 1] = Bot.transform.InverseTransformPoint(end.Bot.Point);

        Top.points = topPoints;
        Bot.points = botPoints;
    }
}
