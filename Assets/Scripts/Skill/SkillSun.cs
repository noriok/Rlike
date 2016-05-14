using UnityEngine;
using System.Collections;

// 水がれ
public class SkillSun : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        var obj = Resources.Load("Prefabs/Effect/pipo-fog001");
        var fog = (GameObject)GameObject.Instantiate(obj);
        fog.transform.position = sender.Position;

        yield return CAction.Fade(fog, 0, 1, 1.5f);

        var mask = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Mask/white"));
        mask.transform.position = sender.Position;

        yield return CAction.Fade(mask, 0, 1, 0.8f);

        // マップ切り替え
        yield return sys.Sun();

        yield return new WaitForSeconds(1.0f);

        GameObject.Destroy(fog);
        yield return CAction.Fade(mask, 1, 0, 0.6f);
        GameObject.Destroy(mask);
    }
}
