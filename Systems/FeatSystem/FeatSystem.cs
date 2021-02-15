using NLog;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;

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

      Log.Info($"{oPC.oid.Name} used feat {((Feat)onUseFeat.Feat).ToString()}");

      switch ((Feat)onUseFeat.Feat)
      {
        case Feat.Elfique:
        case Feat.Abyssal:
        case Feat.Céleste:
        case Feat.Profond:
        case Feat.Draconique:
        case Feat.Druidique:
        case Feat.Nain:
        case Feat.Géant:
        case Feat.Gobelin:
        case Feat.Halfelin:
        case Feat.Infernal:
        case Feat.Orc:
        case Feat.Primordial:
        case Feat.Sylvain:
        case Feat.Voleur:
        case Feat.Gnome:
          new Language(player, (int)onUseFeat.Feat);
          break;
        case Feat.BlueprintCopy:
        case Feat.BlueprintCopy2:
        case Feat.BlueprintCopy3:
        case Feat.BlueprintCopy4:
        case Feat.BlueprintCopy5:
        case Feat.Research:
        case Feat.Research2:
        case Feat.Research3:
        case Feat.Research4:
        case Feat.Research5:
        case Feat.Metallurgy:
        case Feat.Metallurgy2:
        case Feat.Metallurgy3:
        case Feat.Metallurgy4:
        case Feat.Metallurgy5:

          onUseFeat.Skip = true;
          Craft.Blueprint.BlueprintValidation(player, onUseFeat.TargetGameObject, (Feat)onUseFeat.Feat);
          break;

        case Feat.CustomMenuUP:
        case Feat.CustomMenuDOWN:
        case Feat.CustomMenuSELECT:
        case Feat.CustomMenuEXIT:
        case Feat.CustomPositionRight:
        case Feat.CustomPositionLeft:
        case Feat.CustomPositionForward:
        case Feat.CustomPositionBackward:
        case Feat.CustomPositionRotateRight:
        case Feat.CustomPositionRotateLeft:

          onUseFeat.Skip = true;
          oPC.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs((Feat)onUseFeat.Feat));
          break;
        case Feat.WoodProspection:
        case Feat.WoodProspection2:
        case Feat.WoodProspection3:
        case Feat.WoodProspection4:
        case Feat.WoodProspection5:

          onUseFeat.Skip = true;
          Craft.Collect.System.StartCollectCycle(
              oPC,
              player.Area,
              () => Craft.Collect.Wood.HandleCompleteProspectionCycle(oPC)
          );
          break;
        case Feat.Hunting:
        case Feat.Hunting2:
        case Feat.Hunting3:
        case Feat.Hunting4:
        case Feat.Hunting5:

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
      int feat = (int)Feat.ImprovedStrength;
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

      feat = (int)Feat.ImprovedAnimalEmpathy;
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
      for (int attackBonusfeat = (int)Feat.ImprovedAttackBonus; attackBonusfeat < (int)Feat.ImprovedAttackBonus + 5; attackBonusfeat++)
      {
        FeatPlugin.SetFeatModifier(attackBonusfeat, FeatPlugin.NWNX_FEAT_MODIFIER_AB, 1);
        value++;
      }

      feat = (int)Feat.ImprovedSpellSlot0_1;
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

      feat = (int)Feat.ImprovedSavingThrowAll;
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
