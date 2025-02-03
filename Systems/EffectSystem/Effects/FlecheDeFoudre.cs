using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FlecheDeFoudreEffectTag = "_FLECHE_DE_FOUDRE_EFFECT";
    public static Effect FlecheDeFoudre
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.FlecheDeFoudre);
        eff.Tag = FlecheDeFoudreEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.FlecheDeFoudre);
        return eff;
      }
    }
  }
}
