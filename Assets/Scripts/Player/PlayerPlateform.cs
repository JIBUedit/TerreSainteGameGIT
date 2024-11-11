using System.Collections;
using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    [SerializeField] private CapsuleCollider2D playerCollider;
    private PlatformEffector2D effector;
    private float originalRotationalOffset;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentOneWayPlatform != null)
            {
                effector = currentOneWayPlatform.GetComponent<PlatformEffector2D>();
                if (effector != null)
                {
                    originalRotationalOffset = effector.rotationalOffset;
                    StartCoroutine(DisableCollision());
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        if (effector != null)
        {
            // Permet au joueur de passer à travers la plateforme en changeant l'offset de rotation
            effector.rotationalOffset = 180f;
            yield return new WaitForSeconds(0.25f);
            // Restaurer l'offset original pour permettre au joueur de remonter sur la plateforme
            effector.rotationalOffset = originalRotationalOffset;
        }
    }
}
