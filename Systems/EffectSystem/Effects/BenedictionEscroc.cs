using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BenedictionEscrocEffectTag = "_BENEDICTION_ESCROC_EFFECT";
    public static Effect BenedictionEscroc
    {
      get
      {
        Effect eff = Effect.SkillIncrease(Skill.Hide, 1);
        eff.Tag = BenedictionEscrocEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
