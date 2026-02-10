using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Main Soundtrack")]
    public AudioClip mainTrack; // The first track that always plays

    [Header("Random Soundtracks")]
    public AudioClip[] randomTracks; // Pool of random tracks to play after main

    [Header("Settings")]
    public bool shuffleMode = true; // If true, tracks won't repeat until all are played
    public float fadeDuration = 2f; // Fade between tracks

    private List<AudioClip> remainingTracks = new List<AudioClip>();
    private bool hasPlayedMainTrack = false;

    void Start()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        // Start with the main track
        if (mainTrack != null)
        {
            PlayMainTrack();
        }
        else
        {
            Debug.LogWarning("No main track assigned!");
            PlayRandomTrack();
        }
    }

    void Update()
    {
        // Check if current track has finished
        if (!musicSource.isPlaying && hasPlayedMainTrack)
        {
            PlayRandomTrack();
        }
        else if (!musicSource.isPlaying && !hasPlayedMainTrack)
        {
            hasPlayedMainTrack = true;
            PlayRandomTrack();
        }
    }

    void PlayMainTrack()
    {
        musicSource.clip = mainTrack;
        musicSource.Play();
        Debug.Log("Playing main track: " + mainTrack.name);
    }

    void PlayRandomTrack()
    {
        if (randomTracks.Length == 0)
        {
            Debug.LogWarning("No random tracks assigned!");
            return;
        }

        // Refill the pool if empty (for shuffle mode)
        if (shuffleMode && remainingTracks.Count == 0)
        {
            remainingTracks.AddRange(randomTracks);
        }

        AudioClip nextTrack;

        if (shuffleMode)
        {
            // Pick random from remaining tracks
            int randomIndex = Random.Range(0, remainingTracks.Count);
            nextTrack = remainingTracks[randomIndex];
            remainingTracks.RemoveAt(randomIndex);
        }
        else
        {
            // Completely random (can repeat)
            nextTrack = randomTracks[Random.Range(0, randomTracks.Length)];
        }

        StartCoroutine(CrossfadeToTrack(nextTrack));
    }

    IEnumerator CrossfadeToTrack(AudioClip newTrack)
    {
        // Fade out current track
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;

        // Switch to new track
        musicSource.clip = newTrack;
        musicSource.Play();
        Debug.Log("Now playing: " + newTrack.name);

        // Fade in new track
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}