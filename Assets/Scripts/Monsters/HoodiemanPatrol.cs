using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodiemanPatrol : MonoBehaviour, IHealth
{
    // --- Propriétés et Variables ---
    public float speed = 2f; // Vitesse normale de patrouille
    public float chaseSpeed = 4f; // Vitesse de poursuite
    public float detectionRange = 5f; // Portée de détection du joueur
    public float waitTime = 3f; // Temps d'attente en secondes

    public Transform[] waypoints;
    public Transform player; // Référence au joueur

    public SpriteRenderer graphics;
    public Animator animator;

    [SerializeField] private Transform target;
    private Vector3 startPoint; // Point de départ de Hoodieman
    private int destPoint;
    private bool isWaiting = false;
    private bool isAttacking = false;
    private bool isPaused = false; // Pour savoir si la patrouille est en pause

    public LayerMask playerLayer; // Ajoutez une couche pour le joueur
    public Rigidbody2D rb;

    public int maxHealth;
    public int currentHealth;
    public bool isDead = false;
    public Transform attackPoint;
    public float attackRange;
    private Vector3 distanceAttack;

    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioSource audioSource;

    // --- Méthodes Unity ---
    void Start()
    {
        target = waypoints[0];
        startPoint = transform.position; // Enregistrer la position de départ
        animator.SetBool("isWaiting", true);
        currentHealth = maxHealth;
        distanceAttack = attackPoint.position - transform.position;
    }

    void Update()
    {
        if (isDead)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // Bloquer la vélocité pendant l'attaque
        if (isAttacking)
        {
            rb.velocity = Vector3.zero; // Bloque le mouvement
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false); // Désactive les animations de mouvement
            return; // Ne continue pas avec la logique de patrouille ou de poursuite
        }

        // Si la patrouille est en pause, ne rien faire
        if (isPaused)
        {
            return;
        }

        // Logique de patrouille ou poursuite
        var distance = 0f;
        if (IsPlayerInDetectionRange(out distance))
        {
            ChaseOrAttackPlayer(distance);
        }
        else
        {
            Patrol();
        }
    }

    // --- Méthodes de l'Interface ---
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("isHurt");

        if (hurtSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int hp)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Min(currentHealth + hp, maxHealth);
        }
    }

    public void IsDead(out bool dead)
    {
        dead = currentHealth <= 0;
    }

    // --- Méthodes Privées ---
    private void ChaseOrAttackPlayer(float distance)
    {
        if (distance > 0.3f)
        {
            // Poursuite du joueur
            Vector3 chaseDirection = new Vector3(player.position.x - transform.position.x, 0, 0).normalized;
            rb.velocity = chaseDirection * chaseSpeed;

            // Retourner le sprite en fonction de la direction du mouvement
            graphics.flipX = chaseDirection.x < 0;

            animator.SetBool("isRunning", true); // Animation de course
            animator.SetBool("isWalking", false); // Assurer que "isWalking" est false pendant la course
        }
        else
        {
            // Si l'attaque entre en collision avec le joueur
            var attackPosition = graphics.flipX
                ? transform.position - distanceAttack
                : transform.position + distanceAttack;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, playerLayer);
            foreach (var enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    // Déclenche un message de débogage lorsqu'une collision est détectée
                    Debug.Log("AttackPoint hit Player!"); // Message de débogage

                    // Déclenche la pause de patrouille pendant 2 secondes après l'attaque
                    StartCoroutine(PausePatrol(2f));

                    // Dommages à l'ennemi
                    enemy.GetComponent<IHealth>()?.TakeDamage(1);
                }
            }
        }
    }


    private void Patrol()
    {
        target = waypoints[destPoint]; // Mise à jour de la cible

        // Si en attente, ne pas faire de patrouille
        if (isWaiting)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false); // Pas d'animation de marche
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position); // Calcul de la distance au waypoint actuel

        // Calcul de la direction vers le waypoint
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Retourner le sprite en fonction de la direction
        graphics.flipX = directionToTarget.x < 0; // Retourne le sprite si le déplacement est vers la gauche

        if (distanceToTarget < 1f) // Si la distance est faible, on attend au waypoint
        {
            StartCoroutine(WaitAtWaypoint());
        }
        else
        {
            // Déplacement vers le waypoint
            Vector3 patrolDirection = (target.position - transform.position).normalized;
            rb.velocity = patrolDirection * speed;

            animator.SetBool("isRunning", false); // Animation de course désactivée
            animator.SetBool("isWalking", true); // Animation de marche activée
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        rb.velocity = Vector3.zero;

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        rb.velocity = Vector3.zero;
        isWaiting = true;
        animator.SetBool("isWaiting", true);

        yield return new WaitForSeconds(waitTime);

        destPoint = (destPoint + 1) % waypoints.Length;
        target = waypoints[destPoint];
        animator.SetBool("isWaiting", false);
        isWaiting = false;
    }

    private bool IsPlayerInDetectionRange(out float distance)
    {
        Vector2 direction = graphics.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            distance = Vector3.Distance(transform.position, hit.collider.transform.position);
            return true;
        }

        distance = 0f;
        return false;
    }

    private void UpdateOrientationTowardsTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        graphics.flipX = directionToTarget.x < 0;
    }

    private IEnumerator WaitingAttack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");

        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    // Coroutine pour mettre la patrouille en pause pendant 2 secondes après une attaque
    private IEnumerator PausePatrol(float seconds)
    {
        isPaused = true;
        rb.velocity = Vector3.zero; // Arrêter le mouvement
        animator.SetBool("isWalking", false); // Arrêter l'animation de marche
        animator.SetBool("isRunning", false); // Arrêter l'animation de course

        yield return new WaitForSeconds(seconds); // Attente de 2 secondes

        isPaused = false; // Reprendre la patrouille
    }

    private void OnDrawGizmosSelected()
    {
        if (graphics != null)
        {
            Gizmos.color = Color.red;
            Vector2 direction = graphics.flipX ? Vector2.left : Vector2.right;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * detectionRange);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector3 attackPosition = transform.position + (graphics.flipX ? Vector3.left : Vector3.right) * Mathf.Abs(distanceAttack.x);
            Gizmos.DrawWireSphere(attackPosition, attackRange);
        }
    }
}
