using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AttaquesEtudieesEffectTag = "_ATTAQUES_ETUDIEES_EFFECT";
    public static readonly Native.API.CExoString AttaquesEtudieesEffectExoTag = AttaquesEtudieesEffectTag.ToExoString();
    public static Effect AttaquesEtudiees
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackIncrease);
        eff.Tag = AttaquesEtudieesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

