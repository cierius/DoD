using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; } = null;

    private AudioSource musicSource;
    
    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> musicClips = new List<AudioClip>();
    

    private void Awake()
    {
        // Singleton method, only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        
    }

    public void Start()
    {
        musicSource = GetComponent<AudioSource>();
        if (musicSource.clip == null)
            musicSource.clip = musicClips[0];

        musicSource.volume = Singleton.Instance.musicVol;

        musicSource.Play();
        musicSource.loop = true;
    }


    public void SetVolume(float value) => musicSource.volume = value;

}
