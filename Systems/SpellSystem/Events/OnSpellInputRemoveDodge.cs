using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellInputRemoveDodge(OnSpellAction onSpellAction)
    {
      switch(onSpellAction.Spell.Id)
      {
        case CustomSpell.Sprint:
        case CustomSpell.Dodge:
        case CustomSpell.Disengage: return;
      }

      foreach(var eff in onSpellAction.Caster.ActiveEffects)
        if(eff.Tag == EffectSystem.DodgeEffectTag)
          onSpellAction.Caster.RemoveEffect(eff);

      onSpellAction.Caster.OnSpellAction -= OnSpellInputRemoveDodge;
      onSpellAction.Caster.OnCreatureAttack -= CreatureUtils.OnAttackRemoveDodge;
    }
  }
}
