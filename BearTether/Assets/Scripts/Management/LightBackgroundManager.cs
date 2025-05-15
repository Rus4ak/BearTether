using UnityEngine;

public class LightBackgroundManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _day;
    [SerializeField] private SpriteRenderer[] _night = new SpriteRenderer[2];
    [SerializeField] private GameObject _nightGO;

    private void Start()
    {
        _nightGO.SetActive(true);
    }

    private void Update()
    {
        if (Daytime.timeRadius > 0)
        {
            Color color = _day.color;
            color.a = Daytime.timeRadius;
            _day.color = color;

            foreach (var night in _night)
            {
                Color colorN = night.color;
                colorN.a = 1 - Daytime.timeRadius;
                night.color = colorN;
            }
        }
    }
}
