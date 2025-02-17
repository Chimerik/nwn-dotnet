using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DissipationDeLaMagie(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDispel));
      int spellMod = caster.GetAbilityModifier(castingClass.SpellCastingAbility);

      foreach(var eff in oTarget.ActiveEffects)
      {
        if (eff.Spell is not null)
        {
          if(eff.Spell.GetSpellLevelForClass(castingClass) < 4)
            oTarget.RemoveEffect(eff);
          else if(Utils.Roll(20) + spellMod > 10 + eff.Spell.InnateSpellLevel)
              oTarget.RemoveEffect(eff);
        }
          
      }
    }
  }
}
