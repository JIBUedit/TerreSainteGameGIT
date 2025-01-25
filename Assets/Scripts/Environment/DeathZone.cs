using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private Transform playerSpawn; // D�finir la zone de spawn directement dans l'inspecteur
    public PlayerHealth playerHealth;  // R�f�rence au script de sant� du joueur

    private void Awake()
    {
        // Si playerSpawn n'est pas d�fini dans l'inspecteur, chercher par d�faut l'objet avec le tag "PlayerSpawn"
        if (playerSpawn == null)
        {
            GameObject defaultSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (defaultSpawn != null)
            {
                playerSpawn = defaultSpawn.transform;
            }
            else
            {
                Debug.LogError("Aucun PlayerSpawn n'a �t� assign� ou trouv� dans la sc�ne !");
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
                    Debug.LogError("playerSpawn n'est pas d�fini. Impossible de t�l�porter le joueur.");
                }
            }
            else
            {
                Debug.LogWarning("Le joueur n'a pas de script PlayerHealth attach�.");
            }
        }
    }
}
