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

        bonusDamage += GetPhysicalBonusDamage(eff, isCritical);

        if (isUnarmed || isMeleeAttack)
        {
          switch (tag)
          {
            case EffectSystem.DegatsVaillanteEffectTag: bonusDamage += GetDegatsVaillantsBonus(creature, eff); break;
            case EffectSystem.EnlargeEffectTag: bonusDamage += GetAgrandissementBonus(isCritical); break;
            case EffectSystem.RapetissementEffectTag: bonusDamage -= GetRapetissementMalus(); break;
            case EffectSystem.FaveurDuMalinEffectTag: bonusDamage += GetFaveurDuMalinBonus(creature, eff, isCritical); break;
            case EffectSystem.ChargeurEffectTag: bonusDamage += GetChargeurBonus(creature, eff, noStack, isCritical); break;
            case EffectSystem.BarbarianRageEffectTag: bonusDamage += GetBarbarianRageBonus(creature, eff, noStack, attackAbility); break;
            case EffectSystem.RageDuSanglierEffectTag: bonusDamage += GetSanglierRageBonus(creature, eff, noStack, attackAbility); break;
            case EffectSystem.FrappeFrenetiqueEffectTag: bonusDamage += GetFrappeFrenetiqueBonus(creature, eff, attackAbility, isCritical); break;
            case EffectSystem.FrappeBrutaleEffectTag: bonusDamage += GetFrappeBrutaleBonus(creature, attackAbility, isCritical); break;
          }
        }
        else if(!isUnarmed && isMeleeAttack)
        {
          switch (tag)
          {
            case EffectSystem.BotteSecreteEffectTag: bonusDamage += GetDegatsBotteSecrete(creature, eff, isCritical); break;
          }
        }
        else
        {

        }

        switch (tag)
        {
          case EffectSystem.TranspercerEffectTag: bonusDamage += GetTranspercerBonusDamage(); break;
          case EffectSystem.TirPercantEffectTag: bonusDamage += GetTirPercantBonusDamage(); break;
        }
      }   

      return bonusDamage;  
    }
  }
}
