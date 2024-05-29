using UnityEngine;
using System.Collections;

public class RainbowDamage : MonoBehaviour
{
    public int damage = 10;  // Montant des d�g�ts inflig�s
    public float damageInterval = 1.0f;  // Intervalle de d�g�ts en secondes
    private bool isPlayerInZone = false;  // Pour suivre si le joueur est dans la zone
    public PlayerHealth playerHealth;  // R�f�rence au script de sant� du joueur

    private Coroutine damageCoroutine;  // Pour g�rer la Coroutine des d�g�ts p�riodiques

    // D�tecte quand le joueur entre dans la zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (damageCoroutine == null)  // D�marre la Coroutine si elle n'est pas d�j� active
            {
                if (other.GetComponent<PlayerHealth>() != null)
                damageCoroutine = StartCoroutine(DamagePlayerPeriodically(other.GetComponent<PlayerHealth>()));
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
        while (isPlayerInZone)
        {

            health.TakeDamage(damage);  // Inflige des d�g�ts au joueur
            yield return new WaitForSeconds(damageInterval);  // Attend l'intervalle sp�cifi�
        }
    }
}
