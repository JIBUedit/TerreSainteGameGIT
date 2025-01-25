using UnityEngine;
using System.Collections;

public class FlagDamage : MonoBehaviour
{
    public int damage = 1;  // Montant des d�g�ts inflig�s
    public float damageInterval = 1.0f;  // Intervalle de d�g�ts en secondes
    private bool isPlayerInZone = false;  // Pour suivre si le joueur est dans la zone
    public PlayerHealth playerHealth;  // R�f�rence au script de sant� du joueur
    private Coroutine damageCoroutine;  // Pour g�rer la Coroutine des d�g�ts p�riodiques

    [Header("Flag Settings")]
    public GameObject enemyFlag;  // R�f�rence � Enemy Flag (� d�finir dans l'inspecteur)
    public GameObject frenchFlag; // R�f�rence � French Flag (optionnel, selon vos besoins)
    public AudioSource audioSource;  // Source audio pour jouer un son
    public AudioClip flagDisabledSound;  // Son � jouer lorsque Enemy Flag est d�sactiv�

    private bool isFlagDisabled = false; // Indique si Enemy Flag a �t� d�sactiv�

    // D�tecte quand le joueur entre dans la zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (damageCoroutine == null && !isFlagDisabled)  // D�marre les d�g�ts uniquement si Enemy Flag est actif
            {
                if (other.GetComponent<PlayerHealth>() != null)
                {
                    playerHealth = other.GetComponent<PlayerHealth>();
                    damageCoroutine = StartCoroutine(DamagePlayerPeriodically(other.GetComponent<PlayerHealth>()));
                }
            }
        }
    }

    // D�tecte quand le joueur quitte la zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (damageCoroutine != null)  // Arr�te la Coroutine si elle est active
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    // Coroutine pour infliger des d�g�ts p�riodiquement
    private IEnumerator DamagePlayerPeriodically(PlayerHealth health)
    {
        yield return new WaitForSeconds(0.5f);  // Attend l'intervalle sp�cifi�
        while (isPlayerInZone && !isFlagDisabled) // Ne fait pas de d�g�ts si le flag est d�sactiv�
        {
            health.TakeDamage(damage);  // Inflige des d�g�ts au joueur
            yield return new WaitForSeconds(damageInterval);  // Attend l'intervalle sp�cifi�
        }
    }

    private void Update()
    {
        // V�rifie si le joueur appuie sur "E" dans la zone et si le drapeau est actif
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && enemyFlag != null && !isFlagDisabled)
        {
            playerHealth.RestoreHealth(3);

            // D�sactive le Enemy Flag
            enemyFlag.SetActive(false);
            isFlagDisabled = true;

            // Joue le son de d�sactivation si disponible
            if (audioSource != null && flagDisabledSound != null)
            {
                audioSource.PlayOneShot(flagDisabledSound);
            }

            // Arr�te les d�g�ts de la zone
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            // Optionnel : Active le French Flag si n�cessaire
            if (frenchFlag != null)
            {
                frenchFlag.SetActive(true);
            }
        }
    }
}
