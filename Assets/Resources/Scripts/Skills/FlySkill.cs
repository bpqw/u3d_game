using UnityEngine;

public class FlySkill : BaseSkill
{

    private void Awake()
    {
        Name = "飞行术";
        Cooldown = 10f;
    }

    public override void Activate(GameObject caster)
    {
        if (IsOnCooldown()) {
            Debug.Log($"{caster.name} 飞行术正在冷却中！");
            return;
        }

        Debug.Log($"{caster.name} 使用了飞行术！");

        caster.GetComponent<CharacterController>().Move(new Vector3(0, 100, 0));
        Debug.Log($"{caster.name} 飞行术激活！");

        StartCooldown();
    }
}