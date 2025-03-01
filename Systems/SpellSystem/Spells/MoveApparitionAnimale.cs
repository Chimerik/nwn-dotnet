using System.Linq;
using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MoveApparitionAnimale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      var replique = caster.Associates.FirstOrDefault(e => e.Tag == EffectSystem.ApparitionAnimaleTag);

      if (replique is null)
      {
        caster.LoginPlayer?.SendServerMessage("Votre Apparition Animale n'est pas invoquée", ColorConstants.Red);
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
      replique.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster2));
    }
  }
}
