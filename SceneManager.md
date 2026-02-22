
### Что умеет этот Scene Manager
- Singleton + `DontDestroyOnLoad`
- Enum сцен (чтобы не писать строки вручную)
- Асинхронная загрузка с красивым loading screen
- Плавный fade-in/fade-out
- Progress bar + текст "%"
- Поддержка Additive сцен
- Restart текущей сцены
- События (можно подписаться)

### Шаг 1: Настройка в сцене (один раз)
1. Создай **пустой GameObject** → назови `SceneManager`.
2. Создай UI (Canvas → Screen Space - Overlay):
   - Panel (назови `LoadingScreen`)
     - Image (background, чёрный/тёмный)
     - Slider (назови `ProgressBar`)
     - Text (назови `LoadingText`)
   - Добавь `CanvasGroup` на `LoadingScreen`
3. Прикрепи скрипт ниже к `SceneManager`.

### Шаг 2: Код (ScenesManager.cs)

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance { get; private set; }

    // === НАЗВАНИЯ СЦЕН ДОЛЖНЫ ТОЧНО СОВПАДАТЬ С ИМЕНАМИ В BUILD SETTINGS ===
    public enum SceneType
    {
        MainMenu,
        Game,
        Shop,
        Level1,
        Level2,
        // Добавляй сюда свои сцены
    }

    [Header("Loading Screen UI")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text loadingText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Скрываем загрузку при старте
        if (loadingScreen) loadingScreen.SetActive(false);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ========== ОСНОВНЫЕ МЕТОДЫ ДЛЯ ВЫЗОВА ==========

    public void LoadScene(SceneType scene, bool additive = false)
    {
        string sceneName = scene.ToString();
        StartCoroutine(LoadSceneCoroutine(sceneName, additive));
    }

    public void RestartCurrentScene()
    {
        Scene current = SceneManager.GetActiveScene();
        if (Enum.TryParse<SceneType>(current.name, out SceneType sceneType))
            LoadScene(sceneType);
        else
            SceneManager.LoadScene(current.name); // fallback
    }

    public void LoadAdditive(SceneType scene) => LoadScene(scene, true);

    // ========== КОРУТИНА ЗАГРУЗКИ ==========

    private IEnumerator LoadSceneCoroutine(string sceneName, bool additive)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f, 0.4f));

        loadingScreen.SetActive(true);
        progressBar.value = 0;
        loadingText.text = "Загрузка...";

        LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = progress;
            loadingText.text = $"Загрузка... {Mathf.RoundToInt(progress * 100)}%";
            yield return null;
        }

        // Искусственная пауза (чтобы игрок увидел 100%)
        yield return new WaitForSeconds(0.3f);

        op.allowSceneActivation = true;

        while (!op.isDone) yield return null;

        // Fade in новую сцену
        yield return StartCoroutine(Fade(0f, 0.4f));
        loadingScreen.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float start = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, time / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[ScenesManager] Загружена сцена: <color=cyan>{scene.name}</color>");
        // Здесь можно добавить свои события
    }

    // ========== УДОБНЫЕ ШОРТКАТЫ ==========
    public void LoadMainMenu() => LoadScene(SceneType.MainMenu);
    public void LoadGame() => LoadScene(SceneType.Game);
    public void LoadShop() => LoadScene(SceneType.Shop);
}
```

### Как пользоваться

```csharp
// В любом другом скрипте
ScenesManager.Instance.LoadGame();
// или
ScenesManager.Instance.LoadScene(ScenesManager.SceneType.Level1);
// или аддитивно
ScenesManager.Instance.LoadAdditive(ScenesManager.SceneType.Shop);
```
