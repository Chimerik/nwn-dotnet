using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PacteDeLaLameDispelEffectTag = "_PACTE_DE_LA_LAME_DISPEL_EFFECT";
    private static ScriptCallbackHandle onRemovePacteDeLaLameDispelCallback;
    private static ScriptCallbackHandle onIntervalPacteDeLaLameDispelCallback;
    
    public static Effect PacteDeLaLameDispel
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)191), Effect.RunAction(onIntervalHandle: onIntervalPacteDeLaLameDispelCallback, interval:TimeSpan.FromMinutes(1)));
        eff.Tag = PacteDeLaLameDispelEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalPacteDeLaLameDispel(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        if (caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).HasValue)
          caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).Value?.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Delete();

        caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).Delete();

        if (caster.KnowsFeat((Feat)CustomSkill.LameAssoiffee))
        {
          CreatureUtils.InitializeNumAttackPerRound(caster);

          caster.OnItemUnequip -= OccultisteUtils.OnUnequipPacteDeLaLame;
          caster.OnItemEquip -= OccultisteUtils.OnEquipPacteDeLaLame;
        }

        EffectUtils.RemoveTaggedEffect(caster, PacteDeLaLameDispelEffectTag);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
