using UnityEngine;

public class PlayerParticleController : MonoBehaviour
{
    public PerfectParticleController perfectEffect;

    void Start()
    {
        
    }

    public void PlayPerfectParticle()
    {
        perfectEffect.Play(Vector3.zero);        
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayPerfectParticle();
        }
    }
}
