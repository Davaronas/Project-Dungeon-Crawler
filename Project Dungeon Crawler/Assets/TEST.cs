using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    [SerializeField] bool IsCoronaAlive = true;
    [SerializeField] bool StayAtHomeRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForCorona());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckForCorona()
    {
        while(true)
        {
            while(IsCoronaAlive)
            {
                StayAtHome();
                yield return new WaitForEndOfFrame();
            }

            StayAtHomeRunning  = false;

            yield return new WaitForEndOfFrame();
        }

    }

    void StayAtHome()
    {
        // Eat();
        // Sleep();
        StayAtHomeRunning = true;
       // StayAtHome(); // Repeat()
    }
}
