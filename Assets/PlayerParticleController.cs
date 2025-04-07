using UnityEngine;

public class PlayerParticleController : MonoBehaviour
{
    public PerfectParticleController perfectEffect;


    public ParticleSystem jumpParticle;
    public ParticleSystem landParticle;

    public ParticleSystem slideParticle;


    public Material jump_rigth;
    public Material jump_left;

    public ParticleSystem bonkParticle;
    public ParticleSystem bonkedParticle;

    public ParticleSystem teleportIn;
    public ParticleSystem teleportOut;

    void Start()
    {
        
    }
    public void PlayTeleportIn()
    {
        teleportIn.Play();
    }
    public void PlayTeleportout()
    {
        teleportOut.Play();
    }

    public void PlayPerfectParticle(Vector3 velocity)
    {

        perfectEffect.Play(velocity);        
    }

    public void PlayJumpParticle(Vector3 velocity)
    {
        if(velocity.x == 0)
        {
            return;
        }
        Material mat_to_use = 
            velocity.x > 0 ? jump_rigth : jump_left;

        jumpParticle.GetComponent<ParticleSystemRenderer>().material = mat_to_use;
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
    public void playBonk()
    {
        bonkParticle.Play();
    }
    public void playedBonk()
    {
        bonkedParticle.Play();
    }


}
