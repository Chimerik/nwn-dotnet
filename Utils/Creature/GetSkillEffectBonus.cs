using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetSkillEffectBonus(NwCreature creature, int skill)
    {
      if (skill != CustomSkill.StealthProficiency) // Simplification parce que le cas n'existe que pour Stealth pour le moment, mais à retirer lorsqu'il y aura d'autres cas
        return 0;

      int bonusScore = 0;
      List<string> effLink = new();

      foreach (var eff in creature.ActiveEffects)
      {
        if(skill == CustomSkill.StealthProficiency)
        {
          if (!eff.LinkId.Contains(eff.LinkId) && eff.EffectType == EffectType.SkillIncrease && eff.IntParams[0] == 8)
          {
            bonusScore += eff.IntParams[1];
            effLink.Add(eff.LinkId);
          }
        }
      }

      return bonusScore;
    }
  }
}
