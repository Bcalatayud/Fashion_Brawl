using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip[] sfxCollection;


    void Awake()
    {
        instance = this;        
    }

    void Start ()
    {
        GameManager.instance.ChangeStateEvent += GameStateChange;	
}

    void GameStateChange()
    {
		
        switch (GameManager.instance.currentState)
        {
            case GameState.PLAYING:                
                musicSource.Play();
                break;
			case GameState.PAUSE:
				musicSource.mute = false;
                break;
            case GameState.CONTINUE:
                musicSource.mute = false;
                break;
            case GameState.GAME_OVER:
                musicSource.Stop();
                break;
        }
        
    }

	public void PlayShot(int a)
    {
        sfxSource.PlayOneShot(sfxCollection[a]);
    }

    public void PlayWave()
    {
        sfxSource.PlayOneShot(sfxCollection[1]);
    }
		
	public void Volume(float volume)
	{
		musicSource.volume = volume;
	}
}
