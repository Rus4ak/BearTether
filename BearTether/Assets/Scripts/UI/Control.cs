using UnityEngine;

public class Control : MonoBehaviour
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

    public void Apply()
    {
        Options.Instance.optionsData.buttonLeftPosition = _buttonLeft.anchoredPosition;
        Options.Instance.optionsData.buttonRightPosition = _buttonRight.anchoredPosition;
        Options.Instance.optionsData.buttonJumpPosition = _buttonJump.anchoredPosition;
        Options.Instance.Save();
    }

    public void Default()
    {
        _buttonLeft.anchoredPosition = new Vector2(300, 180);
        _buttonRight.anchoredPosition = new Vector2(550, 180);
        _buttonJump.anchoredPosition = new Vector2(-300, 300);
}
}
