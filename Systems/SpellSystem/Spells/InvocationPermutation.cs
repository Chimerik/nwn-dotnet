using System.Numerics;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void InvocationPermutation(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oTarget is NwCreature target)
      {
        if (caster.Faction.GetMembers().Contains(target))
        {
          var casterPos = CreaturePlugin.ComputeSafeLocation(caster, target.Position, 2);
          var targetPos = CreaturePlugin.ComputeSafeLocation(target, caster.Position, 2);

          if (casterPos != Vector3.Zero && targetPos != Vector3.Zero)
          {
            caster.ClearActionQueue();
            caster.Position = casterPos;
            target.ClearActionQueue();
            target.Position = targetPos;

            caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

            caster.SetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation, 0);
          }
          else
            caster.ControllingPlayer?.SendServerMessage("Permutation impossible", ColorConstants.Red);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
      }
      else if(oTarget is null)
      {
        var casterPos = CreaturePlugin.ComputeSafeLocation(caster, targetLocation.Position, 2);

        if (casterPos != Vector3.Zero)
        {
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
          caster.ClearActionQueue();
          caster.Position = casterPos;
          targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));

          caster.SetFeatRemainingUses((Feat)CustomSkill.InvocationPermutation, 0);
        }
        else
          caster.ControllingPlayer?.SendServerMessage("Permutation impossible", ColorConstants.Red);
      }
    }
  }
}
