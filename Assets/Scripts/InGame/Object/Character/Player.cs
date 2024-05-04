using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    float manualEndTime = 0.0f;
    private Vector2 manualMovement = Vector2.zero;
    GameObject followEffect;

    protected override void Awake()
    {
        base.Awake();
        followEffect = Instantiate(EffectManager.instance.followEffectPrefab, transform);
        followEffect.SetActive(false);

        HPBarUI hpBarUI = UIManager.instance.CreatePlayerHPBar(this.transform);
        hpBarUI.Initialize(this);
    }

    public override List<Character> GetTargetList()
    {
        return GameManager.instance.GetEnemyList();
    }

    public override List<Character> GetAllyList()
    {
        return GameManager.instance.GetHeroList(true);
    }

    protected override void UpdateVariable()
    {
    }

    protected override void UpdateManual()
    {
        base.UpdateManual();

        if (curStateTime > manualEndTime)
        {
            ChangeState(State.Idle);
        }
    }

    public void ManualMove(Vector2 movement)
    {
        if (movement.sqrMagnitude > 0)
        {
            ChangeState(State.Manual);
            Move(movement);
        }
    }

    public void OnFollowCall()
    {
        followEffect.SetActive(false);
        followEffect.SetActive(true);
    }
}
