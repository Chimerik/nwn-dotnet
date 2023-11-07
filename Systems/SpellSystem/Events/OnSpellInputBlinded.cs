using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public void HandleSpellInputBlinded(OnSpellAction onSpellAction)
    {
      if (onSpellAction.TargetObject is not NwGameObject target
        || (!onSpellAction.Caster.ActiveEffects.Any(e => e.EffectType == EffectType.Blindness || e.EffectType == EffectType.Darkness)
        && !target.ActiveEffects.Any(e => e.EffectType == EffectType.Darkness))
        || target.DistanceSquared(onSpellAction.Caster) < 9)
        return;

      onSpellAction.PreventSpellCast = true;
      onSpellAction.Caster.LoginPlayer?.SendServerMessage($"Vous devez vous situer à moins de {"3m".ColorString(ColorConstants.White)} de cette cible pour l'atteindre", ColorConstants.Red);
    }
  }
}
