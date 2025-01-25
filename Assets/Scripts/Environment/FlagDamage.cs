using UnityEngine;
using System.Collections;

public class FlagDamage : MonoBehaviour
{
    public int damage = 1;  // Montant des dégâts infligés
    public float damageInterval = 1.0f;  // Intervalle de dégâts en secondes
    private bool isPlayerInZone = false;  // Pour suivre si le joueur est dans la zone
    public PlayerHealth playerHealth;  // Référence au script de santé du joueur
    private Coroutine damageCoroutine;  // Pour gérer la Coroutine des dégâts périodiques

    [Header("Flag Settings")]
    public GameObject enemyFlag;  // Référence à Enemy Flag (à définir dans l'inspecteur)
    public GameObject frenchFlag; // Référence à French Flag (optionnel, selon vos besoins)
    public AudioSource audioSource;  // Source audio pour jouer un son
    public AudioClip flagDisabledSound;  // Son à jouer lorsque Enemy Flag est désactivé

    private bool isFlagDisabled = false; // Indique si Enemy Flag a été désactivé

    // Détecte quand le joueur entre dans la zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (damageCoroutine == null && !isFlagDisabled)  // Démarre les dégâts uniquement si Enemy Flag est actif
            {
                if (other.GetComponent<PlayerHealth>() != null)
                {
                    playerHealth = other.GetComponent<PlayerHealth>();
                    damageCoroutine = StartCoroutine(DamagePlayerPeriodically(other.GetComponent<PlayerHealth>()));
                }
            }
        }
    }

    // Détecte quand le joueur quitte la zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (damageCoroutine != null)  // Arrête la Coroutine si elle est active
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    // Coroutine pour infliger des dégâts périodiquement
    private IEnumerator DamagePlayerPeriodically(PlayerHealth health)
    {
        yield return new WaitForSeconds(0.5f);  // Attend l'intervalle spécifié
        while (isPlayerInZone && !isFlagDisabled) // Ne fait pas de dégâts si le flag est désactivé
        {
            health.TakeDamage(damage);  // Inflige des dégâts au joueur
            yield return new WaitForSeconds(damageInterval);  // Attend l'intervalle spécifié
        }
    }

    private void Update()
    {
        // Vérifie si le joueur appuie sur "E" dans la zone et si le drapeau est actif
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && enemyFlag != null && !isFlagDisabled)
        {
            playerHealth.RestoreHealth(3);

            // Désactive le Enemy Flag
            enemyFlag.SetActive(false);
            isFlagDisabled = true;

            // Joue le son de désactivation si disponible
            if (audioSource != null && flagDisabledSound != null)
            {
                audioSource.PlayOneShot(flagDisabledSound);
            }

            // Arrête les dégâts de la zone
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            // Optionnel : Active le French Flag si nécessaire
            if (frenchFlag != null)
            {
                frenchFlag.SetActive(true);
            }
        }
    }
}
