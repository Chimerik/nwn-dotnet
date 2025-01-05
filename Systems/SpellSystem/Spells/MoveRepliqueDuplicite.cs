using System.Linq;
using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MoveRepliqueDuplicite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      var replique = caster.Associates.FirstOrDefault(e => e.Tag == EffectSystem.repliqueTag);

      if (replique is null)
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.MoveRepliqueDuplicite, 0);
        caster.LoginPlayer?.SendServerMessage("Votre réplique n'est pas invoquée", ColorConstants.Red);
        return;
      }

      if(Vector3.DistanceSquared(targetLocation.Position, replique.Position) > 81)
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas déplacer la réplique de plus de 9 m", ColorConstants.Red);
        return;
      }

      if (Vector3.DistanceSquared(targetLocation.Position, caster.Position) > 1600)
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas déplacer la réplique de plus de 40 m de vous", ColorConstants.Red);
        return;
      }

      replique.Location = targetLocation;
      replique.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      caster.IncrementRemainingFeatUses((Feat)CustomSkill.MoveRepliqueDuplicite);
    }
  }
}
