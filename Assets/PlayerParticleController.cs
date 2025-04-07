using UnityEngine;

public class PlayerParticleController : MonoBehaviour
{
    public PerfectParticleController perfectEffect;


    public ParticleSystem jumpParticle;
    public ParticleSystem landParticle;

    public ParticleSystem slideParticle;

    void Start()
    {
        
    }

    public void PlayPerfectParticle(Vector3 velocity)
    {
        perfectEffect.Play(velocity);        
    }

    public void PlayJumpParticle()
    {
        jumpParticle.Play();
    }
    public void PlayLandParticle()
    {
        landParticle.Play();
    }

    public void startSlide()
    {
        slideParticle.Play();
    }
    public void stopSlide()
    {
        slideParticle.Stop();
    }


}
