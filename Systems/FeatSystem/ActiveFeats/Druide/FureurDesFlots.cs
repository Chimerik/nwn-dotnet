using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FureurDesFlots(NwCreature caster)
    {
      int druideLevels = caster.GetClassInfo(ClassType.Druid).Level;

      if (druideLevels < 14 || !caster.IsPlayerControlled)
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWater));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FureurDesFlots(caster, druideLevels), NwTimeSpan.FromRounds(100));
        UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(druideLevels < 6 ? 3 : 9);

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Fureur des Flots", ColorConstants.Blue, true, true);
        DruideUtils.DecrementFormeSauvage(caster);
      }
      else
      {
        caster.ControllingPlayer.EnterTargetMode(SelectFureurDesFlotsTarget, Config.CreatureTargetMode(9, SpellTargetingShape.Sphere, new Vector2() { X = 9, Y = 9 }));
      }
    }
    private static void SelectFureurDesFlotsTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      NwCreature caster = selection.Player.ControlledCreature;
      int druideLevels = caster.GetClassInfo(ClassType.Druid).Level;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWater));
      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FureurDesFlots(caster, druideLevels), NwTimeSpan.FromRounds(100));
      UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(druideLevels < 6 ? 3 : 9);

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Fureur des Flots", ColorConstants.Blue, true, true);
      DruideUtils.DecrementFormeSauvage(caster);
    }
  }
}
