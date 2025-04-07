using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GameUiController : MonoBehaviour
{

    public Animator countDownAnimator;
    public TMPro.TextMeshProUGUI timer;
    public Button retryButton;
    public Button returnToMenuButton;
    private AudioSource source;


    private int state = 0;

    private float _timer = 0;

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
      

            parent.StartCoroutine(parent.SetTriggerAfterTime(parent.countDownAnimator, 0, "start_countdown"));
    
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
        returnToMenuButton.gameObject.SetActive(enabled);
    }


    public class Playing : UIState
    {
        GameUiController _parent;
        public void Update(float dt) {
            _parent._timer += Time.deltaTime;
            _parent.timer.text = _parent._timer.ToString(".0##");
        }
        public void Enter(GameUiController parent)
        {
            parent.countDownAnimator.SetTrigger("move_corner");

            _parent = parent;
            _parent._timer = 0;

        }
        public void Exit() { }
        public void HandelInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _parent.Change(new LevelCompleated());

            }
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
        // _state.HandelInput();
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
        // This happens when we click retry
        // Change(new CountDown());

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

    public void CompleteLevel(System.Action retry)
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            Change(new LevelCompleated());
            while(!retryPressed)
            {
                yield return null;
            }

            retry();
        }
    }
}
