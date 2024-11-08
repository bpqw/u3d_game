using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TextMeshProUGUI cooldownText;
    
    private float cooldownTime;
    private float currentCooldown = 0f;
    private bool isInCooldown = false;

    // 设置技能信息
    public void SetupSkill(Sprite iconSprite)
    {
        skillIcon.sprite = iconSprite;
        cooldownMask.fillAmount = 0;
        cooldownText.text = "";
    }

    void Update()
    {
        if (isInCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                isInCooldown = false;
                cooldownMask.fillAmount = 0;
                cooldownText.text = "";
            }
            else
            {
                cooldownMask.fillAmount = currentCooldown / cooldownTime;
                cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
            }
        }
    }

    public void StartCooldown(float cooldown)
    {
        if(!isInCooldown) {
            cooldownTime = cooldown;
            currentCooldown = cooldown;
            isInCooldown = true;
        }
        
    }
}