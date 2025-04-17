using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 8;
    
    private Transform _minPos;
    private Transform _maxPos;

    private float _cameraWidth, _cameraHeight;

    private void Start()
    {
        _cameraHeight = Camera.main.orthographicSize;
        _cameraWidth = _cameraHeight * Screen.width / Screen.height;

        Transform borders = GameObject.FindWithTag("CameraBorders").transform;
        _minPos = borders.Find("MinPos");
        _maxPos = borders.Find("MaxPos");
    }

    private void FixedUpdate()
    {
        Vector3 pos = Vector3.Lerp(transform.position, _target.position, _smoothSpeed * Time.fixedDeltaTime);
        pos.z = -10;

        pos.x = Mathf.Clamp(pos.x, _minPos.position.x + _cameraWidth, _maxPos.position.x - _cameraWidth);
        pos.y = Mathf.Clamp(pos.y, _minPos.position.y + _cameraHeight, _maxPos.position.y - _cameraHeight);

        transform.position = pos;
    }
}
