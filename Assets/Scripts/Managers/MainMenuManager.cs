using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] disableOnKeyPress;
    [SerializeField] private AudioClip playOnKeyPress;
    [SerializeField] private AudioSource playSrc;

    [SerializeField] private Animator blackOutAnim;
    [SerializeField] private GameObject pressKeyText;

    private bool pressed = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !pressed)
        {
            pressed = true;
            foreach (AudioSource audio in disableOnKeyPress)
            {
                if (audio.isPlaying) audio.Stop();
                audio.enabled = false;
            }

            StartCoroutine(PlayGame());
        }
    }

    IEnumerator PlayGame()
    {
        playSrc.PlayOneShot(playOnKeyPress);
        Destroy(pressKeyText);
        yield return new WaitForSeconds(1.5f);
        blackOutAnim.Play("fade_out");
        yield return new WaitForSeconds(1.5f);
        GetComponent<SceneChanger>().LoadNextScene();
        Destroy(this);
    }
}
