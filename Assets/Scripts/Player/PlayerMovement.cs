using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public bool isJumping = false;
    public bool isGrounded = false;
    public bool canMove = true;
    public bool onStairs = false;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask collisionLayers;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D playerCollider;

    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;

    public static PlayerMovement instance;

    public Transform feetPosition;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerMovement dans la sc�ne");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.VideoPlayer)
        {
            // V�rifie l'�tat du RawImage pour activer ou d�sactiver le mouvement
            CheckRawImagePresence();
        }

        if (!canMove)
        {
            rb.velocity = Vector2.zero; // Arr�te le joueur si le mouvement est d�sactiv�.
            return; // Bloque toute autre action.
        }

        CustomUpdate();

        var onStairs = false;
        CheckStairs();
        if (onStairs)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        GroundCheck();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }
        Flip(rb.velocity.x);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove) // V�rifie si le mouvement est autoris�
        {
            horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed;
            animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
            animator.SetFloat("yVelocity", rb.velocity.y);

            isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

            MovePlayer(horizontalMovement);
        }
        else
        {
            rb.velocity = Vector2.zero; // Arr�te le mouvement
        }
    }

    void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        if (isJumping == true)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
    }

    void Flip(float _velocity)
    {
        if (_velocity > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (_velocity < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void GroundCheck()
    {
        var test = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers); // ici il te sort un Collider2D
        isGrounded = test != null; // ici c'est un bool
        animator.SetBool("isJumping", !isGrounded);
    }

    private void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void CheckStairs()
    {
        var rayDistance = 1;
        RaycastHit2D hit = Physics2D.Raycast(feetPosition.position, Vector2.down, rayDistance, collisionLayers);
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            //Debug.Log("J'ai touch� des escaliers");
            onStairs = true;
            rb.gravityScale = 0; // D�sactiver la gravit� sur les escaliers
        }
        else
        {
            //Debug.Log("Je ne touche pas d'escaliers");
            onStairs = false;
            rb.gravityScale = 4; // R�activer la gravit�
        }
    }

    private void CheckRawImagePresence()
    {
        GameObject videoObject = GameObject.FindWithTag("VideoPlayer");

        if (videoObject != null)
        {
            RawImage rawImage = videoObject.transform.GetChild(1).GetComponent<RawImage>();
            if (rawImage != null && rawImage.gameObject.activeSelf)
            {
                canMove = false; // D�sactive le mouvement si le RawImage est actif.
                return;
            }
        }

        canMove = true; // Active le mouvement si le RawImage est introuvable ou inactif.
    }
}
