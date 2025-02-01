using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaleficeTag = "_MALEFICE_EFFECT";
    public static readonly Native.API.CExoString MaleficeExoTag = MaleficeTag.ToExoString();

    private static ScriptCallbackHandle onRemoveMaleficeCallback;
    public static Effect Malefice(NwCreature caster, int spellId)
    {
      caster.OnCreatureAttack -= OnAttackMalefice;
      caster.OnCreatureAttack += OnAttackMalefice;

      var ability = spellId switch
      {
        CustomSpell.MaledictionForce => Ability.Strength,
        CustomSpell.MaledictionDexterite => Ability.Dexterity,
        CustomSpell.MaledictionIntelligence => Ability.Intelligence,
        CustomSpell.MaledictionSagesse => Ability.Wisdom,
        CustomSpell.MaledictionCharisme => Ability.Charisma,
        _ => Ability.Constitution,
      };

      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveMaleficeCallback);
      eff.Tag = MaleficeTag;
      eff.Spell = NwSpell.FromSpellId(spellId);
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = (int)ability;
      return eff;
    }
    private static ScriptHandleResult OnRemoveMalefice(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      ModuleSystem.Log.Info("Malefice ON REMOVED");

      if (eventData.EffectTarget is NwCreature creature)
        creature.OnDeath -= SpellSystem.OnDeathMalefice;

      if (eventData.Effect.Creator is NwCreature caster)
      {
        ModuleSystem.Log.Info($"Maléfice creator : {caster.Name}");
        caster.OnCreatureAttack -= OnAttackMalefice;
      }

      return ScriptHandleResult.Handled;
    }

    public const string FreeMaleficeTag = "_FREE_MALEFICE_EFFECT";

    public static Effect FreeMalefice
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SkillIncrease);
        eff.Tag = FreeMaleficeTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void OnAttackMalefice(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit: 
          
          if(target.ActiveEffects.Any(e => e.Tag == MaleficeTag && e.Creator == onAttack.Attacker))
          {
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant, 
              Effect.Damage(Utils.Roll(6, onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1), CustomDamageType.Necrotic)));
          }
          
          break;
      }
    }
  }
}
