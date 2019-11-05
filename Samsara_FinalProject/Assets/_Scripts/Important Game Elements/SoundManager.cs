using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    public List<AudioClip> musicSongs;
    public AudioClip level3Music;

    AudioSource mySource;

    GameController inst;

    // Use this for initialization
    void Awake()
    {
        // ensures there is only one instance of our GameObject at any given time.
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        mySource = GetComponent<AudioSource>();

        mySource.clip = musicSongs[0];

    }

    public void StopMusic()
    {
        if (mySource.isPlaying)
        {
            mySource.Pause();
        }
    }

    public void PlayMusic()
    {
        if (!mySource.isPlaying)
        {
            mySource.Play();
            mySource.volume = .5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inst == null)
        {
            inst = GameController.Instance;
        }
        else
        {
            if (inst.currentGameOptions.musicOn)
            {
                if (inst.GetCurrentGameScene() == ActiveGameScenes.Level3 && mySource.clip != level3Music)
                {
                    mySource.Stop();
                    mySource.clip = level3Music;
                }

                if (inst.GetCurrentGameScene() != ActiveGameScenes.Level3 && mySource.clip == level3Music)
                {
                    mySource.Stop();
                    mySource.clip = musicSongs[0];
                }

                if (!mySource.isPlaying)
                {
                    mySource.volume = .5f;
                    mySource.Play();
                }
            }
        }
    }
}
