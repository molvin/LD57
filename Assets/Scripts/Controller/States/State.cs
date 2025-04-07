using UnityEngine;

public abstract class State : MonoBehaviour
{
    public Player Owner;

    public abstract void Enter();
    public abstract void Tick();

    public abstract void Exit();

}
