using UnityEngine;
   
public abstract class BaseSkill : MonoBehaviour, ISkill
{
    public string Name { get; protected set; }
    public float Cooldown { get; protected set; }
    protected float lastActivationTime;

    public virtual void Activate(GameObject caster)
    {
        if (IsOnCooldown())
        {
            Debug.Log($"{caster.name} 技能正在冷却中！");
            return;
        }
    }

    protected bool IsOnCooldown()
    {
        return Time.time - lastActivationTime < Cooldown;
    }

    protected void StartCooldown()
    {
        lastActivationTime = Time.time;
    }
}