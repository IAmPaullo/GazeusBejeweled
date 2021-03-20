using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarHandler : MonoBehaviour
{
    [SerializeField] Image progressBar;
    [SerializeField] float value;
    private float maxValue;
    [SerializeField] float levelGoal;
    [SerializeField] GameObject finishFX;
    [SerializeField] GameObject finishFXParentPosition;
    private ScenesManager sceneManager;
    private Board board;
    private bool hasWon;



    private void Start()
    {
        sceneManager = FindObjectOfType<ScenesManager>();
        board = FindObjectOfType<Board>();
        progressBar.fillAmount = 0;
        maxValue = levelGoal;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            AddProgress(value);
        }

        if (progressBar.fillAmount >= 1 && !hasWon )
        {
            WinSituation();
        }
    }

    

    public void AddProgress(float amount)
    {

        progressBar.fillAmount += amount / maxValue;
    }


    void WinSituation()
    {
        hasWon = true;
        board.currentState = GameStates.wait;
        StartCoroutine(GenericTimer(1));
        Instantiate(finishFX, finishFXParentPosition.transform.position, Quaternion.identity, finishFXParentPosition.transform);
        sceneManager.CallWinPanel();
    }
   
    private IEnumerator GenericTimer(int time)
    {
        yield return new WaitForSeconds(time);
    }

}
