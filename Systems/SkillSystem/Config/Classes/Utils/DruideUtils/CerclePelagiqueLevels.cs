using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleCerclePelagiqueLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(3).SetPlayerOverride(player.oid, "Cercle Pélagique");
          player.oid.SetTextureOverride("druide", "druide_mer");

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.RayOfFrost, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.NappeDeBrouillard, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Fracassement, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player,(int)Spell.GustOfWind, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player,(int)Spell.Balagarnsironhorn, CustomClass.Druid);

          player.learnableSkills.TryAdd(CustomSkill.DruideFureurDesFlots, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DruideFureurDesFlots], player));
          player.learnableSkills[CustomSkill.DruideFureurDesFlots].LevelUp(player);
          player.learnableSkills[CustomSkill.DruideFureurDesFlots].source.Add(Category.Class);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.LightningBolt, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.RespirationAquatique, CustomClass.Druid);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.IceStorm, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ControleDeLeau, CustomClass.Druid);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldMonster, CustomClass.Druid);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.SummonCreatureVii, CustomClass.Druid);

          break;
      }
    }
  }
}
