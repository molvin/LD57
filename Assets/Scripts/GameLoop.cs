using UnityEngine;
using UnityEngine.Assertions;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    public static Player Player => instance.player;
    public static LevelGenerator Level => instance.generator;

    public Player playerPrefab;
    private Player player;
    private LevelGenerator generator;

    private void TeleportToStart()
    {
        Player.transform.position = Level.StartPosition;
        Player.Velocity = Vector2.zero;
    }

    private void Awake()
    {
        instance = this;

        generator = FindAnyObjectByType<LevelGenerator>();
        generator.Seed = PlayerPrefs.GetInt("seed", 0);
        generator.debugMode = false;
    }

    private void Start()
    {
        StartLevel();
    }

    private void Update()
    {
        instance = this;
    }

    private void StartLevel()
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab);

        generator.GenerateGraph();
        PlayerPrefs.SetInt("seed", generator.Seed);

        TeleportToStart();
    }

    public static void PickupItem(KeyItemType itemType)
    {
        Player.GrantedItems |= 1 << (int)itemType;

        if (Player.GrantedItems == ((1 << (int)KeyItemType.Item1) | (1 << (int)KeyItemType.Item2)))
        {
            instance.StartLevel();
        }
        else
        {
            instance.TeleportToStart();
        }
    }
}
