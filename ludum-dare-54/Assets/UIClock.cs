using UnityEngine;
using UnityEngine.UI;

public class UIClock : MonoBehaviour
{
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
}
