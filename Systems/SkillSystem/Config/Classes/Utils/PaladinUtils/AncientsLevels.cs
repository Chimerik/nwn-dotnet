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

          player.LearnAlwaysPreparedSpell(CustomSpell.SpeakAnimal, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.FrappePiegeuse, CustomClass.Paladin);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.FouleeBrumeuse, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.RayonDeLuneMaster, CustomClass.Paladin);

          break;


        case 7: 
          
          player.LearnClassSkill(CustomSkill.PaladinAuraDeGarde);

          var auraDeProtection = NwFeat.FromFeatId(CustomSkill.AuraDeProtection);
          var tlk = auraDeProtection.Name;
          tlk.SetPlayerOverride(player.oid, "Aura de Garde");

          var descTlk = auraDeProtection.Description;
          descTlk.SetPlayerOverride(player.oid, $"{descTlk.ToString()}\n\n{player.learnableSkills[CustomSkill.PaladinAuraDeGarde].description}");
          player.oid.SetTextureOverride(auraDeProtection.IconResRef, NwFeat.FromFeatId(CustomSkill.PaladinAuraDeGarde).IconResRef);

          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);

          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 7));
          UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(3);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.ProtectionFromElements, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceVegetale, CustomClass.Paladin);

          break;

        case 13:
          
          player.LearnAlwaysPreparedSpell((int)Spell.Stoneskin, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell((int)Spell.IceStorm, CustomClass.Paladin);

          break;

        case 15: player.LearnClassSkill(CustomSkill.PaladinSentinelleImmortelle); break;

        case 17:

          player.LearnAlwaysPreparedSpell(CustomSpell.CommunionAvecLaNature, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.PassageParLesArbres, CustomClass.Paladin);

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.AnciensChampionAntique);

          break;
      }
    }
  }
}
