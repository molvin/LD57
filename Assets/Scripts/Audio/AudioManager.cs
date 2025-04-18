using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    private Queue<AudioSource> m_PooledAudio = new Queue<AudioSource>();
    private Dictionary<AudioEventData, List<int>> m_AvailabeIndexes = new Dictionary<AudioEventData, List<int>>();
    private static AudioSource GetAudioSource {
        get { 
            if(Instance.m_PooledAudio.Count > 0)
            {
                var  audioSource = Instance.m_PooledAudio.Dequeue();
                if (audioSource == null)
                    return GetAudioSource;

                audioSource.gameObject.SetActive(true);
                return audioSource;
            }
            else
            {
                var audioSourceObject = new GameObject("PooledAudio");
                return audioSourceObject.AddComponent<AudioSource>();
            }
        }
}
    private static AudioManager m_Instance;
    private static AudioManager Instance
    {
        get
        {
            if(m_Instance != null)
                return m_Instance;

            var newGameObject = new GameObject("AudioManager");
            m_Instance = newGameObject.AddComponent<AudioManager>();
            DontDestroyOnLoad(m_Instance);
            return m_Instance;
        }
    }

    public static AudioSource PlayAudio(AudioEventData audioEvent, Transform attachPoint = null)
    {
        AudioSource audioSource = null;
        if (audioEvent == null)
            return audioSource;

        audioSource = GetAudioSource;

        Instance.PopulateAudioSource(audioSource, audioEvent, attachPoint);

        return audioSource;
    }

    public static void StopAudio(AudioSource audioSource, AudioEventData audioData)
    {
        Instance.FadeOut(audioSource, audioData.m_FadeInAndOutTime);
    }
    async void FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        float timer = fadeTime;
        while(audioSource != null && timer > 0)
        {
            timer -= Time.deltaTime;
            audioSource.volume = startVolume * (timer / fadeTime);

            await Task.Yield();
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            ReturnAudioSource(audioSource);
            audioSource.mute = true;
        }
    }
    public static AudioSource PlayAudio(AudioClip clip, Transform attachPoint = null)
    {
        AudioSource audioSource = null;

        audioSource = GetAudioSource;

        audioSource.clip = clip;
        audioSource.Play();
        Instance.WaitForCompletion(audioSource, -1, (x) => { x.transform.parent = null; Instance.m_PooledAudio.Enqueue(x); });

        return audioSource;
    }

    private void PopulateAudioSource(AudioSource audioSource, AudioEventData audioEvent, Transform attachPoint = null)
    {
        if (audioSource == null || audioEvent.m_Clips.Count == 0)
            return;

        AudioClip clip = null;
        if(audioEvent.m_Clips.Count == 1)
        {
            clip = audioEvent.m_Clips[0];
        }
        else
        {
            if (!m_AvailabeIndexes.ContainsKey(audioEvent))
            {
                var clips = new List<int>(audioEvent.m_Clips.Count);
                for (int i = 0; i < audioEvent.m_Clips.Count; i++)
                    clips.Add(i);

                clips = clips.OrderBy(x => Random.value).ToList();
                clip = audioEvent.m_Clips[clips[0]];
                clips.RemoveAt(0);
                m_AvailabeIndexes[audioEvent] = clips;
            }
            else
            {
                var clips = m_AvailabeIndexes[audioEvent];
                clip = audioEvent.m_Clips[clips[0]];
                clips.RemoveAt(0);
                if(clips.Count == 0)
                    m_AvailabeIndexes.Remove(audioEvent);
            }
        }
        audioSource.mute = false;
        audioSource.clip = clip;
        audioSource.pitch = 1 + Random.Range(-audioEvent.m_PitchRange, audioEvent.m_PitchRange);
        audioSource.volume = audioEvent.m_Volume + Random.Range(-audioEvent.m_AmplitudeRange, audioEvent.m_AmplitudeRange);
        audioSource.loop = audioEvent.m_IsLooping;
        audioSource.outputAudioMixerGroup = audioEvent.m_MixerGroup;
        if(attachPoint != null)
        {
            audioSource.transform.parent = attachPoint;
            audioSource.transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.parent = Camera.main.transform;
            transform.localPosition = Vector3.zero;
            if(audioEvent.m_DontDestroyOnLoad)
                DontDestroyOnLoad(audioSource);
        }


        WaitForCompletion(audioSource, audioEvent.m_FadeInAndOutTime, ReturnAudioSource);
    }

    private void ReturnAudioSource(AudioSource audio)
    {
        audio.transform.parent = null; 
        m_PooledAudio.Enqueue(audio); 
        audio.gameObject.SetActive(false);
    }

    async void WaitForCompletion(AudioSource audioSource, float fadeInOutTime ,Action<AudioSource> onStop)
    {
        bool doesFade = fadeInOutTime > 0;
        float targetVolume = audioSource.volume;
        float lerpValue = doesFade ? 0 : 1;
        float timer = 0;
        if (doesFade)
            audioSource.volume = 0;

        audioSource.Play();
        
        while (audioSource != null && (audioSource.isPlaying || !audioSource.mute))
        {
            if(doesFade && timer <= fadeInOutTime)
            {
                timer += Time.deltaTime;
                lerpValue = timer / fadeInOutTime;

                audioSource.volume = lerpValue * targetVolume;
            }


            await Task.Yield();
        }

        if (audioSource != null)
            onStop?.Invoke(audioSource);
    }

}
