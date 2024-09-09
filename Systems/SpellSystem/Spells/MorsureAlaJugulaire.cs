
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void MorsureAlaJugulaire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Strength);

      var targetLoc = Location.Create(oTarget.Area, CreaturePlugin.ComputeSafeLocation(caster, target.Position, 3, 1), oCaster.Rotation);

      oCaster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDustExplosion));
      await oCaster.ClearActionQueue();
      await oCaster.ActionJumpToLocation(targetLoc);
      await NwTask.NextFrame();
      await caster.ActionAttackTarget(target);
      targetLoc.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDustExplosion));

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
        EffectSystem.ApplyKnockdown(target, CreatureSize.Large, 1);
    }
  }
}
