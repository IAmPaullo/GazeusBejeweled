using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHandler : MonoBehaviour
{

    [SerializeField] float duration;
    [SerializeField] Image fillImage;
    [SerializeField] GameObject finishFX;
    [SerializeField] GameObject finishFXParentPosition;
    [SerializeField] ScenesManager sceneManager;
    private bool isOutOfTime;


    void Start()
    {
        
        fillImage.fillAmount = 1f;
        StartCoroutine(CountDown(duration));
    }

    private void Update()
    {
        OutOfTime();
    }

    public IEnumerator CountDown(float duration)
    {
        float startingTime = Time.time;
        float time = duration;
        float value = 1;


        while (Time.time - startingTime < duration)
        {
            time -= Time.deltaTime;
            value = time / duration;
            fillImage.fillAmount = value;
            yield return null;
        }
    }

    void OutOfTime()
    {
        if (fillImage.fillAmount == 0 && isOutOfTime == false)
        {
            isOutOfTime = true;
            Instantiate(finishFX, finishFXParentPosition.transform.position, Quaternion.identity, finishFXParentPosition.transform);
            sceneManager.CallTimeOutPanel();
        }
    }

}
