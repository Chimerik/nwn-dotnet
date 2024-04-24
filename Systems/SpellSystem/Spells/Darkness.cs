using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Darkness(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerDarkness), NwTimeSpan.FromRounds(spellEntry.duration));
      NwAreaOfEffect darkness = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      if(oCaster is NwCreature caster)
        EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { darkness }, spellEntry.duration);
    }
  }
}
