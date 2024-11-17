using System;
using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ActionSurge(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ActionSurgeEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage("Vous bénéficiez déjà de cet effet", ColorConstants.Orange);
        return;
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.ActionSurge);
      FeatUtils.DecrementFeatUses(caster, CustomSkill.FighterSurge);

      if(caster.KnowsFeat((Feat)CustomSkill.EldritchKnightChargeArcanique))
        caster.LoginPlayer?.EnterTargetMode(SelectDestination, Config.LocationTargetMode(9, SpellTargetingShape.Sphere, new Vector2() { X = 1, Y = 1}));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Fougue Martiale".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
    }
    private static void SelectDestination(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      NwCreature caster = selection.Player.ControlledCreature;

      var casterPos = CreaturePlugin.ComputeSafeLocation(caster, selection.TargetPosition, 2);

      if (casterPos != Vector3.Zero)
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
        caster.Position = casterPos;
        Location.Create(selection.Player.ControlledCreature.Area, selection.TargetPosition, selection.Player.ControlledCreature.Rotation).ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Charge Arcanique", StringUtils.gold, true);
      }
      else
        caster.ControllingPlayer?.SendServerMessage("Charge arcanique impossible", ColorConstants.Red);     
    }
  }
}
