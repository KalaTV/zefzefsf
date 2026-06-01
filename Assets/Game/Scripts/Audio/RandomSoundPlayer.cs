using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
public class RandomZoneAudioPlayer : MonoBehaviour
{
    [Serializable]
    public class Sound
    {
        [Tooltip("AudioSource utilisé comme template")]
        public AudioSource source;

        [Range(0f, 1f)]
        public float randomWeight = 1f;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Tooltip("Durée du son (0 = durée du clip)")]
        public float duration = 5f;

        public float fadeInDuration = 1f;
        public float fadeOutDuration = 2f;

        public bool playOnEnter = false;
    }

    [Serializable]
    public class SoundCategory
    {
        public string categoryName = "New Category";
        public List<Sound> sounds = new();
    }

    [Header("Zone Settings")]
    [SerializeField] private Vector3 zoneSize = new(10f, 5f, 10f);

    [Header("Playback Settings")]
    [SerializeField] private float soundInterval = 4f;
    [SerializeField] private bool autoPlay = true;

    [Header("Categories")]
    [SerializeField] private List<SoundCategory> categories = new();

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private AudioSource audioSource;
    private BoxCollider boxCollider;

    private float nextPlayTime;
    private bool playerInside;
    private bool isTransitioning;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        boxCollider.isTrigger = true;
        boxCollider.size = zoneSize;

        nextPlayTime = Time.time + soundInterval;
    }

    private void OnValidate()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        boxCollider.isTrigger = true;
        boxCollider.size = zoneSize;
    }

    private void Update()
    {
        if (!autoPlay || !playerInside || isTransitioning)
            return;

        if (audioSource.isPlaying)
            return;

        if (Time.time < nextPlayTime)
            return;

        var sound = GetRandomWeightedSound();
        if (sound == null) return;

        PlaySound(sound);

        nextPlayTime = Time.time + soundInterval;
    }

    private void PlaySound(Sound sound)
    {
        if (sound == null || sound.source == null || sound.source.clip == null)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(PlayRoutine(sound));
    }

    private IEnumerator PlayRoutine(Sound sound)
    {
        isTransitioning = true;

        audioSource.Stop();

        CopyAudioSource(sound.source, audioSource);

        audioSource.volume = 0f;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        audioSource.Play();

        float t = 0f;

        while (t < sound.fadeInDuration)
        {
            t += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(
                0f,
                sound.volume,
                t / sound.fadeInDuration
            );

            yield return null;
        }

        audioSource.volume = sound.volume;

        float clipDuration =
            audioSource.clip != null
                ? audioSource.clip.length
                : 0f;

        float totalDuration =
            sound.duration > 0f
                ? sound.duration
                : clipDuration;

        float playTime =
            Mathf.Max(
                0f,
                totalDuration - sound.fadeOutDuration
            );

        yield return new WaitForSeconds(playTime);

        t = 0f;
        float start = audioSource.volume;

        while (t < sound.fadeOutDuration)
        {
            t += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(
                start,
                0f,
                t / sound.fadeOutDuration
            );

            yield return null;
        }

        audioSource.Stop();
        isTransitioning = false;
    }

    private void CopyAudioSource(AudioSource original, AudioSource target)
    {
        target.clip = original.clip;
        target.outputAudioMixerGroup = original.outputAudioMixerGroup;
        target.mute = original.mute;
        target.bypassEffects = original.bypassEffects;
        target.bypassListenerEffects = original.bypassListenerEffects;
        target.bypassReverbZones = original.bypassReverbZones;
        target.priority = original.priority;
        target.pitch = original.pitch;
        target.panStereo = original.panStereo;
        target.spatialBlend = 0f;
        target.reverbZoneMix = original.reverbZoneMix;
        target.loop = original.loop;
        target.dopplerLevel = original.dopplerLevel;
        target.spread = original.spread;
        target.rolloffMode = original.rolloffMode;
        target.minDistance = original.minDistance;
        target.maxDistance = original.maxDistance;
        target.ignoreListenerPause = original.ignoreListenerPause;
        target.ignoreListenerVolume = original.ignoreListenerVolume;
        target.velocityUpdateMode = original.velocityUpdateMode;
    }

    private Sound GetRandomWeightedSound()
    {
        List<Sound> pool = new();

        foreach (var cat in categories)
        {
            if (cat?.sounds == null) continue;

            foreach (var s in cat.sounds)
            {
                if (s?.source == null || s.source.clip == null)
                    continue;

                if (s.randomWeight <= 0f)
                    continue;

                pool.Add(s);
            }
        }

        if (pool.Count == 0)
            return null;

        float total = 0f;
        foreach (var s in pool)
            total += s.randomWeight;

        float r = UnityEngine.Random.Range(0f, total);

        float acc = 0f;
        foreach (var s in pool)
        {
            acc += s.randomWeight;
            if (r <= acc)
                return s;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        StopInstant();
    }

    public void StopInstant()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        audioSource.Stop();
        isTransitioning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, zoneSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, zoneSize);
    }
}