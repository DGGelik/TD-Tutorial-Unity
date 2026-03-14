```chsharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private EnemyHealth health;

    private void Awake()
    {
        health = GetComponentInParent<EnemyHealth>();

        // === Создаём белый спрайт прямо в коде (один раз) ===
        if (fillImage.sprite == null)
        {
            Texture2D whiteTex = new Texture2D(1, 1);
            whiteTex.SetPixel(0, 0, Color.white);
            whiteTex.Apply();

            Sprite whiteSprite = Sprite.Create(whiteTex, new Rect(0, 0, 1, 1), Vector2.zero);

            fillImage.sprite = whiteSprite;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0; // слева направо
        }

        // Делаем бар зелёным (можно поменять на любой цвет)
        fillImage.color = new Color(0f, 1f, 0f); // чисто зелёный
    }

    private void LateUpdate()
    {
        if (health == null) return;

        fillImage.fillAmount = health.GetHealthNormalized();

        // Поворот к камере (чтобы бар всегда смотрел на игрока)
        transform.rotation = Camera.main.transform.rotation;
    }
}
```