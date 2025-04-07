using Unity.VisualScripting;
using UnityEngine;

public class MenuAudioController : MonoBehaviour
{

    private AudioSource source;

    public AudioEventData button_hover;
    public AudioEventData swoosh;
    public AudioEventData bulbsmash;
    public AudioEventData lets;
    public AudioEventData go;


    public void play(AudioClip audioClip)
    {
        source.PlayOneShot(audioClip);

    }

    public void playButtonHover()
    {
        AudioManager.PlayAudio(button_hover);
    }

    public void playButtonSwosh()
    {
        AudioManager.PlayAudio(swoosh);
    }

    public void playBulbsmash()
    {
        AudioManager.PlayAudio(bulbsmash);
    }

    public void playlets()
    {
        lets.Play();
    }
    public void playgo()
    {
        go.Play();
    }
}
