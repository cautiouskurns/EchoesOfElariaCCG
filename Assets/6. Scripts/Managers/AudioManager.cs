using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum AudioType
    {
        UI,
        CardEffects,
        Combat,
        Music
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioType type;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] musicSources;  // Array for crossfading
    [SerializeField] private AudioSource[] sfxSources;    // Pool of sources for multiple SFX

    [Header("Audio Settings")]
    [SerializeField] private float musicFadeTime = 1f;
    [SerializeField] private int sfxSourcesCount = 5;

    [Header("Audio Collections")]
    [SerializeField] private Sound[] cardSounds;    // Card-related sounds
    [SerializeField] private Sound[] combatSounds;  // Combat effects
    [SerializeField] private Sound[] uiSounds;      // UI feedback
    [SerializeField] private Sound[] musicTracks;   // Background music

    [Header("Audio Mixer")]
    [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup musicGroup;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup sfxGroup;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup uiGroup;

    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private int currentMusicSourceIndex = 0;
    private bool isFading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null; // Ensure it's a root GameObject
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            InitializeAudioDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("[AudioManager] 🎵 Attempting to start music...");
        
        // Check if we have any music tracks
        if (musicTracks != null && musicTracks.Length > 0 && musicTracks[0].clip != null)
        {
            PlayMusic(musicTracks[0].name, fadeIn: false);
            Debug.Log($"[AudioManager] 🎵 Playing track: {musicTracks[0].name}");
        }
        else
        {
            Debug.LogError("[AudioManager] ❌ No music tracks assigned!");
        }

        // Add test keys for manual testing
        Debug.Log("[AudioManager] Test controls: M = Music, S = SFX");
    }

    private void InitializeAudioSources()
    {
        // Create music sources for crossfading
        musicSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            musicSources[i] = gameObject.AddComponent<AudioSource>();
            musicSources[i].playOnAwake = false;
            musicSources[i].outputAudioMixerGroup = musicGroup; // Assign mixer group
        }

        // Create SFX source pool
        sfxSources = new AudioSource[sfxSourcesCount];
        for (int i = 0; i < sfxSourcesCount; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].outputAudioMixerGroup = sfxGroup; // Assign mixer group
        }

        // Only set mixer values if they exist
        if (audioMixer != null)
        {
            try
            {
                audioMixer.SetFloat("MasterVolume", 0f);
                audioMixer.SetFloat("BGMVolume", 0f);
                audioMixer.SetFloat("SFXVolume", 0f);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[AudioManager] Failed to set mixer values: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("[AudioManager] Audio Mixer not assigned!");
        }
    }

    private void InitializeAudioDictionary()
    {
        AddSoundsToDictionary(cardSounds);
        AddSoundsToDictionary(combatSounds);
        AddSoundsToDictionary(uiSounds);
        AddSoundsToDictionary(musicTracks);
    }

    private void AddSoundsToDictionary(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            if (!soundDictionary.ContainsKey(s.name))
            {
                soundDictionary.Add(s.name, s);
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (!soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            Debug.LogWarning($"[AudioManager] ❌ Sound not found: {soundName}");
            return;
        }

        // Find available SFX source
        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.PlayOneShot(sound.clip); // Changed to PlayOneShot
            Debug.Log($"[AudioManager] 🔊 Playing: {soundName}, Volume: {sound.volume}");
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }
        Debug.LogWarning("[AudioManager] No available audio sources!");
        return null;
    }

    public void PlayMusic(string trackName, bool fadeIn = true)
    {
        if (!soundDictionary.TryGetValue(trackName, out Sound music)) return;

        if (fadeIn && !isFading)
        {
            StartCoroutine(CrossFadeMusic(music));
        }
        else
        {
            AudioSource currentSource = musicSources[currentMusicSourceIndex];
            currentSource.clip = music.clip;
            currentSource.volume = music.volume;
            currentSource.loop = true;
            currentSource.Play();
        }
    }

    private System.Collections.IEnumerator CrossFadeMusic(Sound newMusic)
    {
        isFading = true;
        
        int nextSourceIndex = 1 - currentMusicSourceIndex;
        AudioSource fadeOut = musicSources[currentMusicSourceIndex];
        AudioSource fadeIn = musicSources[nextSourceIndex];

        fadeIn.clip = newMusic.clip;
        fadeIn.loop = true;
        fadeIn.volume = 0;
        fadeIn.Play();

        float timer = 0;
        while (timer < musicFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / musicFadeTime;
            fadeIn.volume = Mathf.Lerp(0, newMusic.volume, t);
            if (fadeOut.isPlaying)
                fadeOut.volume = Mathf.Lerp(fadeOut.volume, 0, t);
            yield return null;
        }

        fadeOut.Stop();
        currentMusicSourceIndex = nextSourceIndex;
        isFading = false;
    }

    // Add test method (remove after testing)
    private void Update()
    {
        // Test controls
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (musicTracks != null && musicTracks.Length > 0)
            {
                PlayMusic(musicTracks[0].name);
                Debug.Log($"[AudioManager] 🎵 Manually playing music: {musicTracks[0].name}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (cardSounds != null && cardSounds.Length > 0)
            {
                PlaySound(cardSounds[0].name);
                Debug.Log($"[AudioManager] 🔊 Testing sound: {cardSounds[0].name}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound("CardDraw");
            Debug.Log("Attempted to play CardDraw sound");
        }
    }

    // Add validation method
    private void OnValidate()
    {
        // Ensure we have default sounds
        if (cardSounds == null || cardSounds.Length == 0)
        {
            cardSounds = new Sound[]
            {
                new Sound { name = CardSoundConfig.CARD_DRAW, type = AudioType.CardEffects },
                new Sound { name = CardSoundConfig.CARD_PLAY, type = AudioType.CardEffects },
                new Sound { name = CardSoundConfig.ATTACK_SLASH, type = AudioType.CardEffects },
                new Sound { name = CardSoundConfig.FIREBALL, type = AudioType.CardEffects },
                new Sound { name = CardSoundConfig.HEAL, type = AudioType.CardEffects }
            };
        }
    }
}
