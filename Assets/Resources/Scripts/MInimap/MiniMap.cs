using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private RectTransform miniMap;
    private Transform player;
    private Transform enemy;
    private static Image item;
    private Image playerIcon;
    private Image enemyIcon;
    private Vector2 initialMapPosition;
    private Image viewArc;
    private float viewAngle = 60f; // 视野角度，可以根据需要调整

    private Image scanLine;
    private float scanAngle = 0f;
    private float scanSpeed = 70f; // 每秒旋转180度
    // Start is called before the first frame update

    void Start()
    {
        item = Resources.Load<Image>("Image");
        miniMap = GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemy = GameObject.FindGameObjectWithTag("Enemy")?.transform;

        if (player != null && item != null)
        {
            playerIcon = Instantiate(item);
            Sprite playerSprite = Resources.Load<Sprite>("Icon/Player");
            playerIcon.transform.SetParent(transform, false);
            playerIcon.sprite = playerSprite;

            enemyIcon = create();
            enemyIcon.transform.SetParent(transform, false);
            Sprite enemySprite = Resources.Load<Sprite>("Icon/Enemy");
            enemyIcon.sprite = enemySprite;
        }
        else
        {
            Debug.LogError("无法找到玩家或加载图像资源");
        }
        CreateScanLine();

        //Debug.Log($"Initialization complete: player={player!=null}, enemy={enemy!=null}, playerIcon={playerIcon!=null}, enemyIcon={enemyIcon!=null}, scanLine={scanLine!=null}");
    }


     private void ShowPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("player 为空");
            return;
        }

        if (playerIcon == null)
        {
            // 创建玩家图标
            playerIcon = create();
            playerIcon.transform.SetParent(transform, false);
            playerIcon.rectTransform.sizeDelta = new Vector2(10, 10);
        }

        if (viewArc == null)
        {
            // 创建视野圆弧
            viewArc = CreateViewArc();
        }

        playerIcon.rectTransform.SetAsLastSibling();
        // 更新玩家图标位置和旋转
        playerIcon.rectTransform.anchoredPosition = Vector2.zero; // 将玩家图标固定在小地图中心
        playerIcon.rectTransform.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);

        // 更新视野圆弧的旋转
        if (viewArc != null)
        {
            viewArc.rectTransform.rotation = playerIcon.rectTransform.rotation;
        }
    }

    private Image CreateViewArc()
    {
        GameObject arcObj = new GameObject("ViewArc");
        arcObj.transform.SetParent(transform, false);
        Image arc = arcObj.AddComponent<Image>();
        
        // 创建一个扇形的精灵
        Texture2D arcTexture = CreateArcTexture(100, viewAngle);
        Sprite arcSprite = Sprite.Create(arcTexture, new Rect(0, 0, arcTexture.width, arcTexture.height), new Vector2(0.5f, 0.5f));
        
        arc.sprite = arcSprite;
        arc.color = new Color(1, 1, 1, 0.3f); 
        arc.rectTransform.sizeDelta = new Vector2(93, 93); // 设置圆弧大小
        arc.rectTransform.anchoredPosition = Vector2.zero;
        
        return arc;
    }

    private Texture2D CreateArcTexture(int size, float angle)
    {
        Texture2D texture = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 2;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 direction = (pixel - center).normalized;
                float pixelAngle = Vector2.Angle(Vector2.up, direction);
                float distance = Vector2.Distance(pixel, center);

                if (distance <= radius && pixelAngle <= angle / 2)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return texture;
    }


    //敌人的显示位置应该取决于玩家的位置
    public void ShowEnemy(Transform enemy)
    {
        if (enemyIcon == null)
        {
            enemyIcon = create();
            enemyIcon.transform.SetParent(transform, false);
            Sprite enemySprite = Resources.Load<Sprite>("Icon/Enemy");
            enemyIcon.sprite = enemySprite;
        }

        if (enemyIcon == null || enemy == null || player == null)
        {
            Debug.LogWarning("enemyIcon、enemy 或 player 为空");
            return;
        }
        
        // 计算敌人相对于玩家的位置
        Vector3 relativePosition = enemy.position - player.position;
        
        // 将世界坐标转换为小地图上的坐
        float mapScale = 0.5f; // 调整这个值以改变小地图的缩放比例
        Vector2 mapPosition = new Vector2(relativePosition.x, relativePosition.z) * mapScale;
        
        // 限制敌人图标在小地图范围内
        float maxDistance = miniMap.rect.width / 2;
        mapPosition = Vector2.ClampMagnitude(mapPosition, maxDistance);

        enemyIcon.rectTransform.sizeDelta = new Vector2(5, 5);
        enemyIcon.rectTransform.anchoredPosition = mapPosition;
        enemyIcon.rectTransform.eulerAngles = new Vector3(0, 0, -enemy.eulerAngles.y);

    }
     private void CreateScanLine()
    {
        GameObject lineObj = new GameObject("ScanLine");
        lineObj.transform.SetParent(transform, false);
        scanLine = lineObj.AddComponent<Image>();
        
        // 创建一个细长的矩形作为扫描线
        Texture2D lineTexture = new Texture2D(2, 50);
        for (int y = 0; y < lineTexture.height; y++)
        {
            for (int x = 0; x < lineTexture.width; x++)
            {
                lineTexture.SetPixel(x, y, Color.white);
            }
        }
        lineTexture.Apply();
        
        Sprite lineSprite = Sprite.Create(lineTexture, new Rect(0, 0, lineTexture.width, lineTexture.height), new Vector2(0.5f, 0));
        
        scanLine.sprite = lineSprite;
        scanLine.color = new Color(1, 1, 1, 0.5f); 
        scanLine.rectTransform.sizeDelta = new Vector2(1, miniMap.rect.height / 2.1f);
        scanLine.rectTransform.anchoredPosition = Vector2.zero;
        scanLine.rectTransform.pivot = new Vector2(0.5f, 0);
    }
    void FixedUpdate()
    {
        if (playerIcon != null)
        {
            ShowPlayer();
        }

        // 检查场景中是否还有敌人
        enemy = GameObject.FindGameObjectWithTag("Enemy")?.transform;

        if (enemy != null && player != null && enemyIcon != null && scanLine != null)
        {
            // 计算敌人在小地图上的位置
            Vector3 relativePosition = enemy.position - player.position;
            float mapScale = 0.5f; // 确保这个值与 ShowEnemy 方法中的值相同
            Vector2 mapPosition = new Vector2(relativePosition.x, relativePosition.z) * mapScale;
            
            // 限制敌人图标在小地图范围内
            float maxDistance = miniMap.rect.width / 2;
            mapPosition = Vector2.ClampMagnitude(mapPosition, maxDistance);

            // 计算敌人在小地图上的角度（相对于正右方向）
            float enemyAngle = Mathf.Atan2(mapPosition.y, mapPosition.x) * Mathf.Rad2Deg;
            enemyAngle = (enemyAngle - 90f + 360f) % 360f; // 调整角度使其与扫描线角度一致

            // 计算扫描线前方的扇形区域
            float startAngle = scanAngle - 5f; // 扫描线前方5度
            float endAngle = scanAngle + 5f; // 扫描线后方5度

            // 检查敌人是否在扫描区域内
            bool inScanArea = false;
            if (startAngle < endAngle)
            {
                inScanArea = enemyAngle >= startAngle && enemyAngle <= endAngle;
            }
            else // 处理跨越0/360度的情况
            {
                inScanArea = enemyAngle >= startAngle || enemyAngle <= endAngle;
            }

            if (inScanArea)
            {
                ShowEnemy(enemy);
                enemyIcon.gameObject.SetActive(true);
            }
            else
            {
                enemyIcon.gameObject.SetActive(false);
            }

            //Debug.Log($"Scan Angle: {scanAngle}, Enemy Angle: {enemyAngle}, In Scan Area: {inScanArea}, Enemy Visible: {enemyIcon.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogWarning($"Some objects are null: enemy={enemy!=null}, player={player!=null}, enemyIcon={enemyIcon!=null}, scanLine={scanLine!=null}");
        }

        if (scanLine != null)
        {
            // 更新扫描线的旋转
            scanAngle += scanSpeed * Time.fixedDeltaTime;
            scanAngle %= 360f;
            scanLine.rectTransform.rotation = Quaternion.Euler(0, 0, -scanAngle);
        }
        else
        {
            Debug.LogWarning("扫描线为空");
        }
    }
    



    public static Image create() 
    {
        return Instantiate(item);
    }
}
