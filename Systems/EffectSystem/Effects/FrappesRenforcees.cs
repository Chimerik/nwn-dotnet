using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappesRenforceesEffectTag = "_FRAPPES_RENFORCEES_EFFECT";
    public static Effect FrappesRenforcees(NwCreature caster)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.FrappesRenforcees);
      eff.Tag = FrappesRenforceesEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      return eff;
    }
  }
}

