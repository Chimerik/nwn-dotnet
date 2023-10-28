using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetAbilityAdvantageFromEffect(Ability ability, string effectTag)
    {
      return ability switch
      {
        Ability.Strength => GetStrengthAdvantageFromEffectTag(effectTag),
        Ability.Constitution => GetConstitutionAdvantageFromEffectTag(effectTag),
        Ability.Intelligence => GetIntelligenceAdvantageFromEffectTag(effectTag),
        Ability.Wisdom => GetWisdomAdvantageFromEffectTag(effectTag),
        Ability.Charisma => GetCharismaAdvantageFromEffectTag(effectTag),
        _ => GetDexterityAdvantageFromEffectTag(effectTag),
      };
    }
    public static int GetDexterityAdvantageFromEffectTag(string effectTag)
    {
      return effectTag switch
      {
        EffectSystem.DexterityAdvantageEffectTag => 1,
        EffectSystem.DexterityDisadvantageEffectTag or EffectSystem.ShieldArmorDisadvantageEffectTag => -1,
        _ => 0,
      };
    }
    public static int GetStrengthAdvantageFromEffectTag(string effectTag)
    {
      return effectTag switch
      {
        EffectSystem.StrengthAdvantageEffectTag => 1,
        EffectSystem.StrengthDisadvantageEffectTag or EffectSystem.ShieldArmorDisadvantageEffectTag => -1,
        _ => 0,
      };
    }
    public static int GetConstitutionAdvantageFromEffectTag(string effectTag)
    {
      return effectTag switch
      {
        EffectSystem.ConstitutionAdvantageEffectTag => 1,
        EffectSystem.ConstitutionDisadvantageEffectTag => -1,
        _ => 0,
      };
    }
    public static int GetIntelligenceAdvantageFromEffectTag(string effectTag)
    {
      return effectTag switch
      {
        EffectSystem.IntelligenceAdvantageEffectTag => 1,
        EffectSystem.IntelligenceDisadvantageEffectTag => -1,
        _ => 0,
      };
    }
    public static int GetWisdomAdvantageFromEffectTag(string effectTag)
    {
      return effectTag switch
      {
        EffectSystem.WisdomAdvantageEffectTag => 1,
        EffectSystem.WisdomDisadvantageEffectTag => -1,
        _ => 0,
      };
    }
    public static int GetCharismaAdvantageFromEffectTag(string effectTag)
    {
      return effectTag switch
      {
        EffectSystem.CharismaAdvantageEffectTag => 1,
        EffectSystem.CharismaDisadvantageEffectTag => -1,
        _ => 0,
      };
    }
  }
}
