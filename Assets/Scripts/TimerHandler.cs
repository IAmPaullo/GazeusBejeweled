using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHandler : MonoBehaviour
{

    public float duration;
    public Image fillImage;


    void Start()
    {
        fillImage.fillAmount = 1f;
        StartCoroutine(CountDown(duration));
    }

    
    public IEnumerator CountDown(float duration)
    {
        float startingTime = Time.time;
        float time = duration;
        float value = 1;


        while(Time.time - startingTime < duration)
        {
            time -= Time.deltaTime;
            value = time / duration;
            fillImage.fillAmount = value;
            yield return null;
        }
    }


    
}
