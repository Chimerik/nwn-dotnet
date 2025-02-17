using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AssistanceEffectTag = "_ASSISTANCE_EFFECT";
    public static Effect Assistance
    {
      get
      {
        Effect eff = Effect.SkillIncreaseAll(NwRandom.Roll(Utils.random, 4));
        eff.Tag = AssistanceEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Assistance);
        return eff;
      }
    }
  }
}

