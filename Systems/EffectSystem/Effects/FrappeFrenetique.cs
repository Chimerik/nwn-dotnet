using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeFrenetiqueEffectTag = "_FRAPPE_FRENETIQUE_EFFECT";
    //public static readonly Native.API.CExoString FrappeFrenetiqueEffectExoTag = FrappeFrenetiqueEffectTag.ToExoString();
    public static Effect FrappeFrenetique(NwCreature caster)
    {
      EffectUtils.RemoveTaggedEffect(caster, FrappeFrenetiqueEffectTag);

      int level = caster.GetClassInfo((ClassType)CustomClass.Barbarian).Level;
      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = FrappeFrenetiqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = level > 15 ? 4 : level > 8 ? 3 : 2;
      return eff;
    }
  }
}

