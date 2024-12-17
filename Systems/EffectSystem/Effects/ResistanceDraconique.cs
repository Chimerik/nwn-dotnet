using System.Security.Cryptography;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceDraconiqueEffectTag = "_RESISTANCE_DRACONIQUE_EFFECT";
    public static void ApplyResistanceDraconiqueEffect(NwCreature creature, int charismaModifier)
    {
      Effect acInc = Effect.ACIncrease(charismaModifier, ACBonus.ArmourEnchantment);
      acInc.ShowIcon = false;

      Effect eff = Effect.LinkEffects(acInc, Effect.Icon(CustomEffectIcon.ResistanceDraconique));
      eff.Tag = ResistanceDraconiqueEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      if(creature.IsLoginPlayerCharacter)
        NwFeat.FromFeatId(CustomSkill.EnsoResistanceDraconique).Name.SetPlayerOverride(creature.LoginPlayer, $"Résistance Draconique (+{charismaModifier}");

      creature.ApplyEffect(EffectDuration.Permanent, eff);
    }
  }
}
