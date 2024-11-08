using UnityEngine;

public class SkillUITest : MonoBehaviour
{
    [SerializeField] private SkillUIManager skillUIManager;
    [SerializeField] private Sprite[] testSkillSprites; // 在Inspector中拖入一些测试用的技能图标
    
    void Update()
    {
        // 按1键添加第一个技能
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testSkillSprites.Length > 0)
            {
                skillUIManager.AddSkillIcon("Skill1", testSkillSprites[0]);
                Debug.Log("Added Skill 1");
            }
        }
        
        // 按2键添加第二个技能
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (testSkillSprites.Length > 1)
            {
                skillUIManager.AddSkillIcon("Skill2", testSkillSprites[1]);
                Debug.Log("Added Skill 2");
            }
        }
        
        // 按Q键触发第一个技能冷却
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillUIManager.StartCooldown("Skill1", 5f);
            Debug.Log("Skill 1 Cooldown Started");
        }
        
        // 按W键触发第二个技能冷却
        if (Input.GetKeyDown(KeyCode.R))
        {
            skillUIManager.StartCooldown("Skill2", 8f);
            Debug.Log("Skill 2 Cooldown Started");
        }
    }
}