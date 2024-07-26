using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Bourrasque(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        oCaster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{spell.Id}").Value = (int)castingClass.SpellCastingAbility;
        
        oCaster.Location.ApplyEffect(EffectDuration.Temporary, EffectSystem.Bourrasque(caster), NwTimeSpan.FromRounds(spellEntry.duration));  
        concentrationList.Add(UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>());

        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosNormal20));
      }

      return concentrationList;
    }
  }
}
