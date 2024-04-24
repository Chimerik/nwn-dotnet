using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastDivinationExpert(NwCreature caster, NwSpell spell, NwClass castingClass)
    {
      if (spell.SpellSchool != SpellSchool.Divination || !caster.KnowsFeat((Feat)CustomSkill.DivinationExpert)
         || castingClass is null  || spell.GetSpellLevelForClass(castingClass) < 3)
        return;

      var casterClass = caster.GetClassInfo(castingClass);
      byte spellLevel = spell.GetSpellLevelForClass(castingClass) > 6 ? (byte)6 : (byte)(spell.GetSpellLevelForClass(castingClass) - 1);

      for (byte i = spellLevel; i > 0; i--)
      {
        var remainingSlots = casterClass.GetRemainingSpellSlots(i);

        if (remainingSlots < casterClass.Class.SpellGainTable[casterClass.Level - 1][i])
        {
          casterClass.SetRemainingSpellSlots(i, remainingSlots++);
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
          return;
        }
      }
    }
  }
}
