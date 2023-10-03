using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class OptionsMenu : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public void SetVolume(float volume)
    {
        AudioMixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
