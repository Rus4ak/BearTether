using UnityEngine;

public class BackgroundSize : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float screenHeight = Camera.main.orthographicSize * 2 + .2f;
        float screenWidth = screenHeight * Screen.width / Screen.height + .2f;
        
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        transform.localScale = new Vector3(screenWidth / spriteSize.x, screenHeight / spriteSize.y, 1);
    }
}
