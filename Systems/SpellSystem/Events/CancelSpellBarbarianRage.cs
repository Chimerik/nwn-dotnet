using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CancelSpellBarbarianRage(OnSpellAction onSpellAction)
    {
      onSpellAction.PreventSpellCast = true;
      onSpellAction.Caster.LoginPlayer?.SendServerMessage("Impossible de lancer un sort sous l'effet de rage", ColorConstants.Red);
    }
  }
}
