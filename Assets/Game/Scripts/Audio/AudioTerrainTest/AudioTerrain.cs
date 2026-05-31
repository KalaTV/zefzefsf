using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class TerrainAmbientAudio : MonoBehaviour
{
    [Serializable]
    public class AmbientSound
    {
        public AudioClip clip;

        [Range(0f, 1f)]
        [Tooltip("Volume du son")]
        public float volume = 1f;

        [Range(0f, 1f)]
        [Tooltip("Probabilité de sélection")]
        public float randomWeight = 1f;

        [Tooltip("Autorise plusieurs instances simultanées")]
        public bool allowSimultaneous = true;

        [Min(1)]
        [Tooltip("Nombre maximum d'instances de ce son")]
        public int maxInstances = 1;
        
        [Tooltip("0 = durée réelle du clip")]
        [Min(0f)]
        public float duration = 0f;
    }
    
    private Transform player;
    private Collider terrainCollider;

    [Serializable]
    public class SoundCategory
    {
        public string categoryName = "New Category";

        public List<AmbientSound> sounds = new();
    }

    [Header("Transition Settings")]
    [SerializeField]
    private float fadeInDuration = 3f;

    [SerializeField]
    private float fadeOutDuration = 3f;

    [Header("Playback Settings")]
    [SerializeField]
    [Tooltip("Temps entre chaque tentative de lecture")]
    private float soundInterval = 5f;

    [SerializeField]
    [Min(1)]
    [Tooltip("Nombre maximum de sons simultanés")]
    private int maxSimultaneousSounds = 3;

    [Header("Categories")]
    [SerializeField]
    private List<SoundCategory> categories = new();

    private static TerrainAmbientAudio currentTerrain;

    private readonly List<AudioSource> activeSources = new();

    private bool playerInside;
    private float nextPlayTime;

    private void Awake()
    {
        terrainCollider = GetComponent<Collider>();
        terrainCollider.isTrigger = true;
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
            return;

        bool isInside = IsPlayerInsideTerrainXZ();

        if (isInside && !playerInside)
        {
            playerInside = true;
            ActivateTerrain();
        }
        else if (!isInside && playerInside)
        {
            playerInside = false;

            if (currentTerrain == this)
            {
                FadeOutAllSounds();
                currentTerrain = null;
            }
        }

        if (!playerInside)
            return;

        if (currentTerrain != this)
            return;

        if (Time.time < nextPlayTime)
            return;

        CleanupDestroyedSources();

        if (activeSources.Count < maxSimultaneousSounds)
        {
            PlayRandomSound();
        }

        nextPlayTime = Time.time + soundInterval;
    }
    
    private bool IsPlayerInsideTerrainXZ()
    {
        if (player == null)
            return false;

        Bounds bounds = terrainCollider.bounds;

        Vector3 playerPosition = player.position;

        return
            playerPosition.x >= bounds.min.x &&
            playerPosition.x <= bounds.max.x &&
            playerPosition.z >= bounds.min.z &&
            playerPosition.z <= bounds.max.z;
    }

    private void PlayRandomSound()
    {
        AmbientSound sound = GetRandomSound();

        if (sound == null)
            return;

        int existingCount = 0;

        foreach (AudioSource activeSource in activeSources)
        {
            if (activeSource == null)
                continue;

            if (activeSource.clip == sound.clip)
                existingCount++;
        }

        if (!sound.allowSimultaneous && existingCount > 0)
            return;

        if (existingCount >= sound.maxInstances)
            return;

        GameObject soundObject =
            new GameObject($"Ambient_{sound.clip.name}");

        soundObject.transform.SetParent(transform);

        AudioSource newSource =
            soundObject.AddComponent<AudioSource>();

        newSource.clip = sound.clip;
        newSource.loop = false;
        newSource.playOnAwake = false;
        newSource.spatialBlend = 0f;
        newSource.volume = 0f;

        activeSources.Add(newSource);

        StartCoroutine(
            PlaySoundRoutine(
                newSource,
                sound.volume,
                sound
            )
        );
    }

    private IEnumerator PlaySoundRoutine(
        AudioSource source,
        float targetVolume,
        AmbientSound sound
    )
    {
        if (source == null)
            yield break;

        source.Play();

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            if (source == null)
                yield break;

            timer += Time.deltaTime;

            source.volume = Mathf.Lerp(
                0f,
                targetVolume,
                timer / fadeInDuration
            );

            yield return null;
        }

        source.volume = targetVolume;

        float clipDuration = source.clip.length;

        float totalDuration =
            sound.duration > 0f
                ? sound.duration
                : clipDuration;

        float holdTime =
            Mathf.Max(
                0f,
                totalDuration
                - fadeInDuration
                - fadeOutDuration
            );

        yield return new WaitForSeconds(holdTime);

        timer = 0f;

        float startVolume = source.volume;

        while (timer < fadeOutDuration)
        {
            if (source == null)
                yield break;

            timer += Time.deltaTime;

            source.volume = Mathf.Lerp(
                startVolume,
                0f,
                timer / fadeOutDuration
            );

            yield return null;
        }

        activeSources.Remove(source);

        source.Stop();

        Destroy(source.gameObject);
    }

    private AmbientSound GetRandomSound()
    {
        List<AmbientSound> pool = new();

        foreach (SoundCategory category in categories)
        {
            if (category == null)
                continue;

            if (category.sounds == null)
                continue;

            foreach (AmbientSound sound in category.sounds)
            {
                if (sound == null)
                    continue;

                if (sound.clip == null)
                    continue;

                if (sound.randomWeight <= 0f)
                    continue;

                pool.Add(sound);
            }
        }

        if (pool.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (AmbientSound sound in pool)
        {
            totalWeight += sound.randomWeight;
        }

        float randomValue =
            UnityEngine.Random.Range(
                0f,
                totalWeight
            );

        float currentWeight = 0f;

        foreach (AmbientSound sound in pool)
        {
            currentWeight += sound.randomWeight;

            if (randomValue <= currentWeight)
                return sound;
        }

        return null;
    }

    private void FadeOutAllSounds()
    {
        List<AudioSource> sourcesCopy =
            new List<AudioSource>(activeSources);

        foreach (AudioSource activeSource in sourcesCopy)
        {
            if (activeSource == null)
                continue;

            StartCoroutine(
                FadeOutSource(activeSource)
            );
        }
    }

    private IEnumerator FadeOutSource(
        AudioSource source
    )
    {
        if (source == null)
            yield break;

        float startVolume = source.volume;

        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            if (source == null)
                yield break;

            timer += Time.deltaTime;

            float t = timer / fadeOutDuration;

            source.volume = Mathf.Lerp(
                startVolume,
                0f,
                t
            );

            yield return null;
        }

        activeSources.Remove(source);

        source.Stop();

        Destroy(source.gameObject);
    }

    private void CleanupDestroyedSources()
    {
        activeSources.RemoveAll(
            source => source == null
        );
    }

    private void ActivateTerrain()
    {
        if (currentTerrain == this)
            return;

        if (currentTerrain != null)
        {
            currentTerrain.FadeOutAllSounds();
        }

        currentTerrain = this;

        nextPlayTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        ActivateTerrain();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (currentTerrain == this)
        {
            FadeOutAllSounds();
            currentTerrain = null;
        }
    }

    private void OnDisable()
    {
        FadeOutAllSounds();
    }

    private void OnDestroy()
    {
        FadeOutAllSounds();
    }
}