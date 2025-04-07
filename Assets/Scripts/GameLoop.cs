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

    public GoodSeeds Seeds;

    private int respawns = 0;
    private bool runTimer = false;

    public enum MedalType
    {
        None,
        Bronze,
        Silver,
        Gold,
        Author,
    }

    private void TeleportToStartInitial()
    {
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            Player.enabled = false;
            Player.Anim.gameObject.SetActive(false);
            Player.transform.position = Level.StartPosition;
            Player.GetComponent<PlayerParticleController>().PlayTeleportIn();
            yield return new WaitForSeconds(1.5f);
            Player.enabled = true;
            Player.Anim.gameObject.SetActive(true);
        }
    }

    private bool disallowRespawn = false;
    private bool ending = false;
    public GoodSeed CurrentGoodSeed;

    private void TeleportToStart(bool resetTimer)
    {
        //Player.transform.position = Level.StartPosition;
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            //play teleport out anim
            runTimer = false;

            float speed = Vector3.Distance(Player.transform.position, Level.StartPosition);
            Player.enabled = false;
            Player.GetComponent<PlayerParticleController>().PlayTeleportout(Player.Velocity, Player.CurrentState);
            Player.Velocity = Vector2.zero;
            Player.Anim.enabled = false;
            yield return new WaitForSeconds(1);
            Player.Anim.gameObject.SetActive(false);
            while (Vector3.Distance(Player.transform.position, Level.StartPosition) > 1f)
            {
                Player.transform.position +=  ((Vector3)Level.StartPosition - Player.transform.position).normalized * speed * Time.deltaTime;
         
                yield return null;
            }
            Player.transform.position = Level.StartPosition;
            Player.GetComponent<PlayerParticleController>().PlayTeleportIn();
            yield return new WaitForSeconds(1.5f);
            Player.enabled = true;
            Player.Anim.enabled = true;
            Player.Anim.gameObject.SetActive(true);
            if(resetTimer)
            {
                Timer = 0.0f;
            }
            runTimer = true;
            //play teleport in anim
            yield return null;
        }
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
            CurrentGoodSeed = Seeds.GetRandom();
            generator.GenerateGraph(CurrentGoodSeed.Seed);
            PlayerPrefs.SetInt("seed", generator.Seed);
            player.CurrentAbilities.Clear();
            AbilitiesUi.ClearAbilities();
            GameUi.FadeOut();
            TeleportToStartInitial();

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

            MedalType medal = MedalType.None;
            float nextMeddalTime = CurrentGoodSeed.Bronze;
            if(Timer <= CurrentGoodSeed.Author)
            {
                medal = MedalType.Author;
                nextMeddalTime = -1;
            }
            else if (Timer <= CurrentGoodSeed.Gold)
            {
                medal = MedalType.Gold;
                nextMeddalTime = -1;
            }
            else if (Timer <= CurrentGoodSeed.Silver)
            {
                medal = MedalType.Silver;
                nextMeddalTime = CurrentGoodSeed.Gold;
            }
            else if (Timer <= CurrentGoodSeed.Bronze)
            {
                medal = MedalType.Bronze;
                nextMeddalTime = CurrentGoodSeed.Silver;
            }

            GameUi.CompleteLevel(retry, Timer, nextMeddalTime, medal);
            while (!doRetry)
            {
                yield return null;
            }

            ending = false;
            GameUi.FadeIn();
            yield return new WaitForSeconds(0.8f);
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

        RespawnsRemaining.text = $"Retries: {respawns}\nPress R to Retry";

        if (Input.GetButtonDown("Respawn"))
        {
            //EndLevel();
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
         
            respawns += 1;
            player.CurrentAbilities.Clear();
            AbilitiesUi.ClearAbilities();
            TeleportToStart(true);
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
                instance.TeleportToStart(false);
                instance.disallowRespawn = false;

            }
        }
        else
        {
            instance.EndLevel();
        }

    }
}
