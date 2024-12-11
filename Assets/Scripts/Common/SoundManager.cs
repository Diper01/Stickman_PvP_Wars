using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    public List<AudioClip> MusicList;
    public AudioClip RoundFinished;
    public AudioClip ClipButtonClick;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundsSource;

    private int currentMusicClip = 0;
    private System.Random rand;
    private bool playMusic = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();           
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {        
        GameOptions.MusicChanged += OnMusicChanged;
    }

    private void OnDisable()
    {      
        GameOptions.MusicChanged -= OnMusicChanged;
    }

    private void Update()
    {
        if (playMusic && !musicSource.isPlaying ) {
            int newMusicClip;
            do
            {
                newMusicClip = rand.Next(0, MusicList.Count);
            } while (newMusicClip != currentMusicClip);
            currentMusicClip = newMusicClip;
            musicSource.clip = MusicList[currentMusicClip];
            musicSource.Play();

        }
        
    }

    private void Init()
    {
        DontDestroyOnLoad(this.gameObject);
        rand = new System.Random();
        OnMusicChanged(GameOptions.Music);       
        currentMusicClip = rand.Next(0, MusicList.Count);
        musicSource.clip = MusicList[currentMusicClip];
        musicSource.Play();       
    }

    public void PlayerMusic(AudioClip music) {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void PlaySound(AudioClip sound) {
        if (GameOptions.Sound)
        {
            soundsSource.PlayOneShot(sound);
        }
    }

    public void PlayMusicInLoop() {
        playMusic = true;
        musicSource.loop = true;
        int newMusicClip;
        do
        {
            newMusicClip = rand.Next(0, MusicList.Count);
        } while (newMusicClip == currentMusicClip);
        currentMusicClip = newMusicClip;
        musicSource.clip = MusicList[currentMusicClip];
        musicSource.Play();
    }

    public void StopMusicInLoop() {
        musicSource.Stop();
        musicSource.loop = false;
        playMusic = false;
    }

    private void OnMusicChanged(bool value) {
        if (value)
            musicSource.mute = false;
        else
            musicSource.mute = true;
    }


}
