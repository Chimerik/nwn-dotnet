using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ShillelaghEffectTag = "_SHILLELAGH_EFFECT";
    public static void ApplyShillelagh(NwCreature caster, NwSpell spell, Ability casterAbility)
    {
      EffectUtils.RemoveTaggedEffect(caster, ShillelaghEffectTag);

      Effect attack = Effect.Icon(CustomEffectIcon.Shillelagh);
      attack.Tag = ShillelaghEffectTag;
      attack.SubType = EffectSubType.Supernatural;
      attack.CasterLevel = (int)casterAbility;
      attack.Spell = spell;
      caster.ApplyEffect(EffectDuration.Permanent, attack);
    }
  }
}

