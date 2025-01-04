using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void ChargeDuSanglier(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature || caster == targetObject)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

      if (companion is null)
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireChargeSanglier, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
        return;
      }

      caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);

      if (Vector3.DistanceSquared(targetObject.Position, companion.Position) < 80)
      {
        caster.LoginPlayer?.SendServerMessage("La cible sélectionnée doit se situer à plus de 9 m", ColorConstants.Red);
        return;
      }

      caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireChargeSanglier, 0);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.BelluaireChargeSanglier));

      EffectUtils.RemoveTaggedEffect(companion, EffectSystem.SprintEffectTag);

      Location target = Location.Create(companion.Area, companion.Position, companion.Rotation);

      _ = companion.ClearActionQueue();
      _ = companion.AddActionToQueue(() => _ = companion.ActionForceMoveTo(target, true));
      _ = companion.AddActionToQueue(() => _ = companion.ActionAttackTarget(targetObject, true));

      companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.ChargeDuSanglierAura(companion), NwTimeSpan.FromRounds(1));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Charge du Sanglier", ColorConstants.Red, true);

      await NwTask.NextFrame();
      caster.Commandable = false;
    }
  }
}
