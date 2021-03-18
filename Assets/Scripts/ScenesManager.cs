using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    
    public GameObject canvasPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject timeOutPanel;
    [SerializeField] Animator anim;
    [SerializeField] ScoreManager scoreManager;

    public void LoadScene(int index)
    {
        canvasPanel.layer = 0;

        canvasPanel.GetComponent<Canvas>().sortingOrder = 4;
        StartCoroutine(LoadLevelCo(index));

    }

    public void LoadNextLevel()
    {
        canvasPanel.layer = 0;

        canvasPanel.GetComponent<Canvas>().sortingOrder = 5;
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadLevelCo(nextLevel));
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void TriggerOpening()
    {
        anim.SetTrigger("End");

    }

    public void CallWinPanel()
    {
        if(winPanel != null)
        {
            winPanel.layer = 0;

            canvasPanel.GetComponent<Canvas>().sortingOrder = 4;
            winPanel.SetActive(true);
        }
        
    }

    public void CallTimeOutPanel()
    {
        timeOutPanel.layer = 0;
        canvasPanel.GetComponent<Canvas>().sortingOrder = 4;
        timeOutPanel.SetActive(true);

        if (PlayerPrefs.GetInt("HighScore") < scoreManager.score)
        {
            PlayerPrefs.SetInt("HighScore", scoreManager.score);
        }

        
    }

    IEnumerator LoadLevelCo(int index)
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(index);
    }

    public void OpenPanel()
    {
        anim.SetTrigger("OpenPanel");
    }
    public void ClosePanel()
    {
        anim.SetTrigger("ClosePanel");
    }

}
