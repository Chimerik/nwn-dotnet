using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Darkness(NwGameObject oCaster, NwSpell spell, NwFeat feat, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerDarkness), SpellUtils.GetSpellDuration(oCaster, spellEntry));
     
      if (oCaster is NwCreature caster && feat is not null && feat.Id == CustomSkill.MonkTenebres)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster, 2);
      }

      return new List<NwGameObject>() { UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>() };
    }
  }
}
