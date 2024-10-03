using UnityEngine;

public class FireballSkill : BaseSkill
{
    public GameObject Prefab;
    public float damage = 20f;
    public float speed = 10f;

    private void Awake()
    {
        Name = "火球术";
        Cooldown = 1f;
    }

    public override void Activate(GameObject caster)
    {
        if (IsOnCooldown()) return;

        Debug.Log($"{caster.name} 释放了火球术！");
        GameObject fireball = Instantiate(Prefab, caster.transform.position + caster.transform.forward, caster.transform.rotation);
        fireball.GetComponent<Rigidbody>().velocity = caster.transform.forward * speed;

        StartCooldown();
    }
}