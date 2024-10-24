using System.Collections;
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
        StartCoroutine(ActivateRoutine(caster));
    }

    private IEnumerator ActivateRoutine(GameObject caster)
    {
        Debug.Log($"{caster.name} 释放了火球术！");
        caster.GetComponent<Animator>().SetTrigger("SpellCast");
        yield return new WaitForSeconds(1.3f); // Wait until the animation ends
        caster.GetComponent<Player>().ApplyStun(1.3f);
        GameObject fireball = Instantiate(Prefab, caster.transform.position + caster.transform.forward, caster.transform.rotation);
        fireball.GetComponent<Rigidbody>().velocity = caster.transform.forward * speed;

        StartCooldown();
    }
}
