using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDamageEffects(CNWSCreature creature, CNWSCreature target, Anvil.API.Ability attackAbility, bool isUnarmed, bool isCritical, bool isMeleeAttack = true)
    {
      int bonusDamage = 0;
      List<string> noStack = new();

      foreach(var eff in creature.m_appliedEffects)
      {
        string tag = eff.m_sCustomTag.ToString();

        if (noStack.Contains(tag))
          continue;

        bonusDamage += GetPhysicalBonusDamage(eff, isCritical);

        if (isUnarmed || isMeleeAttack)
        {
          switch (tag)
          {
            case EffectSystem.DegatsVaillanteEffectTag: bonusDamage += GetDegatsVaillantsBonus(creature, eff, noStack); break;
            case EffectSystem.EnlargeEffectTag: bonusDamage += GetAgrandissementBonus(noStack); break;
            case EffectSystem.RapetissementEffectTag: bonusDamage -= GetRapetissementMalus(isCritical, noStack); break;
            case EffectSystem.FaveurDuMalinEffectTag: bonusDamage += GetFaveurDuMalinBonus(creature, eff, isCritical, noStack); break;
            case EffectSystem.ChargeurEffectTag: bonusDamage += GetChargeurBonus(creature, eff, noStack, isCritical); break;
            case EffectSystem.BarbarianRageEffectTag: bonusDamage += GetBarbarianRageBonus(creature, eff, noStack, attackAbility, isCritical); break;
            case EffectSystem.RageDuSanglierEffectTag: bonusDamage += GetSanglierRageBonus(creature, eff, noStack, attackAbility, isCritical); break;
            case EffectSystem.FrappeFrenetiqueEffectTag: bonusDamage += GetFrappeFrenetiqueBonus(creature, eff, attackAbility, isCritical, noStack); break;
            case EffectSystem.FrappeBrutaleEffectTag: bonusDamage += GetFrappeBrutaleBonus(creature, attackAbility, noStack); break;
          }
        }
        else if(!isUnarmed && isMeleeAttack)
        {
          switch (tag)
          {
            case EffectSystem.BotteSecreteEffectTag: bonusDamage += GetDegatsBotteSecrete(creature, eff, noStack); break;
          }
        }
        else
        {

        }

        switch (tag)
        {
          case EffectSystem.TranspercerEffectTag: bonusDamage += GetTranspercerBonusDamage(isCritical, noStack); break;
          case EffectSystem.TirPercantEffectTag: bonusDamage += GetTirPercantBonusDamage(isCritical, noStack); break;
          case EffectSystem.MonkParadeEffectTag: bonusDamage -= GetMonkParadeDamageReduction(target, eff, noStack); break;
          case EffectSystem.RayonAffaiblissantDesavantageEffectTag: bonusDamage -= GetRayonAffaiblissantDamageReduction(noStack); break;
        }
      }

      uint effTarget = target.m_idSelf;

      foreach (var eff in target.m_appliedEffects)
      {
        string tag = eff.m_sCustomTag.ToString();
        uint effCreator = eff.m_oidCreator;

        switch (tag)
        {
          case EffectSystem.MarqueDuChasseurTag: bonusDamage += GetHunterMarqueBonusDamage(creature, effCreator, effTarget, noStack); break;
        }
      }

      return bonusDamage;  
    }
  }
}
