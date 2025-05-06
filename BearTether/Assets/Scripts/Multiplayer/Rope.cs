using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private Transform _anchor1;
    [SerializeField] private Transform _anchor2;

    private Transform _player1;
    private Transform _player2;

    public void InstantiateRope(Transform player1, Transform player2)
    {
        transform.position = player2.position - player1.position;
        _player1 = player1;
        _player2 = player2;
    }

    private void Update()
    {
        _anchor1.position = _player1.position;
        _anchor2.position = _player2.position;
    }
}
