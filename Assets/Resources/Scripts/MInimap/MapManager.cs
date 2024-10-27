using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public MapCamera mapCamera;
    public RawImage mapDisplay;
    public KeyCode toggleKey = KeyCode.M;

    public Canvas miniMapCanvas; // 引用小地图的 Canvas
    public Canvas mapCanvas;     // 引用大地图的 Canvas
    public float panSpeed = 10f;
    public float zoomSpeed = 20f;
    public float minZoom = 10f;
    public float maxZoom = 200f;
    private Vector2 panOffset;
    private bool isMapVisible = false;

    public float mapDisplayScale = 0.4f; // 控制地图显示的大小
    private Image playerIcon; // 玩家图标Image组件
    public Transform player; // 玩家Transform引用

    private const string PlayerIconPath = "Icon/Player"; // 图标在 Resources 文件夹中的路径

    void Start()
    {
        // 初始状态下隐藏大地图
        mapCanvas.gameObject.SetActive(false);

        // 初始化玩家图标
        InitializePlayerIcon();
    }

    void InitializePlayerIcon()
    {
        // 从 Resources 文件夹加载 Sprite
        Sprite playerSprite = Resources.Load<Sprite>(PlayerIconPath);
        
        if (playerSprite != null)
        {
            // 创建一个新的 GameObject 并添加 Image 组件
            GameObject iconObject = new GameObject("PlayerIcon");
            playerIcon = iconObject.AddComponent<Image>();
            playerIcon.sprite = playerSprite;
            
            // 设置图标的父对象为 mapCanvas
            iconObject.transform.SetParent(mapCanvas.transform, false);
            
            // 设置图标的大小和位置
            RectTransform rectTransform = playerIcon.rectTransform;
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(20, 20); // 设置适当的大小
            
            // 确保图标初始时是隐藏的
            playerIcon.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("无法加载玩家图标，请检查路径是否正确：" + PlayerIconPath);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMap();
        }

        if (isMapVisible)
        {
            HandleMapInput();
            UpdatePlayerIconPosition();
        }
    }

    void HandleMapInput()
    {
        // 平移
        Vector2 movement = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) movement.y += 1;
        if (Input.GetKey(KeyCode.S)) movement.y -= 1;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;

        panOffset += movement.normalized * panSpeed * Time.unscaledDeltaTime;

        // 缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newSize = mapCamera.MapSize - scroll * zoomSpeed;
        mapCamera.MapSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        UpdateMap();
    }

    private void ToggleMap()
    {
        isMapVisible = !isMapVisible;
        
        if (isMapVisible)
        {
            ShowMap();
            PauseGame();
        }
        else
        {
            HideMap();
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    void AdjustMapSize()
    {
        float mapSize = 80;
        mapDisplay.rectTransform.sizeDelta = new Vector2(mapSize, mapSize);
        
        mapDisplay.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        mapDisplay.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        mapDisplay.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void ShowMap()
    {
        mapCanvas.gameObject.SetActive(true);
        miniMapCanvas.gameObject.SetActive(false); // 隐藏小地图
        UpdateMap();
        AdjustMapSize();
        
        // 显示玩家图标
        if (playerIcon != null)
        {
            playerIcon.gameObject.SetActive(true);
        }

        // 隐藏实际的玩家模型
        if (player != null)
        {
            player.gameObject.SetActive(false);
        }
    }

    private void HideMap()
    {
        mapCanvas.gameObject.SetActive(false);
        miniMapCanvas.gameObject.SetActive(true); // 显示小地图
        
        // 隐藏玩家图标
        if (playerIcon != null)
        {
            playerIcon.gameObject.SetActive(false);
        }

        // 显示实际的玩家模型
        if (player != null)
        {
            player.gameObject.SetActive(true);
        }
    }

    void UpdateMap()
    {
        mapCamera.UpdateCameraPosition(panOffset);
        Texture2D mapTexture = mapCamera.CaptureMap();
        mapDisplay.texture = mapTexture;
    }

    void UpdatePlayerIconPosition()
    {
        if (playerIcon != null && player != null && mapCamera != null)
        {
            // 获取玩家在世界空间中的位置
            Vector3 playerWorldPos = player.position;

            // 将玩家世界坐标转换为视图坐标
            Vector3 viewportPoint = mapCamera.Camera.WorldToViewportPoint(playerWorldPos);

            // 将视图坐标转换为UI坐标
            Vector2 playerUIPos = new Vector2(
                viewportPoint.x * mapDisplay.rectTransform.rect.width,
                viewportPoint.y * mapDisplay.rectTransform.rect.height
            );

            // 调整UI坐标，使其相对于mapDisplay的中心
            playerUIPos -= mapDisplay.rectTransform.rect.size * 0.5f;

            // 设置图标位置
            playerIcon.rectTransform.anchoredPosition = playerUIPos;

            // 更新玩家图标的旋转，使其朝向与玩家一致
            playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }
    }
}
