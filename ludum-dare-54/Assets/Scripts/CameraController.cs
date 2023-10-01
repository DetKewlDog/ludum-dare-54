using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform target;
    public float Speed = 5;
    protected Vector3 pos;

    void Awake() => Instance = this;
    void Start() => StartCoroutine(UpdateCo());
    IEnumerator UpdateCo() {
        while (true) {
            pos = target.position;
            pos.z = -10;
            transform.position = Vector3.Slerp(transform.position, pos, Speed * Time.unscaledDeltaTime);
            yield return null;
        }
    }
}
