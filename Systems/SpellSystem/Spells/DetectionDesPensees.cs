using System.Collections.Generic;
using Anvil.API;
using NLog.Targets;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> DetectionDesPensees(NwGameObject oCaster, NwSpell spell, NwFeat feat, SpellEntry spellEntry)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is not NwCreature caster)
        return concentrationTargets;

      SpellUtils.SignalEventSpellCast(caster, caster, spell.SpellType);

      if(feat is not null && feat.Id == CustomSkill.ClercDetectionDesPensees)
        ClercUtils.ConsumeConduitDivin(caster);

      return concentrationTargets;
    }
  }
}
