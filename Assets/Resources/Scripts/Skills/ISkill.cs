using UnityEngine;

public interface ISkill
{
    string Name { get; }
    float Cooldown { get; }
    void Activate(GameObject caster);
}