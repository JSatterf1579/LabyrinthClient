using UnityEngine;

// Calculates frames per second and displays it in DebugHUD
//
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10).
// We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with
// correct overall FPS even if the interval renders something like
// 5.5 frames.
public class FPSCounterForDebugHUD : MonoBehaviour {
    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    public float FPS {
        get { return fps; }
    }

    private float fps;
    void Start() {
        timeleft = updateInterval;
    }

    void Update() {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            fps = accum / frames;
            string color;

            if (fps < 30) {
                color = "yellow";
            } else if (fps < 10) {
                color = "red";
            } else {
                color = "green";
            }
            DebugHUD.setValue("FPS", string.Format("<color={0}>{1:F2}</color>", color, fps));
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}