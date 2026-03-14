using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

/// <summary>
/// Ana ses yöneticisi: tüm çevresel ses sistemini kontrol eder.
/// </summary>
public class AdvancedAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource windLoop;
    public AudioSource darkAmbienceLoop;
    public AudioSource randomEventSource;

    [Header("Wind Settings")]
    [Range(0f, 1f)]
    public float stormIntensity = 0.3f;
    public float windChangeSpeed = 0.2f;

    [Header("Random Ice Events")]
    public AudioClip[] iceCracks;
    public AudioClip[] distantRumbles;
    public float minEventDelay = 8f;
    public float maxEventDelay = 25f;

    [Header("Atmosphere")]
    public AnimationCurve windVolumeCurve;
    public AnimationCurve ambienceVolumeCurve;

    float windTarget;
    float eventTimer;

    void Start()
    {
        windLoop.loop = true;
        darkAmbienceLoop.loop = true;

        windLoop.Play();
        darkAmbienceLoop.Play();

        ScheduleNextEvent();
    }

    void Update()
    {
        UpdateWind();
        UpdateAmbience();
        HandleRandomEvents();
    }

    void UpdateWind()
    {
        windTarget = Mathf.Lerp(0.2f, 1f, stormIntensity);

        windLoop.volume = Mathf.Lerp(
            windLoop.volume,
            windVolumeCurve.Evaluate(windTarget),
            Time.deltaTime * windChangeSpeed
        );

        windLoop.pitch = Mathf.Lerp(0.8f, 1.2f, stormIntensity);
    }

    void UpdateAmbience()
    {
        float target = ambienceVolumeCurve.Evaluate(stormIntensity);
        darkAmbienceLoop.volume = Mathf.Lerp(
            darkAmbienceLoop.volume,
            target,
            Time.deltaTime * 0.5f
        );
    }

    void HandleRandomEvents()
    {
        eventTimer -= Time.deltaTime;

        if (eventTimer <= 0)
        {
            PlayRandomEvent();
            ScheduleNextEvent();
        }
    }

    void PlayRandomEvent()
    {
        AudioClip clip;

        if (Random.value > 0.5f)
            clip = iceCracks[Random.Range(0, iceCracks.Length)];
        else
            clip = distantRumbles[Random.Range(0, distantRumbles.Length)];

        randomEventSource.pitch = Random.Range(0.85f, 1.1f);
        randomEventSource.volume = Random.Range(0.4f, 0.8f);
        randomEventSource.PlayOneShot(clip);
    }

    void ScheduleNextEvent()
    {
        eventTimer = Random.Range(minEventDelay, maxEventDelay);
    }
}
