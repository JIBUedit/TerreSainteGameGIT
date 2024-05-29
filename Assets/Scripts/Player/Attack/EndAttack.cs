using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAttack : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // R�cup�rer le script de mouvement du personnage
        PlayerMovement playerMovement = animator.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            // R�activer le mouvement � la fin de l'animation d'attaque
            playerMovement.canMove = true;
        }
    }
}
