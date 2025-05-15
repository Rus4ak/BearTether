using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    private void Update()
    {
        if (Daytime.timeRadius < .25f)
            _light.SetActive(true);
        else
            _light.SetActive(false);
    }
}
