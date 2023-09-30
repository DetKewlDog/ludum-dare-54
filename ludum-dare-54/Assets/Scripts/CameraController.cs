using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float Speed = 5;
    protected Vector3 pos;

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = target.position;
        pos.z = -10;
        transform.position = Vector3.Slerp(transform.position, pos, Speed * Time.deltaTime);
    }
}
