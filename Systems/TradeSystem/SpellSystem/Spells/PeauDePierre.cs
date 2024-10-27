using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> PeauDePierre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwFeat feat = null)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      if (oCaster is NwCreature caster)
      {
        foreach (var target in targets)
        {
          if (feat is not null && feat.Id == CustomSkill.MonkDefenseDeLaMontagne)
          {
            caster.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(caster, 5);
          }
          else
          {
            if (caster.Gold < 100)
            {
              caster.LoginPlayer?.SendServerMessage("Ce sort nécessite des composants d'une valeur de 100 similis", ColorConstants.Red);
              return targets;
            }
            else
              caster.Gold -= 100;
          }

          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.PeauDePierre, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
        }
      }
    
      return targets;
    }
  }
}
