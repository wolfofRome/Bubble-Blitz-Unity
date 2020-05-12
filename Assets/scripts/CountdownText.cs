using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{
    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;
    Text countdown;

    // Use onEnable over Start to run whenever page is called
    void OnEnable()
    {
        // Countdown starts at 3
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }

    // Update the countdown text to print and count from 3 seconds
    IEnumerator Countdown() 
    {
        int count = 3;
        for(int i = 0; i < count; i++) 
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished();
    }
}
