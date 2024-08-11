using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleAnciensLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment des Anciens");
          player.oid.SetTextureOverride("paladin", "anciens");

          player.learnableSkills.TryAdd(CustomSkill.AnciensGuerisonRayonnante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensGuerisonRayonnante], player));
          player.learnableSkills[CustomSkill.AnciensGuerisonRayonnante].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensGuerisonRayonnante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.AnciensCourrouxDeLaNature, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensCourrouxDeLaNature], player));
          player.learnableSkills[CustomSkill.AnciensCourrouxDeLaNature].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensCourrouxDeLaNature].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.AnciensRenvoiDesInfideles, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensRenvoiDesInfideles], player));
          player.learnableSkills[CustomSkill.AnciensRenvoiDesInfideles].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensRenvoiDesInfideles].source.Add(Category.Class);

          PaladinUtils.LearnSermentSpell(player, CustomSpell.SpeakAnimal);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.FrappePiegeuse);

          break;

        case 5:

          PaladinUtils.LearnSermentSpell(player, CustomSpell.FouleeBrumeuse);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.RayonDeLune);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinAuraDeDevotion, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinAuraDeDevotion], player));
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinAuraDeDevotion].source.Add(Category.Class);

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeGarde(player.oid.LoginCreature, 7));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 9:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.ProtectionFromElements);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.CroissanceVegetale);

          break;

        case 13:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.Stoneskin);
          PaladinUtils.LearnSermentSpell(player, (int)Spell.IceStorm);

          break;

        case 15:

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionContreLeMalEtLeBien));

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.SentinelleImmortelle);
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;
          player.oid.LoginCreature.OnDamaged += PaladinUtils.HandleSentinelleImmortelle;
          
          break;

        case 17:

          PaladinUtils.LearnSermentSpell(player, CustomSpell.CommunionAvecLaNature);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.PassageParLesArbres);

          break;

        case 18:

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeGardeEffectTag);
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeGarde(player.oid.LoginCreature, 18));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(18);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.AnciensChampionAntique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AnciensChampionAntique], player));
          player.learnableSkills[CustomSkill.AnciensChampionAntique].LevelUp(player);
          player.learnableSkills[CustomSkill.AnciensChampionAntique].source.Add(Category.Class);

          break;
      }
    }
  }
}
