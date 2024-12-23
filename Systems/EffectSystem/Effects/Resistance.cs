using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceEffectTag = "_RESISTANCE_EFFECT";
    public static Effect Resistance(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.Icon(EffectIcon.DamageResistance);   
      eff.Tag = ResistanceEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = CustomSpell.Resistance;
      eff.IntParams[6] = spell.Id;

      return eff;
    }
  }
}
