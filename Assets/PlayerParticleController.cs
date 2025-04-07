using UnityEngine;

public class PlayerParticleController : MonoBehaviour
{
    public PerfectParticleController perfectEffect;

    void Start()
    {
        
    }

    public void PlayPerfectParticle(Vector3 velocity)
    {
        perfectEffect.Play(velocity);        
    }




}
