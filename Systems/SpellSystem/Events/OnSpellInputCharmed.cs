using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellInputCharmed(OnSpellAction onCast)
    {
      if(onCast.Caster.ActiveEffects.Any(e => e.Tag == EffectSystem.charmEffectTag && onCast.TargetObject == e.Creator))
      {
        onCast.PreventSpellCast = true;
        onCast.Caster.LoginPlayer?.SendServerMessage($"Vous êtes sous le charme de cette création et ne pouvez pas la cibler", ColorConstants.Red);
      }
    }
  }
}
