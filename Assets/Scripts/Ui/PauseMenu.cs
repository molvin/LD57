using System.Collections;
using UnityEngine;
using static GameUiController;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PState _state;
    public Animator _animator;
    public class PState
    {
        protected PauseMenu _parent;
        public PState(PauseMenu parent)
        {
            _parent = parent;
        }

        public virtual void Enter() { }

        public virtual void Exit() { }  
    }
    public class EmptyState : PState
    {
        public EmptyState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
        }

        public override void Enter()
        {
            Time.timeScale = 1;

        }
    }

    public class EntryState : PState
    {
        public EntryState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
        }

        public override void Enter()
        {
            Time.timeScale = 0;
            _parent._animator.SetTrigger("main_enter");
           // _parent.StartCoroutine(_parent.SetStateAfter(1, new MainState(_parent)));

        }
    }

    public class MainState : PState
    {
        public MainState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
            //cal anim
        }

        public override void Enter()
        {
            
        }
    }

    public void ChangeStateToEmpty()
    {
        Change(new EmptyState(this));
    }

    public class MainExitState : PState
    {
        public MainExitState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
            Time.timeScale = 1;
        }

        public override void Enter()
        {
            _parent._animator.SetTrigger("main_exit");
        }
    }

    public void Change(PState state)
    {
        _state.Exit();
        PState next = state;
        next.Enter();
        _state = next;
    }

    void Start()
    {
        _state = new EmptyState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if (_state is EmptyState)
            {
                Pause();
            }
            else if(_state is EntryState)
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        Change(new EntryState(this));
    }

    public void SetStateToIdle()
    {
        Change(new MainState(this));
    }

    public void SetStateToEmpty()
    {
        Change(new EmptyState(this));
    }

    public void Resume()
    {
        Change(new MainExitState(this));
    }

    public void Audio()
    {

    }

    public void BackToMenu()
    {

    }


    public IEnumerator SetStateAfter(float wait_time, PState new_state)
    {
        yield return new WaitForSeconds(wait_time);
        Change(new_state);

    }
}
