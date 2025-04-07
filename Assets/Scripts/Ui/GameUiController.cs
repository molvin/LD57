using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameLoop;
using static System.Net.Mime.MediaTypeNames;

public class GameUiController : MonoBehaviour
{

    public Animator countDownAnimator;
    public TMPro.TextMeshProUGUI timer;
    public Button retryButton;
    public Button returnToMenuButton;
    private AudioSource source;

    public Animator FadeAnim;

    public UnityEngine.UI.Image m_MedalImage;
    public TMPro.TextMeshProUGUI m_TimeToNextMedal;
    public TMPro.TextMeshProUGUI m_MedalText;

    public Sprite m_AuthorSprite;
    public Sprite m_GoldSprite;
    public Sprite m_SilverSprite;
    public Sprite m_BronzSprite;

    private int state = 0;

    private UIState _state;

    private bool retryPressed;

    public void Start()
    {
        this._state = new EmptyState();
        _state.Enter(this);

        source = this.GetComponent<AudioSource>();
        if (this.GetComponent<AudioSource>() == null)
        {
            source = this.AddComponent<AudioSource>();
        }


    }


    public void FadeOut() {
        FadeAnim.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        FadeAnim.SetTrigger("FadeIn");
    }

    public void playSound(AudioClip clip)
    {
        source.PlayOneShot(clip);

    }
    public interface UIState
    {
        void Update(float dt);

        void Enter(GameUiController parent);

        void HandelInput();
        void Exit();
    }

    public class EmptyState : UIState
    {
        GameUiController _parent;
        public void Update(float dt)
        {
        }
        public void Enter(GameUiController parent)
        {
            _parent = parent;

        }
        public void Exit() { }
        public void HandelInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _parent.Change(new CountDown());

            }
        }
    }

    public class CountDown : UIState
    {
        GameUiController _parent;
        public void Update(float dt) { }
        public void HandleInput() { }
        public void Enter(GameUiController parent) {
            _parent = parent;

            parent.countDownAnimator.SetTrigger("start_countdown");
            //parent.StartCoroutine(parent.SetTriggerAfterTime(, 0, "start_countdown"));
    
            parent.StartCoroutine(parent.SetStateAfter(4, new Playing()));

        }
        public void Exit() { }

        public void HandelInput()
        {
           
        }
    }

    public void SetText(string text)
    {
        timer.text = text;
    }

    public void setStateToPlaying()
    {
        Change(new Playing());
    }
    public void EnableRetryButton()
    {
        EnableRetryButton(true);

    }
    public void DisableRetryButton()
    {
        EnableRetryButton(false);

    }


    public void EnableRetryButton(bool enable)
    {
        retryButton.gameObject.SetActive(enable);
        returnToMenuButton.gameObject.SetActive(enable);
    }


    public class Playing : UIState
    {
        GameUiController _parent;
        public void Update(float dt) {
            _parent.timer.text = GameLoop.instance.Timer.ToString(".0##");
        }
        public void Enter(GameUiController parent)
        {
            parent.countDownAnimator.SetTrigger("move_corner");

            _parent = parent;

        }
        public void Exit() { }
        public void HandelInput()
        {
        }
    }


    public class LevelCompleated : UIState
    {
        GameUiController _parent;

        public void Update(float dt) { }
        public void Enter(GameUiController parent)
        {
            _parent = parent;

            parent.countDownAnimator.SetTrigger("move_middle");
        }
        public void Exit() {
            _parent.EnableRetryButton(false);
            _parent.countDownAnimator.SetTrigger("reset");

        }
        public void HandelInput()
        {
        }
    }

    // Update is called once per frame
    void Update()
    {

        _state.Update(Time.deltaTime);
    }
   

    public void Change(UIState state)
    {
        _state.Exit();
        UIState next = state;
        next.Enter(this);
        _state = next;
    }

    public void Reset()
    {
        retryPressed = true;
    }

    public void BackToMenu()
    {
        // Back to manager
        SceneManager.LoadScene(0);  
    }

    public IEnumerator SetStateAfter(float wait_time, UIState new_state)
    {
        yield return new WaitForSeconds(wait_time);
        Change(new_state);

    }


    public IEnumerator SetTextAfterTime(TMPro.TextMeshProUGUI obj, float wait_time, string text)
    {
        yield return new WaitForSeconds(wait_time);
        obj.text = text;
        
    }

    public IEnumerator SetTriggerAfterTime(Animator anim, float wait_time, string trigger)
    {
        yield return new WaitForSeconds(wait_time);
        anim.SetTrigger(trigger);

    }

    public IEnumerator EnableAfterTime(GameObject obj, bool enable, float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        obj.SetActive(enable);

    }


    public void StartCountdown(System.Action completed)
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            Change(new CountDown());

            while (_state is CountDown)
            {
                yield return null;
            }

            completed();
        }
    }

    private Sprite GetMedalSprite(MedalType medal)
    {
        switch (medal)
        {
            case MedalType.None:
                return null;
            case MedalType.Bronze:
                return m_BronzSprite;
            case MedalType.Silver:
                return m_SilverSprite;
            case MedalType.Gold:
                return m_GoldSprite;
            case MedalType.Author:
                return m_AuthorSprite;
        }

        return null;
    }

    public void CompleteLevel(System.Action retry, float completeTime, float timeToNextMeddal, MedalType meddalType)
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            Change(new LevelCompleated());
            m_MedalImage.gameObject.SetActive(true);
            m_MedalText.gameObject.SetActive(true);
            if (meddalType != MedalType.Author)
                m_TimeToNextMedal.gameObject.SetActive(true);

            if(meddalType != MedalType.Author)
                m_TimeToNextMedal.text = $"Time to {(meddalType + 1).ToString()} {timeToNextMeddal.ToString(".0##")}";

            m_MedalText.text = meddalType.ToString();
            m_MedalImage.sprite = GetMedalSprite(meddalType);

            while (!retryPressed)
            {
                yield return null;
            }

            m_MedalImage.gameObject.SetActive(false);
            m_MedalText.gameObject.SetActive(false);
            m_TimeToNextMedal.gameObject.SetActive(false);
            retryPressed = false;
            retry();
        }
    }
}
