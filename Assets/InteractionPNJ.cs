using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; // N�cessaire pour contr�ler le VideoPlayer.

public class InteractionPNJ : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Le VideoPlayer qui lira la vid�o.
    private bool canInteract = false; // V�rifie si le joueur peut interagir.
    private bool dejaVu = false;

    void Update()
    {
        // V�rifie si le joueur est proche et appuie sur "E"
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            GameObject VideoObject = GameObject.FindWithTag("VideoPlayer");
            videoPlayer = VideoObject.transform.GetChild(0).GetComponent<VideoPlayer>();
            RawImage rawImage = VideoObject.transform.GetChild(1).GetComponent<RawImage>();
            if (videoPlayer != null && rawImage != null && !dejaVu)
            {
                dejaVu = true;
                rawImage.gameObject.SetActive(true); // Active le RawImage.
                PlayVideo(rawImage); // Passe le RawImage en param�tre pour activer synchronisation.
            }
            else
            {
                Debug.Log("Le VideoPlayer ou le RawImage n'a pas �t� trouv�");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // V�rifie si le joueur entre dans la zone d'interaction.
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("Appuyez sur E pour interagir.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Le joueur quitte la zone d'interaction.
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            Debug.Log("Zone d'interaction quitt�e.");
        }
    }

    private void PlayVideo(RawImage rawImage)
    {
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true); // Active le GameObject VideoPlayer.
            rawImage.gameObject.SetActive(true); // Assure que le RawImage est actif.
            videoPlayer.Play(); // Lance la lecture de la vid�o.
        }
    }
}
