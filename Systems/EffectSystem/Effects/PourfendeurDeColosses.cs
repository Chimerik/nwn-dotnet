using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PourfendeurDeColossesEffectTag = "_POURFENDEUR_DE_COLOSSES_EFFECT";
    public static void ApplyPourfendeurDeColosses(NwCreature caster, NwSpell spell)
    {
      EffectUtils.RemoveTaggedEffect(caster, PourfendeurDeColossesEffectTag, BriseurDeHordesEffectTag);

      Effect eff = Effect.Icon(CustomEffectIcon.PourfendeurDeColosses);
      eff.Tag = PourfendeurDeColossesEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;
      eff.Spell = spell;

      if (!caster.ActiveEffects.Any(e => e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.ChasseurProie))
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, eff));
      }
    }
  }
}

