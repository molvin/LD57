using UnityEngine;

public class TeleportOutParticleController : MonoBehaviour
{
    public Material jump, jump_rev, jump_f, jump_f_rev, idle;


    public void Play(Vector3 velocity, State state)
    {
        ParticleSystem effect = this.GetComponent<ParticleSystem>();
        Material mat_to_use;

        if(state.GetType() == typeof(AirState))
        {
            mat_to_use = velocity.y > 0 ?
                velocity.x > 0 ? jump : jump_rev : 
                velocity.x > 0 ? jump_f : jump_f_rev;

        } else
        {
            mat_to_use = idle;
        }

        effect.GetComponent<ParticleSystemRenderer>().material = mat_to_use;

        effect.Play();
    }
}
