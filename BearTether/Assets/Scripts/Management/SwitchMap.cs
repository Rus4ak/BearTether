using UnityEngine;

public class SwitchMap : MonoBehaviour
{
    [SerializeField] private Transform[] _grounds;
    [SerializeField] private GameObject[] _backgrounds;
    [SerializeField] private ChoiceLevel _choiceLevel;
    [SerializeField] private GameObject[] _directionButtons;

    private Vector2 _moveTo;
    private int _oldChooseMap;
    private int _newChooseMap;
    private RectTransform _rectTransform;

    private int _chooseMap = 0;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_moveTo == Vector2.zero)
            return;
        
        _rectTransform.anchoredPosition = Vector3.MoveTowards(_rectTransform.anchoredPosition, _moveTo, 20);
        
        if (_rectTransform.anchoredPosition.x == _moveTo.x)
        {
            _grounds[_oldChooseMap].gameObject.SetActive(false);
            _backgrounds[_oldChooseMap].gameObject.SetActive(false);
            _grounds[_newChooseMap].gameObject.SetActive(true);
            _backgrounds[_newChooseMap].gameObject.SetActive(true);
            _choiceLevel.Ground = _grounds[_newChooseMap];

            _moveTo = Vector2.zero;

            foreach (GameObject directionButton in _directionButtons)
                directionButton.SetActive(true);
        }
    }

    public void Move(int direction)
    {
        if (_chooseMap + direction < 0 || _chooseMap + direction > _grounds.Length - 1)
            return;

        foreach (GameObject directionButton in _directionButtons)
            directionButton.SetActive(false);

        _oldChooseMap = _chooseMap;
        _chooseMap += direction;
        _newChooseMap = _chooseMap;
        _moveTo = new Vector2(_rectTransform.anchoredPosition.x + (-direction * 2500), 0);
    }
}
