using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

// 遠距離攻撃
public class ActEnemyLongDistanceAttack : Act {
    private CharacterBase _target;
    private Loc _targetLoc;

    public ActEnemyLongDistanceAttack(Enemy enemy, CharacterBase target, Loc targetLoc) : base(enemy) {
        Assert.IsTrue(target is Player);
        _target = target;
        _targetLoc = targetLoc;

    }

    public override bool IsInvalid() {
        if (_targetLoc != _target.Loc) {
            Debug.LogFormat("位置が変わりました {0} -> {1}", _targetLoc, _target.Loc);
            return true;
        }
        return false;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        // ターゲットの方を向く
        Actor.ChangeDir(Actor.Loc.Toward(_targetLoc));

        // 魔法弾を飛ばす
        var obj = Resources.Load("Prefabs/Effect/magic-ball");
        var gobj = (GameObject)GameObject.Instantiate(obj);
        yield return CAction.Move(gobj, Actor.Loc, _targetLoc);
        GameObject.Destroy(gobj);
        // いかずち
        yield return new SkillThunder().Hit(Actor, _target, sys);
    }

    public override void Apply(MainSystem sys) {
        Debug.Log("遠距離攻撃を行いました");
    }
}
