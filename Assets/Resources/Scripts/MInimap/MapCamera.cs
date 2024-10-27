using UnityEngine;

public class MapCamera : MonoBehaviour
{
    public float mapSize = 80f;
    public float cameraHeight = 100f;
    public int textureResolution = 1024;

    private Camera mapCamera;
    private RenderTexture renderTexture;
    public float MapSize
    {
        get { return mapCamera.orthographicSize * 2; }
        set 
        { 
            mapCamera.orthographicSize = value / 2;
            CreateRenderTexture();
        }
    }
    
    void Start()
    {
        SetupCamera();
        CreateRenderTexture();
    }

    void SetupCamera()
    {
        mapCamera = GetComponent<Camera>();
        if (mapCamera == null)
        {
            mapCamera = gameObject.AddComponent<Camera>();
        }

        mapCamera.orthographic = true;
        mapCamera.orthographicSize = mapSize / 2;
        transform.position = new Vector3(0, cameraHeight, 0);
        transform.rotation = Quaternion.Euler(90, 0, 0);

        // 设置摄像机的视口矩形
        mapCamera.rect = new Rect(0, 0, 1, 1);
    }

    public Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        Vector3 viewportPosition = mapCamera.WorldToViewportPoint(worldPosition);
        return new Vector2(viewportPosition.x, viewportPosition.y);
    }

   void CreateRenderTexture()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
        renderTexture = new RenderTexture(textureResolution, textureResolution, 24);
        mapCamera.targetTexture = renderTexture;
    }

    public void UpdateCameraPosition(Vector2 offset)
    {
        Vector3 newPosition = transform.position;
        newPosition.x = offset.x;
        newPosition.z = offset.y;
        transform.position = newPosition;
    }

    public Texture2D CaptureMap()
    {
        mapCamera.Render();
        RenderTexture.active = renderTexture;
        Texture2D mapTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        mapTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        mapTexture.Apply();
        RenderTexture.active = null;
        return mapTexture;
    }

    private Camera _camera;
    public Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();
            return _camera;
        }
    }
}
