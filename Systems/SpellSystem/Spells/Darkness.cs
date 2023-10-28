using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Darkness(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      
      onSpellCast.TargetLocation.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerDarkness), NwTimeSpan.FromRounds(spellEntry.duration));
      NwAreaOfEffect darkness = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      EffectSystem.ApplyConcentrationEffect(oCaster, onSpellCast.Spell.Id, new List<NwGameObject> { darkness }, spellEntry.duration);
    }
  }
}
