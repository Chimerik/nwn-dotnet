using System;
using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void EspritEveille(NwCreature caster, NwGameObject oTarget)
    {
      /*foreach (ShaderUniform shader in (ShaderUniform[])Enum.GetValues(typeof(ShaderUniform)))
      {
        caster.ControllingPlayer.SetShaderUniform(shader, new Vector4(9, 0, 0, 0));
        await NwTask.Delay(TimeSpan.FromSeconds(2));
      }*/
        
      caster.LoginPlayer?.EnterTargetMode(SelectEspritEveilleTarget, Config.CreatureTargetMode(0, new Vector2() { X = 9, Y = 9 }, SpellTargetingShape.Sphere, SpellTargetingFlags.HarmsAllies));
    }
    private static void SelectEspritEveilleTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject == selection.Player.ControlledCreature || selection.TargetObject is not NwCreature target)
        return;
      
      NwCreature caster = selection.Player.ControlledCreature;

      var eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.EspritEveilleEffectTag && e.Creator != caster);

      if (eff is not null)
      {
        if (eff.Creator is NwCreature previousTarget)
          EffectUtils.RemoveTaggedEffect(previousTarget, caster, EffectSystem.EspritEveilleEffectTag, EffectSystem.EspritEveilleDisadvantageEffectTag);

        caster.RemoveEffect(eff);
      }

      TimeSpan duration =
        TimeSpan.FromMinutes(caster.GetClassInfo((ClassType)CustomClass.Occultiste).Level);
      NWScript.AssignCommand(target, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritEveille, duration));
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritEveille, duration));

      if (caster.KnowsFeat((Feat)CustomSkill.CombattantClairvoyant))
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Charisma);
        if (CreatureUtils.GetSavingThrowResult(target, Ability.Wisdom, caster, spellDC) == SavingThrowResult.Failure)
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritEveilleDisadvantage, duration));
      }
    }
  }
}
