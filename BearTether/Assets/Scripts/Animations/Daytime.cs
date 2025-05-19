using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Daytime : MonoBehaviour
{
    [SerializeField] private GameObject _sun;
    [SerializeField] private GameObject _moon;
    [SerializeField] private Light2D _light;
    [SerializeField] private AudioSource _daySound;
    [SerializeField] private AudioSource _nightSound;

    private float _endPosX;

    public static float timeRadius = .5f;

    private void Start()
    {
        float screenHeight = Camera.main.orthographicSize * 2 + .2f;
        float screenWidth = screenHeight * Screen.width / Screen.height + .2f;

        _endPosX = screenWidth / 3;

        transform.DORotate(new Vector3(0, 0, -360), 200, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        StartMoving();
    }

    public void StartMoving()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z));
        seq.Append(transform.DOLocalMoveX(_endPosX, 50f).SetEase(Ease.Linear));

        seq.AppendCallback(() => transform.localPosition = new Vector3(_endPosX * -1, transform.localPosition.y, transform.localPosition.z));
        seq.Append(transform.DOLocalMoveX(_endPosX, 100f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart));
    }

    private void Update()
    {
        float zRot = transform.eulerAngles.z;
        float val = GetValueFromZRotation(zRot);

        timeRadius = val;

        _daySound.volume = Mathf.Clamp(val, 0, SoundVolume.Instance.volume);
        _nightSound.volume = Mathf.Clamp(val * -1, 0, SoundVolume.Instance.volume);

        if (val > 0)
        {
            _sun.SetActive(true);
            _moon.SetActive(false);
        }
        else if (val < 0)
        {
            _sun.SetActive(false);
            _moon.SetActive(true);
        }

        _light.intensity = Mathf.Clamp(val, .025f, 1);
    }

    float GetValueFromZRotation(float zRotation)
    {
        zRotation %= 360f;
        float radians = zRotation * Mathf.Deg2Rad;
        return Mathf.Cos(radians);
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }
}
