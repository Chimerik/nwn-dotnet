using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Darkness(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerDarkness), NwTimeSpan.FromRounds(spellEntry.duration));
     
      if (oCaster is NwCreature caster)
      {
        if (caster.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Value == CustomSkill.MonkTenebres)
        {
          caster.IncrementRemainingFeatUses((Feat)CustomSkill.MonkTenebres);
          FeatUtils.DecrementKi(caster, 2);
          caster.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Delete();
        }
      }

      return new List<NwGameObject>() { UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>() };
    }
  }
}
