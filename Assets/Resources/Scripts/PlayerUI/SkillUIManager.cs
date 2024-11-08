using UnityEngine;
using System.Collections.Generic;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private GameObject skillIconPrefab;  // 技能图标预制体
    [SerializeField] private float spacing = 10f;        // 图标间距
    [SerializeField] private Vector2 firstIconPosition = new Vector2(200, -280);  // 第一个图标的位置
    
    private Dictionary<string, SkillCooldownUI> skillUIs = new Dictionary<string, SkillCooldownUI>();
    
    // 添加新技能图标
    public void AddSkillIcon(string skillId, Sprite skillSprite)
    {
        if (skillUIs.ContainsKey(skillId)) return;
        
        // 实例化技能图标
        GameObject newSkillIcon = Instantiate(skillIconPrefab, transform);
        RectTransform rectTransform = newSkillIcon.GetComponent<RectTransform>();
        
        // 设置锚点和位置
        rectTransform.anchorMin = new Vector2(0, 1); // 左上角锚点
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // 中心轴心点
        
        // 计算新图标位置
        Vector2 position = firstIconPosition + new Vector2(skillUIs.Count * (rectTransform.sizeDelta.x + spacing), 0);
        rectTransform.anchoredPosition = position;
        
        // 设置图标图片
        SkillCooldownUI cooldownUI = newSkillIcon.GetComponent<SkillCooldownUI>();
        cooldownUI.SetupSkill(skillSprite);
        
        skillUIs.Add(skillId, cooldownUI);
    }
    
    // 触发技能冷却
    public void StartCooldown(string skillId, float cooldownTime)
    {
        if (skillUIs.TryGetValue(skillId, out SkillCooldownUI ui))
        {
            ui.StartCooldown(cooldownTime);
        }
    }
}