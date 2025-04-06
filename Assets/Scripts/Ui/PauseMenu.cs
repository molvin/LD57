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
        }
    }

    public class EntryState : PState
    {
        public EntryState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
            //TODO set timescale to 0
            
            //enable layout?
        }

        public override void Enter()
        {
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

    public class MainExitState : PState
    {
        public MainExitState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
           

            //cal anim
        }

        public override void Enter()
        {
            _parent._animator.SetTrigger("main_exit");
            _parent.StartCoroutine(_parent.SetStateAfter(1, new EmptyState(_parent)));
        }
    }

    public class AudioEnterState : PState
    {
        public AudioEnterState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
            //cal anim
        }

        public override void Enter()
        {
            //cal anim
        }
    }


    public class AudioState : PState
    {
        public AudioState(PauseMenu parent) : base(parent)
        {
        }

        public override void Exit()
        {
            //call anim
        }

        public override void Enter()
        {
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
        if(Input.GetKeyDown(KeyCode.P))
        {
            Pause();
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
