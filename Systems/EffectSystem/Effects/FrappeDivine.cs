using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineEffectTag = "_FRAPPE_DIVINE_EFFECT";
    public static readonly Native.API.CExoString FrappeDivineEffectExoTag = FrappeDivineEffectTag.ToExoString();
    public static void ApplyFrappeDivine(NwCreature caster)
    {
      Effect eff = Effect.Icon((EffectIcon)223);
      eff.Tag = FrappeDivineEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      EffectUtils.RemoveTaggedEffect(caster, FrappeDivineEffectTag);

      if (!caster.ActiveEffects.Any(e => e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.ClercFrappeDivine))
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, eff));
        caster.OnCreatureAttack -= ClercUtils.OnAttackFrappeDivine;
        caster.OnCreatureAttack += ClercUtils.OnAttackFrappeDivine;
      }
    }
  }
}
