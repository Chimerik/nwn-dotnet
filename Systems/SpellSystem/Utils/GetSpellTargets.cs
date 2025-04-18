﻿using System.Collections.Generic;
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
        if (caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{0}").HasValue)
        {
          target = caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{0}").Value;
          caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{0}").Delete();
        }

        targets.Add(target);
      }
      else
      {
        for (int i = 0; i < nbTargets; i++)
        {
          NwGameObject multiTarget = caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Value;

          if (multiTarget is not null && (!distinctTargets || !targets.Contains(multiTarget)))
          {
            float distance = caster.DistanceSquared(multiTarget);

            if (-1 < distance && distance <= spellEntry.range)
              targets.Add(multiTarget);
            else if (caster is NwCreature player)
              player.LoginPlayer?.SendServerMessage($"{multiTarget.Name} n'est plus à portée", ColorConstants.Orange);
          }

          caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{i}").Delete();
        }
      }

      caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();
      caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
      return targets;
    }
  }
}
