using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ClercIllumination(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1 || !caster.IsPlayerControlled)
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ClercIlluminationVariable).Value = 1;
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Illumination", StringUtils.gold, true, true);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercIllumination);
      }
      else
      {
        caster.ControllingPlayer.EnterTargetMode(SelectIlluminationTarget, Config.selectCreatureTargetMode);
        caster.ControllingPlayer.SendServerMessage("Veuillez choisir une cible", ColorConstants.Orange);
      }
    }
    private static void SelectIlluminationTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      NwCreature caster = selection.Player.LoginCreature;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ClercIlluminationVariable).Value = 1;
      StringUtils.DisplayStringToAllPlayersNearTarget(selection.Player.LoginCreature, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Illumination - {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercIllumination);
    }
  }
}
