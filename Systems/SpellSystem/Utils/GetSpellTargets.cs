using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static List<NwGameObject> GetSpellTargets(NwGameObject caster, NwGameObject target, SpellEntry spellEntry, bool distinctTargets = false)
    {
      List<NwGameObject> targets = new();

      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;

      if (nbTargets < 1)
      {
        targets.Add(target);
        return targets;
      }

      for (int i = 0; i < nbTargets; i++)
      {
        NwGameObject multiTarget = caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Value;

        if (!distinctTargets || !targets.Contains(multiTarget))
        {
          float distance = target.DistanceSquared(caster);

          if (-1 < distance && distance <= spellEntry.range)
            targets.Add(multiTarget);
          else if(caster is NwCreature player)
            player.LoginPlayer?.SendServerMessage($"{target.Name} n'est plus à portée", ColorConstants.Orange);
        }

        caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Delete();
      }

      caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      return targets;
    }
  }
}
