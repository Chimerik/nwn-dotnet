using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MurDeFeu(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, Location targetLocation, NwFeat feat = null)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is NwCreature caster)
      {
        if (feat is not null && feat.Id == CustomSkill.MonkTorrentDeFlammes)
        {
          caster.IncrementRemainingFeatUses(feat.FeatType);
          FeatUtils.DecrementKi(caster, 5);
          castingClass = NwClass.FromClassId(CustomClass.Monk);
        }

        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");
      }
      //NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.SphereDeFeu(castingClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry)));

      return new List<NwGameObject>() { oCaster };
    }
  }
}
