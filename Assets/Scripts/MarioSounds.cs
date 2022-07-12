using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MarioSounds : MonoBehaviour
{
    //Mario Sounds and effects

    [SerializeField] [Range(0, 1)] private float voiceVolume;
    [SerializeField] [Range(0, 1)] private float feetVolume;
    [SerializeField] [Range(0, 1)] private float slideVolume;

    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource feetSource;
    [SerializeField] private AudioSource slideSource;

    [SerializeField] private AudioFile[] audioFiles;

    #region SETTERS
    public void SetVoiceVolume(float volume)
    {
        voiceVolume = volume;
    }
    public void SetFeetVolume(float volume)
    {
        feetVolume = volume;
    }
    public void SetSlideVolume(float volume)
    {
        slideVolume = volume;
        slideSource.volume = volume;
    }
    #endregion


    #region Play Sound Functions
    public void PlayVoice(string soundName)
    {
        AudioFile file;
        try
        {
            file = GetFileByName(soundName);
        }
        catch
        {
            file = null;
        }

        if(file != null)
        {

            var clip = file.Clip[Random.Range(0, file.Clip.Length - 1)];
            voiceSource.volume = file.Volume * voiceVolume;
            if(clip!= null)
            {
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }
    }

    public void PlayFeet(string soundName)
    {
        AudioFile file;
        try
        {
            file = GetFileByName(soundName);
        }
        catch
        {
            file = null;
        }

        if (file != null)
        {
            var clip = file.Clip[Random.Range(0, file.Clip.Length - 1)];
            feetSource.volume = file.Volume * feetVolume;
            if (clip != null)
            {
                feetSource.clip = clip;
                feetSource.Play();
            }
        }
    }

    public void PlaySlide(string soundName)
    {
        AudioFile file;
        try
        {
            file = GetFileByName(soundName);
        }
        catch
        {
            file = null;
        }

        if (file != null)
        {
            var clip = file.Clip[Random.Range(0, file.Clip.Length - 1)];
            slideSource.volume = file.Volume * slideVolume;
            if (clip != null)
            {
                slideSource.clip = clip;
                slideSource.Play();
            }
        }
    }
    #endregion


    #region Called Functions
    public void Step()
    {
        PlayFeet("Step");
    }
    public void Jump1()
    {
        PlayVoice("Jump1");
    }
    public void Jump2()
    {
        PlayVoice("Jump2");
    }
    public void Jump3()
    {
        PlayVoice("Jump3");
    }
    public void WallJump()
    {
        PlayVoice("WallJump");
    }
    public void LongJump()
    {
        PlayVoice("LongJump");
    }
    public void Land()
    {
        PlayVoice("Jump");
    }
    public void StartSlide()
    {
        //SetSlideVolume(1);
        PlaySlide("Slide");
        
    }
    public void EndSlide()
    {
        //Debug.Log("stop");
        slideSource.Stop();
        slideSource.clip = null;
    }
    public void ReciveDamage()
    {
        PlayVoice("Hit");
    }
    public void Die()
    {
        PlayVoice("Die");
    }
    public void Punch1()
    {
        PlayVoice("Punch1");
    }
    public void Punch2()
    {
        PlayVoice("Punch2");
    }
    public void Punch3()
    {
        PlayVoice("Punch3");
    }

    public void StartSleep()
    {
        SetSlideVolume(1);
        PlaySlide("Sleep");
    }

    public void EndSleep()
    {

        slideSource.clip = null ;
        PlayVoice("Awake");
    }


    #endregion

    private AudioFile GetFileByName(string name)
    {
        AudioFile file;
        try
        {
            file = audioFiles.First(x => x.Name == name);
        }
        catch
        {
            return null;
        }
        return file;
        
    }


}


[System.Serializable]
public class AudioFile
{
    public string Name;
    public AudioClip[] Clip;
    [Range (0,1)]
    public float Volume;
}


