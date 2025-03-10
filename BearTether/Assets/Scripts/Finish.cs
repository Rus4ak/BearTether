using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameObject _finishMenu;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _finishMenu.SetActive(true);
    }
}
