using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappesRenforceesEffectTag = "_TRUE_STRIKE_EFFECT";
    public static readonly Native.API.CExoString FrappesRenforceesEffectExoTag = FrappesRenforceesEffectTag.ToExoString();
    public static Effect FrappesRenforcees(NwCreature caster)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.FrappesRenforcees);
      eff.Tag = FrappesRenforceesEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      caster.OnCreatureDamage -= MonkUtils.OnAttackFrappesRenforcees;
      caster.OnCreatureDamage += MonkUtils.OnAttackFrappesRenforcees;

      return eff;
    }
  }
}

