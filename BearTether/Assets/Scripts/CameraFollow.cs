using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 8;

    private void FixedUpdate()
    {
        Vector3 pos = Vector3.Lerp(transform.position, _target.position, _smoothSpeed * Time.fixedDeltaTime);
        pos.z = -10;

        transform.position = pos;
    }
}
