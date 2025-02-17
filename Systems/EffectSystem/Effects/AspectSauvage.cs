using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AspectSauvageEffectTag = "_ASPECT_SAUVAGE_EFFECT";
    public static Effect AspectSauvageChouette(int spellId)
    {
      Effect eff = spellId switch
      {
        CustomSpell.AspectSauvageChouette => Effect.Ultravision(),
        CustomSpell.AspectSauvageSaumon => Nage,
        _ => Effect.RunAction(),
      };

      eff.Tag = AspectSauvageEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}

