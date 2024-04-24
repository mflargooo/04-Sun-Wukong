using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    private float time;
    [SerializeField] private TMP_Text onScreenTime;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject basicPanel;

    private bool win = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (win) return;
        time += Time.deltaTime;
        onScreenTime.text = time.ToString("F2");
    }

    public void Win()
    {
        SceneManager.MoveGameObjectToScene(transform.parent.gameObject, SceneManager.GetActiveScene());
        win = true;
        winText.text = "time taken: " + onScreenTime.text;
        onScreenTime.gameObject.SetActive(false);
        basicPanel.SetActive(true);
        winPanel.SetActive(true);
    }

    public void Dead()
    {
        SoundManager.instance.PlayNezhaLaugh();
        win = true;
    }
}
