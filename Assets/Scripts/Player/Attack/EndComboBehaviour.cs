using UnityEngine;

public class EndComboBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Rien � faire � l'entr�e de l'�tat
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.Playing)
        {
            // R�activer le mouvement du joueur � la fin de l'attaque
            TabinAttack.instance.playerMovement.canMove = true;
            TabinAttack.instance.canReceiveInput = false; // Reset the input flag
            TabinAttack.instance.inputReceived = false; // Reset the input flag
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Rien � faire pendant la mise � jour de l'�tat
    }
}
