using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleDevotionLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment de Dévotion");
          player.oid.SetTextureOverride("paladin", "devotion");

          player.learnableSkills.TryAdd(CustomSkill.DevotionArmeSacree, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionArmeSacree], player));
          player.learnableSkills[CustomSkill.DevotionArmeSacree].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionArmeSacree].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DevotionSaintesRepresailles, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionSaintesRepresailles], player));
          player.learnableSkills[CustomSkill.DevotionSaintesRepresailles].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionSaintesRepresailles].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DevotionRenvoiDesImpies, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionRenvoiDesImpies], player));
          player.learnableSkills[CustomSkill.DevotionRenvoiDesImpies].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionRenvoiDesImpies].source.Add(Category.Class);

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ProtectionContreLeMalEtLeBien, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Sanctuary, CustomClass.Paladin);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Silence, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.LesserRestoration, CustomClass.Paladin);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinAuraDeDevotion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinAuraDeDevotion], player));
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].source.Add(Category.Class);

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeDevotion(player.oid.LoginCreature, 7));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.RemoveCurse, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.LueurDespoir, CustomClass.Paladin);

          break;

        case 13:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FreedomOfMovement, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.GardienDeLaFoi, CustomClass.Paladin);

          break;

        case 15:

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionContreLeMalEtLeBien));

          break;

        case 17:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FlameStrike, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Communion, CustomClass.Paladin);

          break;

        case 18:

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeDevotionEffectTag);
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeDevotion(player.oid.LoginCreature, 18));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(9);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.DevotionNimbeSacree, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DevotionNimbeSacree], player));
          player.learnableSkills[CustomSkill.DevotionNimbeSacree].LevelUp(player);
          player.learnableSkills[CustomSkill.DevotionNimbeSacree].source.Add(Category.Class);

          break;
      }
    }
  }
}
