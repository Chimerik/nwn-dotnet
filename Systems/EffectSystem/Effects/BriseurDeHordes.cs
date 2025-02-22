using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BriseurDeHordesEffectTag = "_BRISEUR_DE_HORDES_EFFECT";

    public static void ApplyBriseurDeHordes(NwCreature caster)
    {
      EffectUtils.RemoveTaggedEffect(caster, PourfendeurDeColossesEffectTag, BriseurDeHordesEffectTag);

      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = BriseurDeHordesEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;

      if (!caster.ActiveEffects.Any(e => e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.ChasseurProie))
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, eff));
        caster.OnCreatureAttack -= RangerUtils.OnAttackBriseurDeHordes;
        caster.OnCreatureAttack += RangerUtils.OnAttackBriseurDeHordes;
      }
    }
  }
}

