using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedDoor : MonoBehaviour
{
    public string sceneName;
    public Animator fadeSystem;

    public AudioSource audioSource;
    public AudioClip soundTeleport;
    public AudioClip soundOpenDoor; // Son � jouer lorsque OpenDoor est activ�

    public GameObject openDoor; // Le GameObject OpenDoor

    private bool isPlayerLoaded = false;
    private Collider2D doorCollider;
    private bool wasDoorActive = false; // Pour suivre l'�tat pr�c�dent de la porte

    private void Start()
    {
        // R�cup�rer le Collider de la porte
        doorCollider = GetComponent<Collider2D>();

        // V�rifier initialement si la porte doit �tre activ�e ou non
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

        // V�rifier si la porte doit �tre activ�e � chaque frame fixe
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        // V�rifie s'il reste des ennemis dans la sc�ne
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // D�termine si la porte doit �tre active
        bool isDoorActive = (enemies.Length == 0);

        // Activer ou d�sactiver le collider de la porte
        doorCollider.enabled = isDoorActive;

        // Activer ou d�sactiver le GameObject OpenDoor
        if (openDoor != null)
        {
            openDoor.SetActive(isDoorActive);

            // Jouer un son uniquement si l'�tat passe de d�sactiv� � activ�
            if (isDoorActive && !wasDoorActive)
            {
                audioSource.PlayOneShot(soundOpenDoor);
            }
        }

        // Mettre � jour l'�tat pr�c�dent de la porte
        wasDoorActive = isDoorActive;
    }
}
