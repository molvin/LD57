using Unity.VisualScripting;
using UnityEngine;

public class MenuAudioController : MonoBehaviour
{

    private AudioSource source;

    public AudioClip button_hover;
    public AudioClip swoosh;

    void Start()
    {
        source = this.GetComponent<AudioSource>();
        if (this.GetComponent<AudioSource>() == null)
        {
            source = this.AddComponent<AudioSource>();
        }
    }

    public void playButtonHover()
    {
        source.PlayOneShot(button_hover);
    }

    public void playButtonSwosh()
    {
        source.PlayOneShot(swoosh);
    }

}
