using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using TMPro;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;

    public static Player Player => instance.player;
    public static LevelGenerator Level => instance.generator;

    public Player playerPrefab;
    public GameUiController GameUi;
    public AbilitiesUiCotroller AbilitiesUi;
    public TextMeshProUGUI RespawnsRemaining;
    private Player player;
    private LevelGenerator generator;

    public int MaxRespawns = 3;
    public float Timer = 0.0f;

    private int respawns = 0;
    private bool runTimer = false;
    private bool disallowRespawn = false;
    private bool ending = false;

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
        respawns = 0;
        runTimer = false;
        Timer = 0.0f;

        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
            player = Instantiate(playerPrefab);
            player.TransitionTo(player.GetComponent<IdleState>());
            generator.GenerateGraph();
            PlayerPrefs.SetInt("seed", generator.Seed);
            player.CurrentAbilities.Clear();
            AbilitiesUi.ClearAbilities();
            TeleportToStart();

            bool countdownDone = false;
            System.Action go = () => { countdownDone = true;  };
            GameUi.StartCountdown(go);

            while(!countdownDone)
            {
                yield return null;
            }

            runTimer = true;

            player.TransitionTo(player.GetComponent<AirState>());
        }
    }

    private void EndLevel()
    {
        if (ending)
            return;
        ending = true;
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

            ending = false;
            StartLevel();
        }
    }

    private void Update()
    {
        instance = this;

        if(runTimer)
        {
            Timer += Time.deltaTime;
        }

        RespawnsRemaining.text = $"Remaining Respawns: {MaxRespawns - respawns}";

        if (Input.GetButtonDown("Respawn"))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if(disallowRespawn)
        {
            return;
        }

        if(respawns < MaxRespawns)
        {
            Timer = 0.0f;
            respawns += 1;
            player.CurrentAbilities.Clear();
            AbilitiesUi.ClearAbilities();

            TeleportToStart();
        }
        else
        {
        }
        
    }

    public static void PickupItem(Abilities? itemType, Vector3 pos)
    {
        if (itemType != null)
        {
            Player.CurrentAbilities.Add(itemType.Value);
            instance.StartCoroutine(Coroutine());
            IEnumerator Coroutine()
            {
                instance.disallowRespawn = true;
                instance.AbilitiesUi.AddAbility(pos, itemType.ToString());
                yield return new WaitForSeconds(2.0f);
                instance.TeleportToStart();
                instance.disallowRespawn = false;

            }
        }
        else
        {
            instance.EndLevel();
        }

    }
}
