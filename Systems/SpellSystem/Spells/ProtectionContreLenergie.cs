using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ProtectionContreLenergie(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      
      var eff = spellEntry.damageType[0] switch
      {
        DamageType.Acid => Effect.LinkEffects(EffectSystem.ResistanceAcide, Effect.VisualEffect(CustomVfx.ProtectionAcide), Effect.RunAction()),
        DamageType.Cold => Effect.LinkEffects(EffectSystem.ResistanceFroid, Effect.VisualEffect(CustomVfx.ProtectionFroid), Effect.RunAction()),
        DamageType.Electrical => Effect.LinkEffects(EffectSystem.ResistanceElec, Effect.VisualEffect(CustomVfx.ProtectionElec), Effect.RunAction()),
        DamageType.Sonic => Effect.LinkEffects(EffectSystem.ResistanceTonnerre, Effect.VisualEffect(CustomVfx.ProtectionTonnerre), Effect.RunAction()),
        _ => Effect.LinkEffects(EffectSystem.ResistanceFeu, Effect.VisualEffect(CustomVfx.ProtectionFeu), Effect.RunAction()),
      };

      foreach (var target in targets)
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, eff, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      return targets;
    }
  }
}
