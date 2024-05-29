using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAttack : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Récupérer le script de mouvement du personnage
        PlayerMovement playerMovement = animator.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            // Réactiver le mouvement à la fin de l'animation d'attaque
            playerMovement.canMove = true;
        }
    }
}
