using UnityEngine;
using System.Collections;

public class TabinAttack : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 0.5f;
    public int attackDamage = 1;

    public AudioClip attackSound;
    public AudioSource audioSource;

    public bool isAttacking = false;

    public bool canReceiveInput;
    public bool inputReceived;

    public PlayerMovement playerMovement;

    // Distance entre le personnage et le point d'attaque
    public Vector2 attackOffset = new Vector2(0.5f, 0f); // Définition correcte d'un Vector2

    public static TabinAttack instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de TabinAttack dans la scène");
            return;
        }

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Si le joueur appuie sur Mouse0 et que le personnage est au sol
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerMovement.isGrounded)
        {
            // Ajuste la position de l'attackPoint selon le flip du sprite
            if (playerMovement.spriteRenderer.flipX)
            {  // Si le personnage est retourné à gauche
                attackPoint.localPosition = new Vector2(-0.3f, attackPoint.localPosition.y);  // Utilisez le float ajusté
            }
            else
            {  // Si le personnage fait face à droite
                attackPoint.localPosition = new Vector2(0.3f, attackPoint.localPosition.y);  // Position normale
            }
            Attack();
        }
    }

    void Attack()
    {
        // L'attaque commence
        if (Input.GetKeyDown(KeyCode.Mouse0) && GameManager.Instance.currentGameState == GameManager.GameState.Playing)
        {
            if (canReceiveInput)
            {
                inputReceived = true;
                canReceiveInput = false;
            }
            else
            {
                return;
            }
        }

        //Détecte les ennemis dans la range d'attaque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Vérifier si un ennemi a été touché
        if (hitEnemies.Length > 0)
        {
            // Jouer le son d'attaque
            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }

        //Appliquer des dégâts
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<IHealth>()?.TakeDamage(attackDamage);
        }

        if (playerMovement != null)
        {
            if (GameManager.Instance.currentGameState == GameManager.GameState.Playing)
                Debug.Log("Playing");
            // Désactiver le mouvement
            playerMovement.canMove = false;
        }

    }

    public void InputManager()
    {
        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else
        {
            canReceiveInput = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}