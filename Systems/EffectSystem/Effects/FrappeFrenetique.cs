using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeFrenetiqueEffectTag = "_FRAPPE_FRENETIQUE_EFFECT";
    public static Effect FrappeFrenetique(NwCreature caster)
    {
      EffectUtils.RemoveTaggedEffect(caster, FrappeFrenetiqueEffectTag);

      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = FrappeFrenetiqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}

