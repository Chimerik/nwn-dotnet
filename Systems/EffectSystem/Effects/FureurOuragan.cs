using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurDelOuraganEffectTag = "_FUREUR_OURAGAN_EFFECT";
    public static Effect FureurDeLOuragan(int spellId)
    {
      var eff = Effect.RunAction();
      eff.Tag = FureurDelOuraganEffectTag;
      eff.IntParams[5] = spellId == CustomSpell.FureurDelOuraganFoudre ? (int)DamageType.Electrical : (int)DamageType.Sonic;
      return eff;
    }
  }
}
