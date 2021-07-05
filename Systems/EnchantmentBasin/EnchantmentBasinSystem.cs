using NWN.API.Constants;

namespace NWN.Systems
{
    public partial class EnchantmentBasinSystem
    {
        public static EnchantmentBasin GetEnchantmentBasinFromTag(string tag)
        {
            switch (tag)
            {
                default:
                    return new EnchantmentBasin(
                      minSuccessPercent: 5,
                      maxSuccessPercent: 95,
                      maxAttackBonus: 3,
                      maxACBonus: 3,
                      maxAbilityBonus: 3,
                      maxDamageBonus: IPDamageBonus.Plus1d12,
                      maxSavingThrowBonus: 3,
                      maxRegenBonus: 2
                    );

                case "enchantment_basin_expensive":
                    return new EnchantmentBasin(
                      costRate: 2,
                      minSuccessPercent: 5,
                      maxSuccessPercent: 95,
                      maxCostRateForSuccessRateMin: 500000,
                      maxAttackBonus: 5,
                      maxACBonus: 5,
                      maxAbilityBonus: 6,
                      maxDamageBonus: IPDamageBonus.Plus1d8,
                      maxSavingThrowBonus: 4,
                      maxRegenBonus: 4
                    );
            }
        }
    }
}
