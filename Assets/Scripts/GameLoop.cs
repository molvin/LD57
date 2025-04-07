using UnityEngine;
using UnityEngine.Assertions;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    public static Player Player => instance.player;
    public static LevelGenerator Level => instance.generator;

    private Player player;
    private LevelGenerator generator;

    private void TeleportToStart()
    {
        Player.transform.position = Level.StartPosition;
        Player.Velocity = Vector2.zero;
        Player.TransitionTo(Player.GetComponent<AirState>());
    }

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;

        player = FindFirstObjectByType<Player>();
        generator = FindAnyObjectByType<LevelGenerator>();
        generator.Seed = PlayerPrefs.GetInt("seed", 0);
        generator.debugMode = false;
    }

    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        generator.GenerateGraph();
        PlayerPrefs.SetInt("seed", generator.Seed);

        TeleportToStart();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            TeleportToStart();
        }
    }

    public static void PickupItem(KeyItemType itemType)
    {
        Player.GrantedItems |= 1 << (int)itemType;

        if (Player.GrantedItems == ((1 << (int)KeyItemType.Item1) & (1 << (int)KeyItemType.Item2)))
        {
            instance.StartLevel();
        }
        else
        {
            instance.TeleportToStart();
        }
    }
}
