using System.Linq;
using Anvil.API;
using NWN.Core;
namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BuveuseDeVieEffectTag = "_BUVEUSE_DE_VIE_EFFECT";

    public static void ApplyBuveuseDeVie(NwCreature caster)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.BuveuseDeVie);
      eff.Tag = BuveuseDeVieEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      EffectUtils.RemoveTaggedEffect(caster, BuveuseDeVieEffectTag);

      if (!caster.ActiveEffects.Any(e => e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.BuveuseDeVie))
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, eff));
        caster.OnCreatureAttack -= OccultisteUtils.OnAttackBuveuseDeVie;
        caster.OnCreatureAttack += OccultisteUtils.OnAttackBuveuseDeVie;
      }
    }
  }
}
