using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayonDeLuneDeplacement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      foreach(var aoe in NwObject.FindObjectsWithTag<NwAreaOfEffect>(EffectSystem.RayonDeLuneEffectTag))
      {
        if(aoe.Creator == oCaster)
        {
          if(aoe.Location.Area == targetLocation.Area
          && Vector3.DistanceSquared(aoe.Position, targetLocation.Position) < 324)
          {
            aoe.Position = targetLocation.Position;
          }
          else
          {
            if (oCaster is NwCreature castingCreature)
              castingCreature.LoginPlayer?.SendServerMessage("Cible hors de portée", ColorConstants.Red);
          }

          return;
        }
      }

      if(oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage("Vous n'avez aucun rayon de lune actif", ColorConstants.Orange);
    }
  }
}
