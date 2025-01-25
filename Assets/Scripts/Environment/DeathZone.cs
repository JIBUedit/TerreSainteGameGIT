using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private Transform playerSpawn; // Définir la zone de spawn directement dans l'inspecteur
    public PlayerHealth playerHealth;  // Référence au script de santé du joueur

    private void Awake()
    {
        // Si playerSpawn n'est pas défini dans l'inspecteur, chercher par défaut l'objet avec le tag "PlayerSpawn"
        if (playerSpawn == null)
        {
            GameObject defaultSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (defaultSpawn != null)
            {
                playerSpawn = defaultSpawn.transform;
            }
            else
            {
                Debug.LogError("Aucun PlayerSpawn n'a été assigné ou trouvé dans la scène !");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<PlayerHealth>();  // Obtenez le script PlayerHealth du joueur

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);

                if (playerSpawn != null)
                {
                    collision.transform.position = playerSpawn.position;
                }
                else
                {
                    Debug.LogError("playerSpawn n'est pas défini. Impossible de téléporter le joueur.");
                }
            }
            else
            {
                Debug.LogWarning("Le joueur n'a pas de script PlayerHealth attaché.");
            }
        }
    }
}
