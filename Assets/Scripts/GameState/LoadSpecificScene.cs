using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSpecificScene : MonoBehaviour
{
    public string sceneName;
    public Animator fadeSystem;

    public AudioSource audioSource;
    public AudioClip soundTeleport;

    private bool isPlayerLoaded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            audioSource.PlayOneShot(soundTeleport);
            StartCoroutine(loadNextScene());
        }
    }
    public IEnumerator loadNextScene()
    {
        fadeSystem.SetTrigger("FadeIn");
        fadeSystem.SetBool("GameStarted", true);
        PlayerMovement.instance.canMove = false;
        yield return new WaitForSeconds(1f);
        PlayerMovement.instance.canMove = true;
        SceneManager.LoadScene(sceneName);
    }
    public void FixedUpdate()
    {
        if(!isPlayerLoaded)
        {
            GameObject GoFade = GameObject.FindGameObjectWithTag("Fade");
            if (GoFade != null)
            {
                fadeSystem = GoFade.GetComponent<Animator>();
                isPlayerLoaded = true;
            }
        }

    }
}
