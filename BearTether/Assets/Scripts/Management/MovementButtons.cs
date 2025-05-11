using UnityEngine;

public class MovementButtons : MonoBehaviour
{
    [SerializeField] private RectTransform _buttonLeft;
    [SerializeField] private RectTransform _buttonRight;
    [SerializeField] private RectTransform _buttonJump;

    private void Start()
    {
        _buttonLeft.anchoredPosition = Options.Instance.optionsData.buttonLeftPosition;
        _buttonRight.anchoredPosition = Options.Instance.optionsData.buttonRightPosition;
        _buttonJump.anchoredPosition = Options.Instance.optionsData.buttonJumpPosition;
    }
}
