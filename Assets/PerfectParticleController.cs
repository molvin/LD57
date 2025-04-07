using UnityEngine;

public class PerfectParticleController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Material run_rigth;
    public Material run_left;
    public Material slide_rigth;
    public Material slide_left;

    public ParticleSystem imagePrticle;

    public ParticleSystem mainParticle;

    public void Play(Vector3 velocity)
    {
        Play(velocity, false);
    }
    public void Play(Vector3 velocity, bool sliding)
    {
        Material mat_to_use = sliding ?
            velocity.x > 0 ? slide_rigth : slide_left :
            velocity.x > 0 ? run_rigth : run_left;
       
        imagePrticle.GetComponent<ParticleSystemRenderer>().material = mat_to_use;
        imagePrticle.Play();
        mainParticle.Play();
    }
}
