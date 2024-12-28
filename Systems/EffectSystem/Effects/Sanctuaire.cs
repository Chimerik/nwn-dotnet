using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SanctuaireEffectTag = "_SANCTUAIRE_EFFECT";
    private static ScriptCallbackHandle onRemoveSanctuaireCallback;
    public static Effect Sanctuaire(NwCreature target, int spellDC)
    {
      target.OnCreatureAttack -= OnAttackSanctuaire;
      target.OnCreatureAttack += OnAttackSanctuaire;
      target.OnSpellAction -= OnSpellActionSanctuaire;
      target.OnSpellAction += OnSpellActionSanctuaire;
      target.OnCreatureDamage -= OnDamageSanctuaire;
      target.OnCreatureDamage += OnDamageSanctuaire;

      Effect eff = Effect.LinkEffects(Effect.Sanctuary(spellDC), Effect.RunAction(onRemovedHandle: onRemoveSanctuaireCallback));
      eff.Tag = SanctuaireEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      return eff;
    }
    private static ScriptHandleResult OnRemoveSanctuaire(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is  NwCreature creature)
      {
        creature.OnCreatureAttack -= OnAttackSanctuaire;
        creature.OnSpellAction -= OnSpellActionSanctuaire;
        creature.OnCreatureDamage -= OnDamageSanctuaire;
      }

      return ScriptHandleResult.Handled;
    }
    private static void OnAttackSanctuaire(OnCreatureAttack onAttack)
    {
      EffectUtils.RemoveTaggedEffect(onAttack.Attacker, SanctuaireEffectTag);
    }
    private static void OnSpellActionSanctuaire(OnSpellAction onAttack)
    {
      EffectUtils.RemoveTaggedEffect(onAttack.Caster, SanctuaireEffectTag);
    }
    private static void OnDamageSanctuaire(OnCreatureDamage onAttack)
    {
      if(onAttack.DamagedBy is NwCreature damager)
        EffectUtils.RemoveTaggedEffect(damager, SanctuaireEffectTag);
    }
  }
}

