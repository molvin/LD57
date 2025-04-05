using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GameUiController : MonoBehaviour
{

    public Animator countDownAnimator;
    public TMPro.TextMeshProUGUI timer;
    public Button retryButton;
    

    private int state = 0;

    private float _timer = 0;

    private UIState _state;
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
            parent.StartCoroutine(parent.SetTextAfterTime(parent.timer, 0, "3"));
            parent.StartCoroutine(parent.SetTriggerAfterTime(parent.countDownAnimator, 0, "start_countdown"));

            parent.StartCoroutine(parent.SetTextAfterTime(parent.timer, 1, "2"));
            parent.StartCoroutine(parent.SetTriggerAfterTime(parent.countDownAnimator, 1, "start_countdown"));

            parent.StartCoroutine(parent.SetTextAfterTime(parent.timer, 2, "1"));
            parent.StartCoroutine(parent.SetTriggerAfterTime(parent.countDownAnimator, 2, "start_countdown"));

            parent.StartCoroutine(parent.SetTextAfterTime(parent.timer, 3, "GO"));
            parent.StartCoroutine(parent.SetTriggerAfterTime(parent.countDownAnimator, 3, "start_countdown"));
            parent.StartCoroutine(parent.SetTriggerAfterTime(parent.countDownAnimator, 4, "move_corner"));
            parent.StartCoroutine(parent.SetStateAfter(4, new Playing()));

        }
        public void Exit() { }

        public void HandelInput()
        {
           
        }
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
            parent.StartCoroutine(parent.EnableAfterTime(parent.retryButton.gameObject, true, 0.2f));
        }
        public void Exit() {
            _parent.retryButton.gameObject.SetActive(false);
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
        _state.HandelInput();
     

    }
    public void Start()
    {
        this._state = new EmptyState();
        _state.Enter(this);

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
        Change(new CountDown());

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

}
