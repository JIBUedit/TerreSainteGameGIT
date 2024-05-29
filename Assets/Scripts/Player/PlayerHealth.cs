using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 20;
    public int currentHealth;

    public HealthBar healthBar;
    private Animator animator;  // Référence à l'Animator
    private bool isStunned = false; // Indicateur de période de stun
    private Rigidbody2D rb; // Pour arrêter le mouvement du joueur
    private PlayerMovement playerMovement; // Pour désactiver les actions de mouvement

    public static PlayerHealth instance;

    public AudioSource audioSource;
    public AudioClip soundDie;
    public AudioClip soundDamage;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth dans la scène");
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Obtenez la référence à l'Animator attaché au joueur
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Obtenir le Rigidbody2D
        playerMovement = PlayerMovement.instance; // Référence au script de mouvement
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!isStunned) // Seulement si le joueur n'est pas étourdi
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(soundDamage);

        if (isStunned) return; // Ignore les dégâts si étourdi

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        // vérifier si le joueur est toujours vivant
        if(currentHealth <= 0)
        {
            Die();
            return;
        }

        // Déclenche l'animation de prise de dégâts
        animator.SetTrigger("isHurting");

        // Commence la période de stun
        StartCoroutine(Stun(0.1f));
    }

    private IEnumerator Stun(float stunDuration)
    {
        isStunned = true; // Activer l'étourdissement

        // Arrêter le mouvement du joueur
        rb.velocity = Vector2.zero; // Stopper le mouvement
        rb.angularVelocity = 0; // Stopper la rotation

        // Désactiver le contrôle du joueur
        playerMovement.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; // Désactiver l'étourdissement
        PlayerMovement.instance.enabled = true; // Réactiver les mouvements
    }

    public void Die()
    {
        audioSource.PlayOneShot(soundDie);
        isStunned = true; // Activer l'étourdissement

        // Arrêter le mouvement du joueur
        rb.velocity = new Vector2(0,rb.velocity.y); // Stopper le mouvement
        rb.angularVelocity = 0; // Stopper la rotation

        // bloquer les mouvements du personnage
        Debug.Log("Le joueur est éliminé.");
        PlayerMovement.instance.enabled = false;
        TabinAttack.instance.enabled = false;

        if (PlayerMovement.instance.animator != null)
        {
            Debug.Log("kys");
        }
        // jouer l'animation d'élimination
        PlayerMovement.instance.animator.SetBool("isDead",true);

        // empêcher les intéractions physiques avec les autres éléments de la scène
        //PlayerMovement.instance.rb.bodyType = RigidbodyType2D.Kinematic;
        //PlayerMovement.instance.playerCollider.enabled = false;

        GameOverManager.instance.OnPlayerDeath();
    }

    public void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }
}