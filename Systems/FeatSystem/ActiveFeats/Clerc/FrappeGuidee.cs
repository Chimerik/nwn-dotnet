using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeGuidee(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1 || !caster.IsPlayerControlled)
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeGuideeVariable).Value = 1;
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Guidée", StringUtils.gold, true, true);
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
        ClercUtils.ConsumeConduitDivin(caster);
      }
      else
      {
        caster.ControllingPlayer.EnterTargetMode(SelectFrappeGuideeTarget, Config.CreatureTargetMode(0, new System.Numerics.Vector2(9, 9), flags: SpellTargetingFlags.HelpsAllies));
        caster.ControllingPlayer.SendServerMessage("Veuillez choisir une cible", ColorConstants.Orange);
      }
    }
    private static void SelectFrappeGuideeTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      NwCreature caster = selection.Player.LoginCreature;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeGuideeVariable).Value = 1;
      StringUtils.DisplayStringToAllPlayersNearTarget(selection.Player.LoginCreature, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Guidée - {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
