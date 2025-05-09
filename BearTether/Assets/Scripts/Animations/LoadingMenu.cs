using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingMenu : MonoBehaviour
{
    [SerializeField] private GameObject _loadingImage;
    [SerializeField] private TextMeshProUGUI _loadingText;

    private int _countTextDots = 1;

    private IEnumerator TextAnimation()
    {
        if (_countTextDots == 5)
        {
            _countTextDots = 1;
            _loadingText.text = "Loading<size=300%>.";
        }
        else
        {
            _loadingText.text += ".";
            _countTextDots++;
        }

        yield return new WaitForSeconds(1);

        StartCoroutine(TextAnimation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        DOTween.Clear();
    }

    private void OnEnable()
    {
        _loadingImage.transform
            .DORotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        StartCoroutine(TextAnimation());
    }
}
