using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappesRenforcees(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.FrappesRenforceesEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.FrappesRenforceesEffectTag);
        caster.OnCreatureDamage -= MonkUtils.OnAttackFrappesRenforcees;
        caster.LoginPlayer?.SendServerMessage($"{"Frappes Renforcées".ColorString(ColorConstants.White)} désactivé", ColorConstants.Orange);
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FrappesRenforcees(caster));
        caster.LoginPlayer?.SendServerMessage($"{"Frappes Renforcées".ColorString(ColorConstants.White)} activé", ColorConstants.Orange);
      }
    }
  }
}
