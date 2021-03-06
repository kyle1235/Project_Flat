using UnityEngine;
using System.Collections;

public class PowerKick : Ability
{
    Vector3 attackForce;

    public PowerKick(int level)
    {
        this.level = level;
        isPassive = false;
        rangedAttack = false;
        attackCoolDown = 0.5f;
    }

    public override void Attack(Animation animation, float directionX)
    {
        if (directionX == 1)
            animation.Play("powerKickLeft");
        else
            animation.Play("powerKickRight");
    }

    public override void AttackSuccess(GameObject enemyPlayer)
    {
        enemyPlayer.transform.networkView.RPC("knockBack", RPCMode.All, attackForce);
    }

    public override void AttackMiss()
    {
        
    }
}
