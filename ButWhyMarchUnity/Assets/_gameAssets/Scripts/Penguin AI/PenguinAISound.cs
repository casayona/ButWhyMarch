using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PenguinAISound : MonoBehaviour
{
    [Header("Voice Clips")]
    public AudioClip[] penguinVoices;

    [Header("Player Reference")]
    public Transform listener;

    [Header("Distance Settings")]
    public float minDistance = 2f;
    public float maxDistance = 25f;

    [Header("Talk Timing")]
    public float minTalkDelay = 4f;
    public float maxTalkDelay = 12f;

    [Header("Volume Control")]
    public AnimationCurve volumeByDistance;

    private AudioSource audioSource;
    private float talkTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 3D ses ayarý
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Custom;

        ScheduleNextTalk();
    }

    void Update()
    {
        UpdateVolumeByDistance();
        HandleTalking();
    }

    void HandleTalking()
    {
        talkTimer -= Time.deltaTime;

        if (talkTimer <= 0)
        {
            PlayVoice();
            ScheduleNextTalk();
        }
    }

    void PlayVoice()
    {
        if (penguinVoices.Length == 0) return;

        AudioClip clip = penguinVoices[Random.Range(0, penguinVoices.Length)];

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }

    void ScheduleNextTalk()
    {
        float distance = Vector3.Distance(transform.position, listener.position);

        // uzaktaki penguen daha az konuþur
        float distanceFactor = Mathf.InverseLerp(maxDistance, minDistance, distance);
        float delay = Mathf.Lerp(maxTalkDelay, minTalkDelay, distanceFactor);

        talkTimer = delay + Random.Range(-1f, 1f);
    }

    void UpdateVolumeByDistance()
    {
        float distance = Vector3.Distance(transform.position, listener.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        audioSource.volume = volumeByDistance.Evaluate(1f - t);
    }
}