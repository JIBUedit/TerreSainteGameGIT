using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSequenceController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Référence au VideoPlayer.
    public VideoClip[] videoClips; // Tableau des vidéos à enchaîner.
    public AudioClip[] videoSounds; // Tableau des sons associés aux vidéos.
    public AudioSource musicSource; // AudioSource pour la musique de fond.
    public AudioSource soundEffectSource; // AudioSource pour les sons des vidéos.

    private AudioSource[] sceneAudioSources; // Tous les AudioSource de la scène.
    private int currentVideoIndex = 0; // Index de la vidéo actuelle.
    private bool isVideoPausedAtEnd = false; // Indique si la vidéo est figée à la fin.

    void OnEnable()
    {
        // Interrompt tous les sons de la scène.
        MuteSceneAudio(true);

        // Démarre la musique lorsque le GameObject s'active.
        if (musicSource != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play(); // Force la lecture de la musique.
            }
            else
            {
                Debug.LogWarning("MusicSource est déjà en lecture.");
            }
        }
    }

    void OnDisable()
    {
        // Réactive tous les sons de la scène.
        MuteSceneAudio(false);

        // Arrête la musique lorsque le GameObject se désactive.
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    void Start()
    {
        if (videoClips.Length > 0 && videoPlayer != null)
        {
            PlayVideo(currentVideoIndex); // Lance la première vidéo.
            videoPlayer.loopPointReached += OnVideoEnd; // Abonnez-vous à l’événement de fin.
        }
        else
        {
            Debug.LogError("Aucune vidéo assignée ou VideoPlayer manquant.");
        }
    }

    void Update()
    {
        if (!isVideoPausedAtEnd)
        {
            // Accélération de la vidéo si une touche est enfoncée.
            if (Input.anyKey)
            {
                videoPlayer.playbackSpeed = 4.0f; // Vitesse x4.
            }
            else
            {
                videoPlayer.playbackSpeed = 1.0f; // Retour à la vitesse normale.
            }
        }

        // Vérifie les interactions utilisateur pour avancer après une pause.
        if (isVideoPausedAtEnd && Input.anyKeyDown)
        {
            if (currentVideoIndex < videoClips.Length - 1)
            {
                ProceedToNextVideo();
            }
            else
            {
                // Désactive le GameObject après la dernière vidéo.
                gameObject.SetActive(false);
            }
        }
    }

    private void PlayVideo(int index)
    {
        if (index >= 0 && index < videoClips.Length)
        {
            // Arrête le son de la vidéo précédente (si nécessaire).
            if (soundEffectSource != null && soundEffectSource.isPlaying)
            {
                soundEffectSource.Stop();
            }

            videoPlayer.clip = videoClips[index]; // Charge la vidéo correspondante.
            videoPlayer.playbackSpeed = 1.0f; // S'assure que la vitesse initiale est normale.
            videoPlayer.Play(); // Joue la vidéo.

            // Joue le son associé à cette vidéo.
            if (videoSounds != null && index < videoSounds.Length && soundEffectSource != null)
            {
                soundEffectSource.clip = videoSounds[index];
                soundEffectSource.Play(); // Joue le son correspondant.
            }

            isVideoPausedAtEnd = false; // Réinitialise l'état.
        }
        else
        {
            Debug.LogWarning("Index de vidéo invalide.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log($"Fin de la vidéo : {vp.clip.name}");
        if (currentVideoIndex < videoClips.Length - 1)
        {
            // Si ce n'est pas la dernière vidéo, fige la vidéo sur la dernière image.
            vp.Pause();
            isVideoPausedAtEnd = true; // Permet d'attendre une interaction pour continuer.
        }
        else
        {
            // Si c'est la dernière vidéo, fige l'image sur la dernière frame.
            vp.Pause();
            isVideoPausedAtEnd = true; // Attend l'entrée utilisateur avant de désactiver.
            Debug.Log("Fin de la dernière vidéo. Appuyez sur une touche pour quitter.");
                GameObject VideoObject = GameObject.FindWithTag("VideoPlayer");
                RawImage rawImage = VideoObject.transform.GetChild(1).GetComponent<RawImage>();
            rawImage.transform.gameObject.SetActive(false);
        }
    }

    private void ProceedToNextVideo()
    {
        if (currentVideoIndex < videoClips.Length - 1)
        {
            currentVideoIndex++; // Passe à la vidéo suivante.
            PlayVideo(currentVideoIndex); // Joue la prochaine vidéo.
        }
    }

    private void MuteSceneAudio(bool mute)
    {
        // Trouve tous les AudioSource actifs dans la scène.
        if (sceneAudioSources == null)
        {
            sceneAudioSources = FindObjectsOfType<AudioSource>();
        }

        foreach (var audioSource in sceneAudioSources)
        {
            if (audioSource != musicSource && audioSource != soundEffectSource)
            {
                audioSource.mute = mute;
            }
        }
    }
}