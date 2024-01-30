using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MaitreBouclier(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
      {
        caster.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
        return;
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Maître bouclier", StringUtils.gold, true);
      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

      if (targetCreature.Size > caster.Size + 1)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est trop grande pour que vous la renversiez !", ColorConstants.Red);
        return;
      }

      EffectSystem.ApplyKnockdown(caster, targetCreature);
      CreatureUtils.HandleBonusActionCooldown(caster);
    }
  }
}
