using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    static AudioManager instance_;
    public static AudioManager instance() => instance_;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource; // loop = true, playOnAwake = false
    [SerializeField] private AudioSource sfxSource;   // loop = false, playOnAwake = false
    [SerializeField] private AudioSource delayedSfxSource;
    private readonly Queue<AudioClip> queuedSfx = new Queue<AudioClip>();

    [Header("Clips")]
    public AudioClip hurtClip;

    public AudioClip swingClip;
    public AudioClip punchConnectClip;
    public AudioClip punchCrunchClip;
    public AudioClip explosionClip;
    public AudioClip defaultMusic;

    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private float lastHurtTime = -1f;
    private const float hurtCooldown = 0.15f; // avoid stacking hurt sounds from multi-hit frames                                                 

    void Awake()
    {
        if (instance_ != null && instance_ != this)
        {
            Destroy(gameObject);   // a manager from an earlier scene already exists                                                              
            return;
        }
        instance_ = this;
        DontDestroyOnLoad(gameObject);
        musicSource.volume = musicVolume;
        if (defaultMusic != null) PlayMusic(defaultMusic);
    }
    private void Update()
    {
        
    }
    
    // Same clip already playing -> keep going (continuous). Different clip -> swap.                                                              
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float pitchVariance = 0f, float delay = 0f)
    {
        if (clip == null) return;
        
        sfxSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariance, pitchVariance);
        if(delay > 0f)
        {
            Debug.Log("Playing SFX with delay: " + clip.name + " after " + delay + " seconds");
            delayedSfxSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariance, pitchVariance);
            delayedSfxSource.clip = clip;
            delayedSfxSource.PlayDelayed(delay);
        }
        else 
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
            
    }

    public void PlayHurt(float delay = 0f)
    {
        if (Time.time - lastHurtTime < hurtCooldown) return;
        lastHurtTime = Time.time;
        PlaySFX(hurtClip, 0.05f, delay);
    }

    public void PlayPunchConnect() => PlaySFX(punchConnectClip, 0.1f); // slight pitch variation keeps repeats from sounding robotic
    public void PlayPunchCrush() => PlaySFX(punchCrunchClip, 0.1f); // slight pitch variation keeps repeats from sounding robotic
    public void PlaySwing() => PlaySFX(swingClip, 0.1f); // slight pitch variation keeps repeats from sounding robotic
    public void PlayExplosion() => PlaySFX(explosionClip, 0.1f); // slight pitch variation keeps repeats from sounding robotic
}
