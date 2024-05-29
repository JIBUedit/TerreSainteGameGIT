using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 20;
    public int currentHealth;

    public HealthBar healthBar;
    private Animator animator;  // R�f�rence � l'Animator
    private bool isStunned = false; // Indicateur de p�riode de stun
    private Rigidbody2D rb; // Pour arr�ter le mouvement du joueur
    private PlayerMovement playerMovement; // Pour d�sactiver les actions de mouvement

    public static PlayerHealth instance;

    public AudioSource audioSource;
    public AudioClip soundDie;
    public AudioClip soundDamage;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth dans la sc�ne");
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Obtenez la r�f�rence � l'Animator attach� au joueur
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Obtenir le Rigidbody2D
        playerMovement = PlayerMovement.instance; // R�f�rence au script de mouvement
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!isStunned) // Seulement si le joueur n'est pas �tourdi
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(soundDamage);

        if (isStunned) return; // Ignore les d�g�ts si �tourdi

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        // v�rifier si le joueur est toujours vivant
        if(currentHealth <= 0)
        {
            Die();
            return;
        }

        // D�clenche l'animation de prise de d�g�ts
        animator.SetTrigger("isHurting");

        // Commence la p�riode de stun
        StartCoroutine(Stun(0.1f));
    }

    private IEnumerator Stun(float stunDuration)
    {
        isStunned = true; // Activer l'�tourdissement

        // Arr�ter le mouvement du joueur
        rb.velocity = Vector2.zero; // Stopper le mouvement
        rb.angularVelocity = 0; // Stopper la rotation

        // D�sactiver le contr�le du joueur
        playerMovement.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; // D�sactiver l'�tourdissement
        PlayerMovement.instance.enabled = true; // R�activer les mouvements
    }

    public void Die()
    {
        audioSource.PlayOneShot(soundDie);
        isStunned = true; // Activer l'�tourdissement

        // Arr�ter le mouvement du joueur
        rb.velocity = new Vector2(0,rb.velocity.y); // Stopper le mouvement
        rb.angularVelocity = 0; // Stopper la rotation

        // bloquer les mouvements du personnage
        Debug.Log("Le joueur est �limin�.");
        PlayerMovement.instance.enabled = false;
        TabinAttack.instance.enabled = false;

        if (PlayerMovement.instance.animator != null)
        {
            Debug.Log("kys");
        }
        // jouer l'animation d'�limination
        PlayerMovement.instance.animator.SetBool("isDead",true);

        // emp�cher les int�ractions physiques avec les autres �l�ments de la sc�ne
        //PlayerMovement.instance.rb.bodyType = RigidbodyType2D.Kinematic;
        //PlayerMovement.instance.playerCollider.enabled = false;

        GameOverManager.instance.OnPlayerDeath();
    }

    public void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }
}