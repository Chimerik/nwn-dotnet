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

          player.LearnClassSkill(CustomSkill.AnciensGuerisonRayonnante);
          player.LearnClassSkill(CustomSkill.AnciensCourrouxDeLaNature);
          player.LearnClassSkill(CustomSkill.AnciensRenvoiDesInfideles);

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.SpeakAnimal, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FrappePiegeuse, CustomClass.Paladin);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FouleeBrumeuse, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.RayonDeLune, CustomClass.Paladin);

          break;


        case 7:

          player.LearnClassSkill(CustomSkill.PaladinAuraDeGarde);

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 7));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ProtectionFromElements, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.CroissanceVegetale, CustomClass.Paladin);

          break;

        case 13:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Stoneskin, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.IceStorm, CustomClass.Paladin);

          break;

        case 15:

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionContreLeMalEtLeBien));

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.SentinelleImmortelle);
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;
          player.oid.LoginCreature.OnDamaged += PaladinUtils.HandleSentinelleImmortelle;
          
          break;

        case 17:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.CommunionAvecLaNature, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.PassageParLesArbres, CustomClass.Paladin);

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.AnciensChampionAntique);

          break;
      }
    }
  }
}
