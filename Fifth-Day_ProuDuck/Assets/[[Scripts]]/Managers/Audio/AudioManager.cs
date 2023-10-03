using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] Sounds;

    public AudioMixerGroup MainMixer;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        

        foreach (Sound sound in Sounds)
        {
            sound.AudioSource = gameObject.AddComponent<AudioSource>();
            sound.AudioSource.clip = sound.Clip;
            sound.AudioSource.volume = sound.Volume;
            sound.AudioSource.pitch = sound.Pitch;
            sound.AudioSource.loop = sound.LoopTrack;
            sound.AudioSource.outputAudioMixerGroup = MainMixer;
        }

        Play("MainMenu_BGM");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }

        s.AudioSource.Play();
    }

    public void PlayPeriodically(string name, float timeBetweenPlay)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }

        StartCoroutine(PlayPeriodically_Coroutine(s, timeBetweenPlay));
    }

    IEnumerator PlayPeriodically_Coroutine(Sound s, float timeBetweenPlay)
    {
        s.AudioSource.PlayOneShot(s.Clip);
        yield return new WaitForSeconds(timeBetweenPlay);
        StartCoroutine(PlayPeriodically_Coroutine(s, timeBetweenPlay));
    }

    public void FadeIn(string trackToFadeIn, float fadeDuration = 1.0f, float volumeLevel = 0.5f)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == trackToFadeIn);

        if (s == null)
        {
            Debug.LogWarning($"Sound: {trackToFadeIn} not found!");
            return;
        }

        s.AudioSource.Play();

        StartCoroutine(FadeIn_Coroutine(s, fadeDuration, volumeLevel));
    }

    private IEnumerator FadeIn_Coroutine(Sound s, float fadeDuration, float volumeLevel)
    {
        float time = 0.0f;

        while(time < fadeDuration)
        {
            float tValue = time / fadeDuration;
            s.AudioSource.volume = tValue - volumeLevel;
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeOut(string trackToFadeOut, float fadeDuration = 1.0f, float volumeLevel = 1.0f)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == trackToFadeOut);

        if (s == null)
        {
            Debug.LogWarning($"Sound: {trackToFadeOut} not found!");
            return;
        }

        StartCoroutine(FadeOut_Coroutine(s, fadeDuration, volumeLevel));
    }

    private IEnumerator FadeOut_Coroutine(Sound s, float fadeDuration, float volumeLevel)
    {
        float time = 0.0f;
        while(time < fadeDuration)
        {
            float tValue = time / fadeDuration;

            s.AudioSource.volume = volumeLevel - tValue;

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        s.AudioSource.Stop();
        s.AudioSource.volume = 1.0f;
    }

    public void CrossFade(string trackOne, string trackTwo, float transitionDuration = 5.0f, float volumeLevel = 0.5f)
    {
        Sound oldTrack = Array.Find(Sounds, oldTrack => oldTrack.Name == trackOne);
        Sound newTrack = Array.Find(Sounds, newTrack => newTrack.Name == trackTwo);

        if (oldTrack == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        else if(newTrack == null)
        {
            Debug.LogWarning($"Sound: {newTrack} not found!");
            return;
        }

        newTrack.AudioSource.Play();

        StartCoroutine(CrossFade_Coroutine(oldTrack, newTrack, transitionDuration, volumeLevel));
    }

    private IEnumerator CrossFade_Coroutine(Sound oldTrack, Sound newTrack, float transitionDuration, float volumeLevel)
    {
        float time = 0.0f;

        while(time < transitionDuration)
        {
            float tValue = time / transitionDuration;
            newTrack.AudioSource.volume = tValue - volumeLevel;
            oldTrack.AudioSource.volume = volumeLevel - tValue;

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        oldTrack.AudioSource.Stop();
        oldTrack.AudioSource.volume = 1.0f;
    }
}
