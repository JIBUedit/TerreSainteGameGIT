using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void IsDead(out bool dead);
    public void TakeDamage(int damage);
    public void Heal(int hp);

}
