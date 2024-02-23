using System.Linq;
using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Teleportation(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
      {
        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
        {
          caster.LoginPlayer?.EnterTargetMode(SelectTeleportationTarget, Config.selectLocationTargetMode);
          caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible à moins de 18 m", ColorConstants.Orange);
        }
        else
        {
          caster.LoginPlayer?.SendServerMessage("Vous devez être sous l'effet de Rage pour utiliser cette capacité", ColorConstants.Red);
          caster.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 0);
        }
      }
      else
        caster.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
    }
    private static void SelectTeleportationTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      NwCreature caster = selection.Player.LoginCreature;

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (Vector3.DistanceSquared(selection.TargetPosition, caster.Position) > 324)
      {
        selection.Player.SendServerMessage("La cible sélectionnée doit se situer à moins de 18 m", ColorConstants.Red);
        return;
      }

      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      caster.Location = Location.Create(selection.Player.LoginCreature.Area, selection.TargetPosition, selection.Player.LoginCreature.Rotation);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Magie Sauvage - Téléportation", StringUtils.gold, true);
    }
  }
}
