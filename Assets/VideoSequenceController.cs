using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSequenceController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // R�f�rence au VideoPlayer.
    public VideoClip[] videoClips; // Tableau des vid�os � encha�ner.
    public AudioClip[] videoSounds; // Tableau des sons associ�s aux vid�os.
    public AudioSource musicSource; // AudioSource pour la musique de fond.
    public AudioSource soundEffectSource; // AudioSource pour les sons des vid�os.

    private AudioSource[] sceneAudioSources; // Tous les AudioSource de la sc�ne.
    private int currentVideoIndex = 0; // Index de la vid�o actuelle.
    private bool isVideoPausedAtEnd = false; // Indique si la vid�o est fig�e � la fin.

    void OnEnable()
    {
        // Interrompt tous les sons de la sc�ne.
        MuteSceneAudio(true);

        // D�marre la musique lorsque le GameObject s'active.
        if (musicSource != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play(); // Force la lecture de la musique.
            }
            else
            {
                Debug.LogWarning("MusicSource est d�j� en lecture.");
            }
        }
    }

    void OnDisable()
    {
        // R�active tous les sons de la sc�ne.
        MuteSceneAudio(false);

        // Arr�te la musique lorsque le GameObject se d�sactive.
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    void Start()
    {
        if (videoClips.Length > 0 && videoPlayer != null)
        {
            PlayVideo(currentVideoIndex); // Lance la premi�re vid�o.
            videoPlayer.loopPointReached += OnVideoEnd; // Abonnez-vous � l��v�nement de fin.
        }
        else
        {
            Debug.LogError("Aucune vid�o assign�e ou VideoPlayer manquant.");
        }
    }

    void Update()
    {
        if (!isVideoPausedAtEnd)
        {
            // Acc�l�ration de la vid�o si une touche est enfonc�e.
            if (Input.anyKey)
            {
                videoPlayer.playbackSpeed = 4.0f; // Vitesse x4.
            }
            else
            {
                videoPlayer.playbackSpeed = 1.0f; // Retour � la vitesse normale.
            }
        }

        // V�rifie les interactions utilisateur pour avancer apr�s une pause.
        if (isVideoPausedAtEnd && Input.anyKeyDown)
        {
            if (currentVideoIndex < videoClips.Length - 1)
            {
                ProceedToNextVideo();
            }
            else
            {
                // D�sactive le GameObject apr�s la derni�re vid�o.
                gameObject.SetActive(false);
            }
        }
    }

    private void PlayVideo(int index)
    {
        if (index >= 0 && index < videoClips.Length)
        {
            // Arr�te le son de la vid�o pr�c�dente (si n�cessaire).
            if (soundEffectSource != null && soundEffectSource.isPlaying)
            {
                soundEffectSource.Stop();
            }

            videoPlayer.clip = videoClips[index]; // Charge la vid�o correspondante.
            videoPlayer.playbackSpeed = 1.0f; // S'assure que la vitesse initiale est normale.
            videoPlayer.Play(); // Joue la vid�o.

            // Joue le son associ� � cette vid�o.
            if (videoSounds != null && index < videoSounds.Length && soundEffectSource != null)
            {
                soundEffectSource.clip = videoSounds[index];
                soundEffectSource.Play(); // Joue le son correspondant.
            }

            isVideoPausedAtEnd = false; // R�initialise l'�tat.
        }
        else
        {
            Debug.LogWarning("Index de vid�o invalide.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log($"Fin de la vid�o : {vp.clip.name}");
        if (currentVideoIndex < videoClips.Length - 1)
        {
            // Si ce n'est pas la derni�re vid�o, fige la vid�o sur la derni�re image.
            vp.Pause();
            isVideoPausedAtEnd = true; // Permet d'attendre une interaction pour continuer.
        }
        else
        {
            // Si c'est la derni�re vid�o, fige l'image sur la derni�re frame.
            vp.Pause();
            isVideoPausedAtEnd = true; // Attend l'entr�e utilisateur avant de d�sactiver.
            Debug.Log("Fin de la derni�re vid�o. Appuyez sur une touche pour quitter.");
                GameObject VideoObject = GameObject.FindWithTag("VideoPlayer");
                RawImage rawImage = VideoObject.transform.GetChild(1).GetComponent<RawImage>();
            rawImage.transform.gameObject.SetActive(false);
        }
    }

    private void ProceedToNextVideo()
    {
        if (currentVideoIndex < videoClips.Length - 1)
        {
            currentVideoIndex++; // Passe � la vid�o suivante.
            PlayVideo(currentVideoIndex); // Joue la prochaine vid�o.
        }
    }

    private void MuteSceneAudio(bool mute)
    {
        // Trouve tous les AudioSource actifs dans la sc�ne.
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