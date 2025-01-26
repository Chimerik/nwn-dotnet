
using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void Charge(NwGameObject oCaster, NwSpell spell, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      
      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.SprintEffectTag);

      oCaster.ClearActionQueue();
      _ = oCaster.AddActionToQueue(() => _ = caster.ActionForceMoveTo(targetLocation, true));

      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.ChargeDuSanglierAura(caster), NwTimeSpan.FromRounds(1));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Charge", ColorConstants.Red, true);

      await NwTask.NextFrame();
      caster.Commandable = false;
    }
  }
}
