using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth
{
    public int maxHealth = 20;
    public int currentHealth;

    public HealthBar healthBar;
    private Animator animator;
    private bool isStunned = false;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private bool isDead = false;

    public static PlayerHealth instance;

    public AudioSource audioSource;
    public AudioClip soundDie;
    public AudioClip soundDamage;
    public AudioClip soundHeal; // Son joué lors de la guérison

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth dans la scène");
            return;
        }

        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = PlayerMovement.instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!isStunned)
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(soundDamage);

        if (isStunned || isDead) return;

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("isHurting");

        StartCoroutine(Stun(0.1f));
    }

    public void RestoreHealth(int amount)
    {
        if (isDead) return; // Ne pas soigner si le joueur est mort

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Mise à jour de la barre de santé
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        // Joue le son de soin
        if (audioSource != null && soundHeal != null)
        {
            audioSource.PlayOneShot(soundHeal);
        }

        Debug.Log($"Santé restaurée : +{amount}. Santé actuelle : {currentHealth}/{maxHealth}");
    }


    public void Heal(int hp)
    {
        RestoreHealth(hp);
    }

    public void IsDead(out bool dead)
    {
        dead = isDead;
    }

    private IEnumerator Stun(float stunDuration)
    {
        isStunned = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        playerMovement.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        PlayerMovement.instance.enabled = true;
    }

    public void Die()
    {
        audioSource.PlayOneShot(soundDie);
        isStunned = true;
        isDead = true;

        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.angularVelocity = 0;

        Debug.Log("Le joueur est éliminé.");
        PlayerMovement.instance.enabled = false;
        TabinAttack.instance.enabled = false;

        if (PlayerMovement.instance.animator != null)
        {
            Debug.Log("Animation de mort jouée");
        }

        PlayerMovement.instance.animator.SetBool("isDead", true);

        GameOverManager.instance.OnPlayerDeath();
    }
}
