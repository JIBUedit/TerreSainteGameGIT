using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class HoodiemanPatrol : MonoBehaviour,IHealth
{
    public float speed = 2f; // Vitesse normale de patrouille
    public float chaseSpeed = 4f; // Vitesse de poursuite
    public float detectionRange = 5f; // Portée de détection du joueur
    public float waitTime = 3f; // Temps d'attente en secondes

    public Transform[] waypoints;
    public Transform player; // Référence au joueur

    public SpriteRenderer graphics;
    public Animator animator;

    [SerializeField]private Transform target;
    private Vector3 startPoint; // Point de départ de Hoodieman
    private int destPoint;
    private bool isWaiting = false;
    private bool isAttacking = false;

    public LayerMask playerLayer; // Ajoutez une couche pour le joueur
    public Rigidbody2D rb;

    public int maxHealth;
    public int currentHealth;
    public bool isDead = false;
    public Transform attackPoint;
    public float attackRange;

    void Start()
    {
        target = waypoints[0];
        startPoint = transform.position; // Enregistrer la position de départ
        animator.SetBool("isWaiting", true);
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!isWaiting || isAttacking)
        {
            var distance = 0f;
            if (IsPlayerInDetectionRange(out distance))
            {
                //Si il est près ET si il est éloigné
                if (Vector3.Distance(transform.position, player.position) > 0.3f)
                {
                Vector3 chaseDirection = new Vector3(player.position.x - transform.position.x, 0, 0).normalized;
                rb.velocity = chaseDirection * chaseSpeed;
                    Debug.Log($"{chaseDirection} | {chaseSpeed}");

                graphics.flipX = chaseDirection.x < 0;
                animator.SetBool("isRunning", true);
                } //crochet code pour chase
                else
                { 
                StartCoroutine(WaitingAttack());
                rb.velocity = Vector3.zero;
                    var result = graphics.flipX ? transform.position - (transform.position - attackPoint.position) : transform.position + (transform.position - attackPoint.position);
                    
                    //Détecte le player dans la range d'attaque
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(result, attackRange, playerLayer);

                    // Vérifier si player a été touché
                    if (hitEnemies.Length > 0)
                    {
                        // Jouer le son d'attaque
                        /*if (attackSound != null && audioSource != null)
                        {
                            audioSource.PlayOneShot(attackSound);
                        }*/
                    }

                    //Appliquer des dégâts
                    foreach (Collider2D enemy in hitEnemies)
                    {
                        enemy.GetComponent<IHealth>()?.TakeDamage(1);
                    }
                } //crochet code pour attaquer

            }
            else
            {
                target = waypoints[destPoint];
                animator.SetBool("isRunning", false);
                UpdateOrientationTowardsTarget();
                if (Vector3.Distance(transform.position, target.position) < 0.3f)
                {
                    StartCoroutine(WaitAtWaypoint());
                }
                else
                {
                    Vector3 patrolDirection = (target.position - transform.position).normalized;
                    rb.velocity = patrolDirection * speed;
                }
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    private bool IsPlayerInDetectionRange(out float distance)
    {
        Vector2 direction = graphics.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, playerLayer);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                distance = hit.transform.position.x - transform.position.x;
                return true;
            }
        }
        distance = 0f;
        return false;
    }

    private IEnumerator WaitAtWaypoint()
    {
        rb.velocity = Vector3.zero;
        isWaiting = true;
        animator.SetBool("isWaiting", true);

        yield return new WaitForSeconds(waitTime);

        destPoint = (destPoint + 1) % waypoints.Length;
        target = waypoints[destPoint];

        UpdateOrientationTowardsTarget();

        animator.SetBool("isWaiting", false);
        isWaiting = false;
    }

    private void UpdateOrientationTowardsTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        graphics.flipX = directionToTarget.x < 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("isRunning", false);
            Debug.Log("entre en collision avec Wall");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        IsDead(out isDead);
    }
    public void Heal(int hp)
    {

    }
    public void IsDead(out bool dead)
    {
        if (currentHealth <= 0f)
        {
            dead = true;
        }
        else
        {
            dead = false;
        }

    }
    private IEnumerator WaitingAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (graphics != null)
        {
            Gizmos.color = Color.red;
            Vector2 direction = graphics.flipX ? Vector2.left : Vector2.right;
            Vector3 origin = transform.position;
            Gizmos.DrawLine(origin, origin + (Vector3)direction * detectionRange);
        }
        if (attackPoint == null)
                return;
        var result = graphics.flipX ? transform.position - (transform.position - attackPoint.position) : transform.position + (transform.position - attackPoint.position);
        Gizmos.DrawWireSphere(result, attackRange);
    }
}
