using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private Transform playerSpawn;
    public PlayerHealth playerHealth;  // Référence au script de santé du joueur
    private void Awake()
    {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<PlayerHealth>();  // Obtenez le script PlayerHealth du joueur

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                collision.transform.position = playerSpawn.position;
            }
            else
            {
                Debug.LogWarning("Le joueur n'a pas de script PlayerHealth attaché.");
            }
        }
    }
}