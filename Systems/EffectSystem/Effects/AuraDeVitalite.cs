﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AuraDeVitaliteEffectTag = "_AURA_DE_VITALITE_EFFECT";
    public const string AuraDeVitaliteHealEffectTag = "_AURA_DE_VITALITE_HEAL_EFFECT";
    public static Effect AuraDeVitalite(Ability ability, NwSpell spell)
    {
      Effect eff = Effect.VisualEffect(CustomVfx.AuraDeVitalite);
      eff.Tag = AuraDeVitaliteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = (int)ability;
      eff.Spell = spell;
      return eff;
    }
    public static Effect AuraDeVitaliteHeal
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.AuraDeVitalite);
        eff.Tag = AuraDeVitaliteHealEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      } 
    }
  }
}
