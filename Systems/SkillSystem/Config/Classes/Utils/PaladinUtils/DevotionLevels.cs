using Anvil.API;
using NWN.Core;
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

          PaladinUtils.LearnSermentSpell(player, CustomSpell.ProtectionContreLeMalEtLeBien);
          PaladinUtils.LearnSermentSpell(player, (int)Spell.Sanctuary);

          break;

        case 5:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.Silence);
          PaladinUtils.LearnSermentSpell(player, (int)Spell.LesserRestoration);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinAuraDeDevotion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinAuraDeDevotion], player));
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].source.Add(Category.Class);

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeDevotion(7)));

          break;

        case 9:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.RemoveCurse);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.LueurDespoir);

          break;

        case 13:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.FreedomOfMovement);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.GardienDeLaFoi);

          break;

        case 15:

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionContreLeMalEtLeBien));

          break;

        case 17:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.FlameStrike);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.Communion);

          break;

        case 18:

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeDevotionEffectTag);
          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeDevotion(18)));

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
