using System.Collections;
using UnityEngine;

public class FlySkill : BaseSkill
{

    private void Awake()
    {
        Name = "飞行术";
        Cooldown = 10f;
        sprite = Resources.Load<Sprite>("SkillIcon/exp");
    }

    public override void Activate(GameObject caster)
    {
        if (IsOnCooldown()) return;
        StartCoroutine(ActivateRoutine(caster));
        
    }

    private IEnumerator ActivateRoutine(GameObject caster)
    {
        Debug.Log($"{caster.name} 释放了火球术！");
        caster.GetComponent<Animator>().SetTrigger("Buff");
        caster.GetComponent<Player>().ApplyStun(0.5f);
        yield return new WaitForSeconds(0.5f); // Wait until the animation ends
        caster.GetComponent<CharacterController>().Move(new Vector3(0, 100, 0));
        StartCooldown();
    }




    // public override void Activate(GameObject caster)
    // {
    //     if (IsOnCooldown()) {
    //         Debug.Log($"{caster.name} 飞行术正在冷却中！");
    //         return;
    //     }

    //     Debug.Log($"{caster.name} 使用了飞行术！");

    //     caster.GetComponent<Animator>().SetTrigger("Buff");

    //     caster.GetComponent<CharacterController>().Move(new Vector3(0, 100, 0));
    //     Debug.Log($"{caster.name} 飞行术激活！");

    //     StartCooldown();
    // }
}
