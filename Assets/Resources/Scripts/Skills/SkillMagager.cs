using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<BaseSkill> skills = new List<BaseSkill>();
    public GameObject fireballPrefab;
    [SerializeField] private SkillUIManager skillUIManager;
    //[SerializeField] private Sprite[] testSkillSprites; // 在Inspector中拖入一些测试用的技能图标
    public void AddSkill(BaseSkill skill)
    {
        skills.Add(skill);
        //skillUIManager.AddSkillIcon(skill.Name, skill.sprite);
        Debug.Log("添加"+skill.Name);
    }

    public void ActivateSkill(int index, GameObject caster)
    {
        if (index >= 0 && index < skills.Count)
        {
            skills[index].Activate(caster);
            skillUIManager.StartCooldown(skills[index].Name, skills[index].Cooldown);
        }
    }

    public void InitializeSkills()
    {
        foreach (BaseSkill skill in skills)
        {
            if (skill is FireballSkill fireballSkill)
            {
                fireballSkill.Prefab = fireballPrefab;
                Debug.Log("FireballSkill initialized");
            }
        }
    }
}