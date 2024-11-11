using UnityEngine;

public class FurryPillory : MonoBehaviour, IHealth
{
    public Animator animator;

    public AudioSource audioSource;
    public AudioClip soundDamage;
    public AudioClip soundGrowl;

    public float maxHealth = 100;
    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        // Lier la fonction PlayDogGrowl à l'événement d'animation
        AnimationEvent dogGrowlEvent = new AnimationEvent();
        dogGrowlEvent.functionName = "PlayDogGrowl";
        dogGrowlEvent.time = 0.0f; // Au début de l'animation
        AnimationClip clip = animator.runtimeAnimatorController.animationClips[0]; // Assurez-vous de mettre le bon indice de clip ici
        clip.AddEvent(dogGrowlEvent);
    }

    public void TakeDamage(int damage)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(soundDamage);
        //currentHealth -= damage;

        // Forcer le redémarrage de l'animation
        animator.Play("Furry Pillory Hit", -1, 0f);
    }
    public void Heal(int hp)
    {

    }
    public void IsDead(out bool dead)
    {
        dead = true;
    }

        // Fonction pour jouer le son de grognement de chien en boucle
        void PlayDogGrowl()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = soundDamage;
            audioSource.loop = true;
            audioSource.PlayOneShot(soundGrowl);
        }
    }
}