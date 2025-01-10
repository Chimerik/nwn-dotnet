using Anvil.API;
using NWN.Native.API;
using DamageType = Anvil.API.DamageType;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetPhysicalBonusDamage(CGameEffect eff, bool isCritical)
    {
      int bonusDamage = 0;

      /*for (var index = 0; index < attackWeapon.m_lstPassiveProperties.Count; index++)
      {
        var ip = attackWeapon.GetPassiveProperty(index);

        if (ip.m_nPropertyName == (int)ItemPropertyType.DamageBonus)
        {
          switch ((IPDamageType)ip.m_nSubType)
          {
            case IPDamageType.Bludgeoning:
            case IPDamageType.Piercing:
            case IPDamageType.Slashing:
              
              var damageEntry = ItemPropertyDamageCost2da.ipDamageCostTable[ip.m_nCostTableValue];

              if (damageEntry.numDice == 0)
              {
                int damage = damageEntry.die;
                bonusDamage += damage;
                LogUtils.LogMessage($"Dégâts bonus propriétés de l'arme : +{damage}", LogUtils.LogType.Combat);
              }
              else
              {
                int random = NwRandom.Roll(Utils.random, damageEntry.die, damageEntry.numDice);
                bonusDamage += random;
                LogUtils.LogMessage($"Dégâts bonus propriétés de l'arme ({damageEntry.numDice}d{damageEntry.die}) : +{random}", LogUtils.LogType.Combat);
              }

              break;
          }
        }
      }*/

        switch((EffectTrueType)eff.m_nType)
        {
          case EffectTrueType.DamageIncrease:

            switch((DamageType)eff.GetInteger(1))
            {
              case DamageType.Bludgeoning:
              case DamageType.Piercing:
              case DamageType.Slashing:

                switch((DamageBonus)eff.GetInteger(0))
                {
                  case DamageBonus.Plus1:
                  case DamageBonus.Plus2:
                  case DamageBonus.Plus3:
                  case DamageBonus.Plus4:
                  case DamageBonus.Plus5:

                    int damage = eff.GetInteger(0);
                    bonusDamage += damage;
                    LogUtils.LogMessage($"Effet dégâts bonus : +{damage}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus6:
                  case DamageBonus.Plus7:
                  case DamageBonus.Plus8:
                  case DamageBonus.Plus9:
                  case DamageBonus.Plus10:
                  case DamageBonus.Plus11:
                  case DamageBonus.Plus12:
                  case DamageBonus.Plus13:
                  case DamageBonus.Plus14:
                  case DamageBonus.Plus15:
                  case DamageBonus.Plus16:
                  case DamageBonus.Plus17:
                  case DamageBonus.Plus18:
                  case DamageBonus.Plus19:
                  case DamageBonus.Plus20:

                    int bonus = eff.GetInteger(0) - 10;
                    bonusDamage += bonus;
                    LogUtils.LogMessage($"Effet dêgats bonus : +{bonus}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus1d4:

                    int d4 = Utils.Roll(4, isCritical ? 2 : 1);
                    bonusDamage += d4;
                    LogUtils.LogMessage($"Effet dêgats bonus (1d4) : +{d4}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus1d6:

                    int d6 = Utils.Roll(6, isCritical ? 2 : 1);
                  bonusDamage += d6;
                    LogUtils.LogMessage($"Effet dêgats bonus (1d6) : +{d6}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus1d8:

                    int d8 = Utils.Roll(8, isCritical ? 2 : 1);
                    bonusDamage += d8;
                    LogUtils.LogMessage($"Effet dêgats bonus (1d8) : +{d8}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus1d10:

                    int d10 = Utils.Roll(10, isCritical ? 2 : 1);
                    bonusDamage += d10;
                    LogUtils.LogMessage($"Effet dêgats bonus (1d10) : +{d10}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus1d12:

                    int d12 = Utils.Roll(12, isCritical ? 2 : 1);
                    bonusDamage += d12;
                    LogUtils.LogMessage($"Effet dêgats bonus (1d12) : +{d12}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus2d4:

                    int d24 = Utils.Roll(4, isCritical ? 4 : 2);
                  bonusDamage += d24;
                    LogUtils.LogMessage($"Effet dêgats bonus (2d4) : +{d24}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus2d6:

                    int d26 = Utils.Roll(6, isCritical ? 4 : 2);
                  bonusDamage += d26;
                    LogUtils.LogMessage($"Effet dêgats bonus (2d6) : +{d26}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus2d8:

                    int d28 = Utils.Roll(8, isCritical ? 4 : 2);
                    bonusDamage += d28;
                    LogUtils.LogMessage($"Effet dêgats bonus (2d4) : +{d28}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus2d10:

                    int d210 = Utils.Roll(10, isCritical ? 4 : 2);
                    bonusDamage += d210;
                    LogUtils.LogMessage($"Effet dêgats bonus (2d10) : +{d210}", LogUtils.LogType.Combat);

                    break;

                  case DamageBonus.Plus2d12:

                    int d212 = Utils.Roll(12, isCritical ? 4 : 2);
                    bonusDamage += d212;
                    LogUtils.LogMessage($"Effet dêgats bonus (2d12) : +{d212}", LogUtils.LogType.Combat);

                    break;
                }

                break;
            }

            break;
        }

      return bonusDamage;
    }
  }
}
