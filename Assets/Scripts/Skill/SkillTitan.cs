using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillTitan : Skill {

    private IEnumerator MaskFade(GameObject obj, float alphaFrom, float alphaTo) {
        var renderer = obj.GetComponent<SpriteRenderer>();

        float duration = 0.8f;
        float elapsed = 0;
        while (elapsed <= duration) {
            float a = Mathf.Lerp(alphaFrom, alphaTo, elapsed / duration);
            var color = renderer.color;
            color.a = a;
            renderer.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
       yield return EffectAnim.Aura(sender.Position);

        sys.HideMinimap();

        // 黒マスクで画面全体を暗くする
        var mask = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Mask/black"), sender.Position, Quaternion.identity);
        mask.transform.localScale = new Vector3(10f, 15f, 1);

        float cameraZoomDelta = 0.65f;
        yield return Anim.Par(sys,
                              () => sys.CameraZoomOut(cameraZoomDelta),
                              () => MaskFade(mask, 0, 0.5f));

        yield return Anim.Par(sys,
                              () => CAction.Quake(GameObject.Find(LayerName.Map), 1.4f),
                              () => EffectAnim.Skill(sender.Position));
        yield return new WaitForSeconds(0.3f);

        yield return Anim.Par(sys,
                              () => sys.CameraZoomIn(cameraZoomDelta),
                              () => MaskFade(mask, 0.5f, 0));
        GameObject.Destroy(mask);

        sys.ShowMinimap();

        var enemies = sys.GetEnemies();
        var damageAnims = new List<DamageWait>();
        for (int i = 0; i < enemies.Length; i++) {
            damageAnims.Add(new DamageWait(enemies[i], sys));
        }

        while (true) {
            bool finished = true;
            foreach (var d in damageAnims) {
                finished = finished && d.AnimationFinished;
            }

            if (finished) break;
            yield return null;
        }
    }
}
