using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PourfendeurDeColossesEffectTag = "_POURFENDEUR_DE_COLOSSES_EFFECT";
    public static readonly Native.API.CExoString PourfendeurDeColossesEffectExoTag = PourfendeurDeColossesEffectTag.ToExoString();
    public static void ApplyPourfendeurDeColosses(NwCreature caster)
    {
      EffectUtils.RemoveTaggedEffect(caster, PourfendeurDeColossesEffectTag, BriseurDeHordesEffectTag);

      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
        eff.Tag = PourfendeurDeColossesEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        eff.Creator = caster;

      if (!caster.ActiveEffects.Any(e => e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.ChasseurProie))
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, eff));
        caster.OnCreatureAttack -= RangerUtils.OnAttackPourfendeurDeColosses;
        caster.OnCreatureAttack += RangerUtils.OnAttackPourfendeurDeColosses;
      }
    }
  }
}

