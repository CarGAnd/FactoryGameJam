using UnityEngine;

public class AudioTimer : MonoBehaviour
{
    private float setTimer;

    public float timer;

    [HideInInspector]
    public bool run = false;

    private void Update()
    {
        if (!run)
            return;

        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0, Mathf.Infinity); // below zero not possible

        if (timer == 0)
        {
            //StopTimer();
            //SetTimer(setTimer);
        }
    }

    public void StartTimer()
    {
        run = true;
    }

    public void StopTimer()
    {
        run = false;
    }

    public void AddTime(float seconds)
    {
        timer += seconds;
    }

    public void SetTimer(float seconds)
    {
        timer = seconds;
    }
}