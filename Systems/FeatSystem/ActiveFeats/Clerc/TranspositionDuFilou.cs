using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TranspositionDuFilou(NwCreature caster)
    {
      var replique = caster.Associates.FirstOrDefault(e => e.Tag == EffectSystem.repliqueTag);

      if (replique is null)
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.TeleportRepliqueDuplicite, 0);
        caster.LoginPlayer?.SendServerMessage("Votre réplique n'est pas invoquée", ColorConstants.Red);
        return;
      }

      if(caster.DistanceSquared(replique) > 1600)
      {
        caster.LoginPlayer?.SendServerMessage("Votre réplique n'est pas à portée", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      (replique.Location, caster.Location) = (caster.Location, replique.Location);
      replique.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      caster.IncrementRemainingFeatUses((Feat)CustomSkill.TeleportRepliqueDuplicite);
    }
  }
}
