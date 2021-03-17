using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public Animator anim;
    public GameObject canvasPanel;
    [SerializeField] GameObject winPanel;

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


    public void TriggerOpening()
    {
        anim.SetTrigger("End");

    }

    public void CallWinPanel()
    {
        winPanel.layer = 0;

        canvasPanel.GetComponent<Canvas>().sortingOrder = 4;
        winPanel.SetActive(true);
        

    }

    IEnumerator LoadLevelCo(int index)
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(index);
    }

}
