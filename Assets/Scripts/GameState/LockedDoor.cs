using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedDoor : MonoBehaviour
{
    public string sceneName;
    public Animator fadeSystem;

    public AudioSource audioSource;
    public AudioClip soundTeleport;
    public AudioClip soundOpenDoor; // Son à jouer lorsque OpenDoor est activé

    public GameObject openDoor; // Le GameObject OpenDoor

    private bool isPlayerLoaded = false;
    private Collider2D doorCollider;
    private bool wasDoorActive = false; // Pour suivre l'état précédent de la porte

    private void Start()
    {
        // Récupérer le Collider de la porte
        doorCollider = GetComponent<Collider2D>();

        // Vérifier initialement si la porte doit être activée ou non
        UpdateDoorState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && doorCollider.enabled)
        {
            audioSource.PlayOneShot(soundTeleport);
            StartCoroutine(loadNextScene());
        }
    }

    public IEnumerator loadNextScene()
    {
        fadeSystem.SetTrigger("FadeIn");
        fadeSystem.SetBool("GameStarted", true);
        PlayerMovement.instance.canMove = false;
        yield return new WaitForSeconds(1f);
        PlayerMovement.instance.canMove = true;
        SceneManager.LoadScene(sceneName);
    }

    private void FixedUpdate()
    {
        if (!isPlayerLoaded)
        {
            GameObject GoFade = GameObject.FindGameObjectWithTag("Fade");
            if (GoFade != null)
            {
                fadeSystem = GoFade.GetComponent<Animator>();
                isPlayerLoaded = true;
            }
        }

        // Vérifier si la porte doit être activée à chaque frame fixe
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        // Vérifie s'il reste des ennemis dans la scène
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Détermine si la porte doit être active
        bool isDoorActive = (enemies.Length == 0);

        // Activer ou désactiver le collider de la porte
        doorCollider.enabled = isDoorActive;

        // Activer ou désactiver le GameObject OpenDoor
        if (openDoor != null)
        {
            openDoor.SetActive(isDoorActive);

            // Jouer un son uniquement si l'état passe de désactivé à activé
            if (isDoorActive && !wasDoorActive)
            {
                audioSource.PlayOneShot(soundOpenDoor);
            }
        }

        // Mettre à jour l'état précédent de la porte
        wasDoorActive = isDoorActive;
    }
}
