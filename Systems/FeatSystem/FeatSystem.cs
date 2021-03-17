using NLog;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using System.Collections.Generic;
using NWN.API.Constants;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public class FeatSystem
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    
    public FeatSystem(NWNXEventService nwnxEventService)
    {
      nwnxEventService.Subscribe<FeatUseEvents.OnUseFeatBefore>(OnUseFeatBefore);
    }
    private void OnUseFeatBefore(FeatUseEvents.OnUseFeatBefore onUseFeat)
    {
      if (!(onUseFeat.FeatUser is NwPlayer player))
        return;

      if (!PlayerSystem.Players.TryGetValue(player, out PlayerSystem.Player oPC))
        return;

      Log.Info($"{oPC.oid.Name} used feat {(onUseFeat.Feat).ToString()}");

      switch (onUseFeat.Feat)
      {
        case CustomFeats.Elfique:
        case CustomFeats.Abyssal:
        case CustomFeats.Céleste:
        case CustomFeats.Profond:
        case CustomFeats.Draconique:
        case CustomFeats.Druidique:
        case CustomFeats.Nain:
        case CustomFeats.Géant:
        case CustomFeats.Gobelin:
        case CustomFeats.Halfelin:
        case CustomFeats.Infernal:
        case CustomFeats.Orc:
        case CustomFeats.Primordiale:
        case CustomFeats.Sylvain:
        case CustomFeats.Voleur:
        case CustomFeats.Gnome:
          new Language(player, (int)onUseFeat.Feat);
          break;
        case CustomFeats.BlueprintCopy:
        case CustomFeats.Research:
        case CustomFeats.Metallurgy:

          onUseFeat.Skip = true;
          Craft.Blueprint.BlueprintValidation(player, onUseFeat.TargetGameObject, (Feat)onUseFeat.Feat);
          break;

        case CustomFeats.Recycler:
          new Recycler(player, onUseFeat.TargetGameObject);
          break;

        case CustomFeats.Renforcement:
          new Renforcement(player, onUseFeat.TargetGameObject);
          break;

        case CustomFeats.SurchargeArcanique:
          new SurchargeArcanique(player, onUseFeat.TargetGameObject);
          break;

        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:
        case CustomFeats.CustomPositionRight:
        case CustomFeats.CustomPositionLeft:
        case CustomFeats.CustomPositionForward:
        case CustomFeats.CustomPositionBackward:
        case CustomFeats.CustomPositionRotateRight:
        case CustomFeats.CustomPositionRotateLeft:

          onUseFeat.Skip = true;
          oPC.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat));
          break;
        case CustomFeats.WoodProspection:

          onUseFeat.Skip = true;
          Craft.Collect.System.StartCollectCycle(
              oPC,
              player.Area,
              () => Craft.Collect.Wood.HandleCompleteProspectionCycle(oPC)
          );
          break;
        case CustomFeats.Hunting:

          onUseFeat.Skip = true;
          Craft.Collect.System.StartCollectCycle(
              oPC,
              player.Area,
              () => Craft.Collect.Pelt.HandleCompleteProspectionCycle(oPC)
          );
          break;
      }
    }
    public static void InitializeFeatModifiers()
    {
      int feat = (int)CustomFeats.ImprovedStrength;
      int value = 1;
      for (int ability = NWScript.ABILITY_STRENGTH; ability <= NWScript.ABILITY_CHARISMA; ability++)
      {
        value = 1;
        while (value < 6)
        {
          FeatPlugin.SetFeatModifier(feat, FeatPlugin.NWNX_FEAT_MODIFIER_ABILITY, ability, 1);
          value++;
          feat++;
        }
      }

      feat = (int)CustomFeats.ImprovedAnimalEmpathy;
      for (int skill = NWScript.SKILL_ANIMAL_EMPATHY; skill <= NWScript.SKILL_INTIMIDATE; skill++)
      {

        if (skill == NWScript.SKILL_PERSUADE || skill == NWScript.SKILL_APPRAISE || skill == NWScript.SKILL_CRAFT_TRAP)
          continue;

        value = 1;
        while (value < 6)
        {
          SkillFeat skillFeat = new SkillFeat();
          skillFeat.iFeat = feat;
          skillFeat.iSkill = skill;
          skillFeat.iModifier = 1;
          SkillranksPlugin.SetSkillFeat(skillFeat, 1);
          value++;
          feat++;
        }
      }

      value = 1;
      for (int attackBonusfeat = (int)CustomFeats.ImprovedAttackBonus; attackBonusfeat < (int)CustomFeats.ImprovedAttackBonus + 5; attackBonusfeat++)
      {
        FeatPlugin.SetFeatModifier(attackBonusfeat, FeatPlugin.NWNX_FEAT_MODIFIER_AB, 1);
        value++;
      }

      feat = (int)CustomFeats.ImprovedSpellSlot0;
      for (int spellLevel = 0; spellLevel < 10; spellLevel++)
      {
        value = 1;
        while (value < 11)
        {
          FeatPlugin.SetFeatModifier(feat, 22, 43, spellLevel, 1); // 22 = NWNX_FEAT_MODIFIER_BONUSSPELL, 43 = class aventurier
          value++;
          feat++;
        }
      }

      feat = (int)CustomFeats.ImprovedSavingThrowAll;
      for (int savingThrow = NWScript.SAVING_THROW_ALL; savingThrow < NWScript.SAVING_THROW_WILL; savingThrow++)
      {
        value = 1;
        while (value < 6)
        {
          FeatPlugin.SetFeatModifier(feat, FeatPlugin.NWNX_FEAT_MODIFIER_SAVE, savingThrow, 1);
          value++;
          feat++;
        }
      }
    }
  }
}
