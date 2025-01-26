using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SphereDeFeuDeplacement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      foreach(var aoe in NwObject.FindObjectsWithTag<NwAreaOfEffect>(EffectSystem.SphereDeFeuEffectTag))
      {
        if(aoe.Creator == oCaster)
        {
          if(aoe.Location.Area == targetLocation.Area
          && Vector3.DistanceSquared(aoe.Position, targetLocation.Position) < 81)
          {
            Ability castAbility = (Ability)aoe.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value;
            targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.SphereDeFeu(oCaster, castAbility), aoe.RemainingDuration);
            
            var newAOE = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
            newAOE.Tag = EffectSystem.SphereDeFeuEffectTag;
            newAOE.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value = (int)castAbility;

            aoe.Destroy();
            return;
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
        caster.LoginPlayer?.SendServerMessage("Vous n'avez aucune sphère de feu active", ColorConstants.Orange);
    }
  }
}
