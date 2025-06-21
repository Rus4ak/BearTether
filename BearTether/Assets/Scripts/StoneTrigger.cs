using UnityEngine;

public class StoneTrigger : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _stone;
    [SerializeField] private float _force;
    [SerializeField] private int _direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _stone.AddForce(new Vector2(_direction * _force, 0));
        }
    }
}
