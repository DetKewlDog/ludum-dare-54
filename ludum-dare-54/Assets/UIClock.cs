using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIClock : MonoBehaviour
{
    public GameObject clock, alarmClock;
    public Image clockFill;
    public RectTransform clockHand;

    protected Vector3 eulerAngles = Vector3.zero;

    protected float fill = 0;
    public float Fill {
        get => fill;
        set => SetFill(value);
    }


    public void SetPercentage(float value) => SetFill(value);
    public void Reset() => SetFill(0);

    protected void SetFill(float value) {
        clockFill.fillAmount = fill = value;
        eulerAngles.z = value * 360f;
        clockHand.eulerAngles = eulerAngles;
    }

    public void ToggleAlarmClock(bool value) {
        clock.SetActive(!value);
        alarmClock.SetActive(value);
        if (value) StartCoroutine(AlarmSoundCo());
    }

    IEnumerator AlarmSoundCo() {
        var waitForSeconds = new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < 3; i++) {
            SoundManager.Play("alarm");
            yield return waitForSeconds;
        }
    }
}
