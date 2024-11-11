using UnityEngine;

public partial class PlayerMovement
{
    public GameObject levelSelector; // Référence à votre Canvas
    private bool isNearMap = false; // Indique si le joueur est proche de la carte

    void CustomUpdate()
    {
        // Vérifiez si le joueur est proche de la carte et appuie sur "E"
        if (isNearMap && Input.GetKeyDown(KeyCode.E))
        {
            // Active le Canvas
            levelSelector.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si le joueur est entré en collision avec la carte
        if (other.CompareTag("Map"))
        {
            isNearMap = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Vérifie si le joueur a quitté la collision avec la carte
        if (other.CompareTag("Map"))
        {
            isNearMap = false;
            levelSelector.gameObject.SetActive(false);
        }
    }
}
