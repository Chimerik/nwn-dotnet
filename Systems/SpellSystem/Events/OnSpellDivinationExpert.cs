using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastDivinationExpert(NwCreature caster, SpellEvents.OnSpellCast spellCast)
    {
      if (spellCast.Spell.SpellSchool != SpellSchool.Divination || !caster.KnowsFeat((Feat)CustomSkill.DivinationExpert)
        || spellCast.SpellLevel < 3 || spellCast.SpellCastClass is null)
        return;

      var castingClass = caster.GetClassInfo(spellCast.SpellCastClass);
      byte spellLevel = spellCast.SpellLevel > 6 ? (byte)6 : (byte)(spellCast.SpellLevel - 1);

      for (byte i = spellLevel; i > 0; i--)
      {
        var remainingSlots = castingClass.GetRemainingSpellSlots(i);

        if (remainingSlots < castingClass.Class.SpellGainTable[castingClass.Level - 1][i])
        {
          castingClass.SetRemainingSpellSlots(i, remainingSlots++);
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
          return;
        }
      }
    }
  }
}
