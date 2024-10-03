using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<BaseSkill> skills = new List<BaseSkill>();
    public GameObject fireballPrefab;

    public void AddSkill(BaseSkill skill)
    {
        skills.Add(skill);
    }

    public void ActivateSkill(int index, GameObject caster)
    {
        if (index >= 0 && index < skills.Count)
        {
            skills[index].Activate(caster);
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