using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public void OnSpellInputDodging(OnSpellAction onSpellAction)
    {
      foreach(var eff in onSpellAction.Caster.ActiveEffects)
        if(eff.Tag == EffectSystem.DodgeEffectTag)
          onSpellAction.Caster.RemoveEffect(eff);
    }
  }
}
