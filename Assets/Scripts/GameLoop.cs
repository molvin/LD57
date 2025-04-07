using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    public static Player Player => instance.player;
    public static LevelGenerator Level => instance.generator;

    public Player playerPrefab;
    public GameUiController GameUi;
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

    private void StartLevel()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            if (player != null)
            {
                Destroy(player);
            }
            player = Instantiate(playerPrefab);
            player.TransitionTo(player.GetComponent<IdleState>());
            generator.GenerateGraph();
            PlayerPrefs.SetInt("seed", generator.Seed);
            player.GrantedItems = 0;
            player.CurrentAbilities.Clear();
            TeleportToStart();

            bool countdownDone = false;
            System.Action go = () => { countdownDone = true;  };
            GameUi.StartCountdown(go);

            while(!countdownDone)
            {
                yield return null;
            }

            player.TransitionTo(player.GetComponent<AirState>());
        }


    }

    private void EndLevel()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            player.Velocity = Vector3.zero;
            Debug.Log("Congrats");

            bool doRetry = false;
            System.Action retry = () => { doRetry = true;  };
            GameUi.CompleteLevel(retry);
            while (!doRetry)
            {
                yield return null;
            }

            StartLevel();
        }
    }

    private void Update()
    {
        instance = this;

        if (Input.GetKeyDown(KeyCode.R))
        {
            TeleportToStart();
        }
    }

    public static void PickupItem(Abilities itemType)
    {
        // Player.GrantedItems |= 1 << (int)itemType;
        Player.GrantedItems += 1;
        // if (Player.GrantedItems == ((1 << (int)KeyItemType.Item1) & (1 << (int)KeyItemType.Item2)))

        Player.CurrentAbilities.Add(itemType);

        if (Player.GrantedItems == 3)
        {
            instance.EndLevel();
        }
        else
        {
            instance.TeleportToStart();
        }
    }
}
