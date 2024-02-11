using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleBersekerLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(5213).SetPlayerOverride(player.oid, "Berseker");

          player.learnableSkills.TryAdd(CustomSkill.BersekerFrenziedStrike, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BersekerFrenziedStrike], player));
          player.learnableSkills[CustomSkill.BersekerFrenziedStrike].LevelUp(player);
          player.learnableSkills[CustomSkill.BersekerFrenziedStrike].source.Add(Category.Class);

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike)))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike));

          player.oid.LoginCreature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike), 0);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.BersekerRageAveugle, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BersekerRageAveugle], player));
          player.learnableSkills[CustomSkill.BersekerRageAveugle].LevelUp(player);
          player.learnableSkills[CustomSkill.BersekerRageAveugle].source.Add(Category.Class);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.BersekerPresenceIntimidante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BersekerPresenceIntimidante], player));
          player.learnableSkills[CustomSkill.BersekerPresenceIntimidante].LevelUp(player);
          player.learnableSkills[CustomSkill.BersekerPresenceIntimidante].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.BersekerRepresailles, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BersekerRepresailles], player));
          player.learnableSkills[CustomSkill.BersekerRepresailles].LevelUp(player);
          player.learnableSkills[CustomSkill.BersekerRepresailles].source.Add(Category.Class);

          break;
      }
    }
  }
}
