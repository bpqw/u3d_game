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
        // Add Vector3.up * 1.5f to raise the spawn position by 1.5 units
        GameObject fireball = Instantiate(Prefab, caster.transform.position + caster.transform.forward + Vector3.up * 1.5f, caster.transform.rotation);
        fireball.GetComponent<Rigidbody>().velocity = caster.transform.forward * speed;

        StartCooldown();
    }
}
