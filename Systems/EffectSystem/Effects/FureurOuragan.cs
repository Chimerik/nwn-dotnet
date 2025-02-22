using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurDelOuraganEffectTag = "_FUREUR_OURAGAN_EFFECT";
    public static Effect FureurDeLOuragan(NwSpell spell)
    {
      var eff = Effect.RunAction();
      eff.Tag = FureurDelOuraganEffectTag;
      eff.Spell = spell;
      return eff;
    }
  }
}
