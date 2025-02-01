//via: https://discussions.unity.com/t/accurate-frames-per-second-count/21088/6

using UnityEngine;

public class FpsMonitor
{
    const float MeasurePeriod = 0.5f;
    private int i = 0;
    private float NextPeriod = 0;
    public int currentFps;

    //Method which is automatically called before the first frame update  
    public void Start()
    {
        NextPeriod = Time.realtimeSinceStartup + MeasurePeriod;
    }

    public void Update()
    {
        i++;

        if (Time.realtimeSinceStartup < NextPeriod)
            return;

        currentFps = (int)(i / MeasurePeriod);
        NextPeriod += MeasurePeriod;
        i = 0;
    }
}

