using System.Collections;
using UnityEngine;

public class WaveEndScreen : MonoBehaviour
{
    public RectTransform Hider;
    public float toggleDuration = 1;

    public void ToggleHider(bool toggle, System.Action callback) => StartCoroutine(ToggleHiderCo(toggle, callback));

    IEnumerator ToggleHiderCo(bool toggle, System.Action callback) {
        float startY = toggle ? 2200 : 0;
        float stopY = toggle ? 0 : -2200;
        float startTime = Time.realtimeSinceStartup;
        float time = 0;
        Vector3 position = Hider.localPosition;
        while (time < toggleDuration) {
            position.y = Mathf.Lerp(startY, stopY, time / toggleDuration);
            Hider.localPosition = position;
            time = Time.realtimeSinceStartup - startTime;
            yield return null;
        }
        callback();
    }
}
