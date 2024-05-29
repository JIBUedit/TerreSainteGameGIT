using UnityEngine;
using System.Collections;

public class RainbowDamage : MonoBehaviour
{
    public int damage = 10;  // Montant des dégâts infligés
    public float damageInterval = 1.0f;  // Intervalle de dégâts en secondes
    private bool isPlayerInZone = false;  // Pour suivre si le joueur est dans la zone
    public PlayerHealth playerHealth;  // Référence au script de santé du joueur

    private Coroutine damageCoroutine;  // Pour gérer la Coroutine des dégâts périodiques

    // Détecte quand le joueur entre dans la zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (damageCoroutine == null)  // Démarre la Coroutine si elle n'est pas déjà active
            {
                if (other.GetComponent<PlayerHealth>() != null)
                damageCoroutine = StartCoroutine(DamagePlayerPeriodically(other.GetComponent<PlayerHealth>()));
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
        while (isPlayerInZone)
        {

            health.TakeDamage(damage);  // Inflige des dégâts au joueur
            yield return new WaitForSeconds(damageInterval);  // Attend l'intervalle spécifié
        }
    }
}
