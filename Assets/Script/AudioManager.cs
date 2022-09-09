using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    // Start is called before the first frame update

    [SerializeField] AudioEntry[] audioLibrary;

    [SerializeField] AudioSource globalAudioSource;
    [SerializeField] AudioSource loopingAudioSource;

    public float globalAudioVolume
    {
        get => globalAudioSource.volume;
        set => globalAudioSource.volume = value;
    }

    public float loopingAudioVolume
    {
        get => loopingAudioSource.volume;
        set => loopingAudioSource.volume = value;
    }

    Dictionary<string, AudioEntry> audioDict;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        InitLibrary();
    }

    private void Start()
    {
        PlayAudioLoop("bgMusic");
    }

    void InitLibrary()
    {
        audioDict = new Dictionary<string, AudioEntry>(audioLibrary.Length);
        foreach (AudioEntry entry in audioLibrary)
        {
            Debug.Assert(!audioDict.ContainsKey(entry.name));

            audioDict[entry.name] = entry;
        }
    }

    public void PlayAudio(string name)
    {
        if (audioDict.TryGetValue(name, out AudioEntry audioEntry))
        {
            globalAudioSource.clip = audioEntry.audioClip;
            globalAudioSource.Play();
        }
        else
        {
            Debug.LogError($"PlayAudio: Could not find audio clip '{name}' ");
        }
    }

    public void PlayAudioLoop(string name)
    {
        if (audioDict.TryGetValue(name, out AudioEntry audioEntry))
        {
            loopingAudioSource.Stop();
            loopingAudioSource.loop = true;
            loopingAudioSource.clip = audioEntry.audioClip;
            loopingAudioSource.Play();
        }
        else
        {
            Debug.LogError($"PlayAudioLoop: Could not find audio clip '{name}' ");
        }
    }

    public AudioClip GetAudioClip(string name)
    {
        if (audioDict.TryGetValue(name, out AudioEntry audioEntry))
        {
            return audioEntry.audioClip;
        }
        else
        {
            Debug.LogError($"GetAudioClip: Could not find audio clip '{name}' ");
            return null;
        }
    }


        public void StopAudioLoop()
    {
        loopingAudioSource.Stop();
    }
}

[System.Serializable]
public class AudioEntry
{
    public string name;
    public AudioClip audioClip;
}

