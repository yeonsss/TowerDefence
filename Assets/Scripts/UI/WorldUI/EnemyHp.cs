using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UI;
using Unity.Mathematics;
using UnityEngine.UI;

public class EnemyHp : BaseUI
{
    private enum Images
    {
        Hp
    }

    public override void Init()
    {
        Bind<SpriteRenderer>(typeof(Images));
    }

    public void ResetHpUi()
    {
        Get<SpriteRenderer>((int)Images.Hp).transform.localScale = new Vector3(1, 1, 1);
    }
    
    public void HpUpdate(int maxHp, int currentHp)
    {
        var diff = (float)currentHp / (float)maxHp;
        float offset = Mathf.Clamp(diff, 0, 1);
        Get<SpriteRenderer>((int)Images.Hp).transform.localScale = new Vector3(offset, 1, 1);
    }
}
