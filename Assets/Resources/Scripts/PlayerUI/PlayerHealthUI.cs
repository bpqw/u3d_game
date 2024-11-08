using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private Image healthBarImage;
    private Player player;

    void Start()
    {
        // 获取Image组件
        healthBarImage = GetComponent<Image>();
        
        // 获取Player组件
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        
        if (player == null)
        {
            Debug.LogError("未找到Player组件");
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // 更新血条显示
            healthBarImage.fillAmount = player.currentHealth / player.maxHealth;
        }
    }
}