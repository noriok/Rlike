// using UnityEngine;
using UnityEngine.Assertions;
// using System.Collections;

public static class ItemFactory {
    private static readonly Skill SkillNone = new SkillNone();

    public static Item CreateGold() {
        var data = new ItemData(
            type: ItemType.Gold,
            name: "お金",
            desc: "お金だぞ。",
            skill: SkillNone
        );
        return new Item(data);
    }

    public static Item CreateHerb(int index) {
        return new Item(Herb(index));
    }

    public static Item CreateStone() {
        var data = new ItemData(
            type: ItemType.Stone,
            name: "魔法の石",
            desc: "投げると近くの敵に向かって飛んでいきダメージを与えるぞ。",
            skill: SkillNone
        );
        return new Item(data);
    }

    public static Item CreateMagic(int index) {
        return new Item(Magic(index));
    }

    public static Item CreateWand(int index) {
        return new Item(Wand(index));
    }

    private static ItemData CreateItemData(ItemType type, string name, string desc, Skill skill) {
        return new ItemData(type, name, desc, skill);
    }

    private static ItemData Herb(int index) {
        ItemType type = ItemType.Herb;

        switch (index) {
        case 0:
            return new ItemData(
                type: type,
                name: "薬草",
                desc: "HP が少しだけ回復するぞ。",
                skill: new SkillHeal()
            );
        case 1:
            return new ItemData(
                type: type,
                name: "睡眠草",
                desc: "睡眠状態になるぞ。",
                skill: new SkillSleep()
            );
        case 2:
            return new ItemData(
                type: type,
                name: "消え去り草",
                desc: "しばらく透明になるぞ。",
                skill: new SkillInvisible()
            );
        case 3:
            return new ItemData(
                type: type,
                name: "高飛び草",
                desc: "どこかにワープするぞ。",
                skill: new SkillWarp()
            );
        case 4:
            return new ItemData(
                type: type,
                name: "目薬草",
                desc: "見えないものが見えるようになるぞ。",
                skill: SkillNone
            );
        }

        Assert.IsTrue(false);
        return null;
    }

    private static ItemData Wand(int index) {
        var type = ItemType.Wand;

        switch (index) {
        case 0:
            return new ItemData(
                type: type,
                name: "場所替えの杖",
                desc: "場所が入れ替わるぞ。",
                skill: new SkillChange()
            );
        case 1:
            return new ItemData(
                type: type,
                name: "とびつきの杖",
                desc: "とびつくぞ。",
                skill: new SkillFly()
            );
        case 2:
            return CreateItemData(
                type: type,
                name: "ふきとばしの杖",
                desc: "モンスターを吹き飛ばすぞ。",
                skill: SkillNone
            );
        case 3:
            return CreateItemData(
                type: type,
                name: "いかずちの杖",
                desc: "いかずちのダメージを与えるぞ。",
                skill: new SkillThunder()
            );
        case 4:
            return CreateItemData(
                type: type,
                name: "かなしばりの杖",
                desc: "モンスターを動けなくするぞ。",
                skill: new SkillFreeze()
            );
        }

        Assert.IsTrue(false);
        return null;
    }

    private static ItemData Magic(int index) {
        var type = ItemType.Magic;

        switch (index) {
        case 0:
            return new ItemData(
                type: type,
                name: "睡眠の書",
                desc: "敵を眠らせるぞ。",
                skill: SkillNone
            );
        case 1:
            return new ItemData(
                type: type,
                name: "大地の怒りの書",
                desc: "部屋中の敵に大ダメージを与えるぞ。",
                skill: new SkillTitan()
            );
        case 2:
            return new ItemData(
                type: type,
                name: "地獄耳の書",
                desc: "敵の位置が分かるぞ。",
                skill: SkillNone
            );
        case 3:
            return new ItemData(
                type: type,
                name: "千里眼の書",
                desc: "アイテムの位置が分かるぞ。",
                skill: SkillNone
            );
        case 4:
            return CreateItemData(
                type: type,
                name: "水がれの書",
                desc: "フロアの水が涸れるぞ。",
                skill: new SkillSun()
            );
        }

        Assert.IsTrue(false);
        return null;
    }

}
