using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Metamagie(NwCreature caster, int featId)
    {
      switch(featId)
      {
        case CustomSkill.EnsoAmplification:
        case CustomSkill.EnsoGuidage: EffectUtils.RemoveTaggedParamEffect(caster, featId, EffectSystem.MetamagieEffectTag); break;
        default:
          
          if(caster.KnowsFeat((Feat)CustomSkill.SorcellerieIncarnee) 
            && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.SorcellerieInneeEffectTag))
          {
            if(caster.ActiveEffects.Count(e => e.Tag == EffectSystem.MetamagieEffectTag && !Utils.In(e.IntParams[5], CustomSkill.EnsoAmplification, CustomSkill.EnsoGuidage)) > 2)
              RemoveFirstNonCumulativeMetaMagic(caster);
          }         
          else
            RemoveNonCumulativeMetaMagic(caster); 
          
          break;
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {SkillSystem.learnableDictionary[featId].name}", StringUtils.gold, true, true);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.MetaMagie(featId), NwTimeSpan.FromRounds(100));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));

      if (caster.KnowsFeat((Feat)CustomSkill.EnsoApotheose) && caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.EnsoApotheoseVariable).HasNothing)
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.EnsoApotheoseVariable).Value = 1;
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
        caster.LoginPlayer?.SendServerMessage("Apothéose", StringUtils.gold);
        return;
      }

      byte sourceCost = featId switch
      {
        CustomSkill.EnsoGemellite => 0,
        CustomSkill.EnsoAmplification => 2,
        CustomSkill.EnsoIntensification or CustomSkill.EnsoAcceleration => 3,
        _ => 1,
      };

      if(sourceCost > EnsoUtils.GetSorcerySource(caster))
      {
        caster.LoginPlayer?.SendServerMessage($"Cette métamagie nécessite {sourceCost}", ColorConstants.Red);
        caster.SetFeatRemainingUses((Feat)featId, 0);
        return;
      }

      
      if(featId ==  CustomSkill.EnsoTransmutation && PlayerSystem.Players.TryGetValue(caster, out var player))
      {
        if (player.windows.TryGetValue("ensoMetaTransmutationSelection", out var transmu)) ((EnsoMetaTransmutationSelectionWindow)transmu).CreateWindow();
        else player.windows.Add("ensoMetaTransmutationSelection", new EnsoMetaTransmutationSelectionWindow(player));

      }

      EnsoUtils.DecrementSorcerySource(caster, sourceCost);
    }

    public static void RemoveNonCumulativeMetaMagic(NwGameObject target)
    {
      foreach (var eff in target.ActiveEffects)
        if (eff.Tag == EffectSystem.MetamagieEffectTag && !Utils.In(eff.IntParams[5], CustomSkill.EnsoAmplification, CustomSkill.EnsoGuidage))
          target.RemoveEffect(eff);
    }
    public static void RemoveFirstNonCumulativeMetaMagic(NwGameObject target)
    {
      var eff = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.MetamagieEffectTag && !Utils.In(e.IntParams[5], CustomSkill.EnsoAmplification, CustomSkill.EnsoGuidage));
      target.RemoveEffect(eff);
    }
  }
}
