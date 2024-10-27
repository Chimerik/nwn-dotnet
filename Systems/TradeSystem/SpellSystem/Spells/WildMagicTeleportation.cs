using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void WildMagicTeleportation(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;
   
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      _ = caster.ClearActionQueue();
      _ = caster.ActionJumpToLocation(targetLocation);
      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
    }
  }
}
