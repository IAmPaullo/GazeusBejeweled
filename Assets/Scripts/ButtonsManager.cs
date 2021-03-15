using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsManager : MonoBehaviour
{
    public Animator anim;
    public GameObject canvasPanel;

   

    public void LoadLevel(int index)
    {
        canvasPanel.layer = 0;
     
        canvasPanel.GetComponent<Canvas>().sortingOrder = 4;
        StartCoroutine(LoadLevelCo(index));
        
    }


    public void TriggerOpening()
    {
        anim.SetTrigger("End");
    }

    IEnumerator LoadLevelCo(int index)
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(index);
    }

}
