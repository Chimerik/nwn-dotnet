using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Ralliement(NwCreature caster, NwGameObject targetObject)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      if (targetObject is not NwCreature target || target == caster || caster.IsReactionTypeHostile(target))
      {
        caster.ControllingPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      int warMasterLevel = caster.GetClassInfo(ClassType.Fighter).Level;
      int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;
      int temporaryHP = NwRandom.Roll(Utils.random, superiorityDice) + caster.GetAbilityModifier(Ability.Charisma);
      
      List<int> highestHPList = new() { 0 };

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.TemporaryHitpoints)
          highestHPList.Add(eff.IntParams[5]);

      if (temporaryHP > highestHPList.Max())
      {
        target.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(temporaryHP));
        LogUtils.LogMessage($"{caster.Name} ralliement sur {target.Name} : 1d{superiorityDice} = {temporaryHP} PV temporaires", LogUtils.LogType.Combat);
      }
      else
      {
        caster.ControllingPlayer?.SendServerMessage("Attention : les points de vie temporaires ne sont pas cumulatifs !", ColorConstants.Red);
        LogUtils.LogMessage($"{caster.Name} ralliement sur {target.Name} mais la cible bénéficie déjà d'une source de PV temporaires supérieure", LogUtils.LogType.Combat);
      }
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"Ralliement ({target.Name})", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
