using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastSelectTargets(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster.IsLoginPlayerCharacter(out NwPlayer player))
      {
        switch (onSpellCast.Spell.SpellType)
        {
          case Spell.Bane:
          case Spell.Firebrand:

            if (player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").HasNothing)
            {
              onSpellCast.PreventSpellCast = true;

              player.EnterTargetMode(SelectBaneTarget, Config.selectCreatureTargetMode);
              player.SendServerMessage("Sélectionnez jusqu'à trois cibles", ColorConstants.Orange);
            }
            else
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();

            break;

          default:

            player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>("_SPELL_TARGET_1").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>("_SPELL_TARGET_2").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>("_SPELL_TARGET_3").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();

            break;
        }
      }
    }
    private static void SelectBaneTarget(ModuleEvents.OnPlayerTarget selection)
    {
      NwCreature caster = selection.Player.LoginCreature;
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;

      if(selection.TargetObject is not NwCreature target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectBaneTarget, Config.selectCreatureTargetMode);
        caster.LoginPlayer.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (selection.IsCancelled)
      {
        if (nbTargets > 0)
        {
          caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Value = 1;

          switch(caster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Value)
          {
            case (int)Spell.Bane: _ = caster.ActionCastSpellAt(Spell.Bane, caster);break;
            case (int)Spell.Firebrand: _ = caster.ActionCastSpellAt(Spell.Firebrand, caster);break;
          }
          
        }
        
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value += 1;
      caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_SPELL_TARGET_{nbTargets + 1}").Value = target;
      caster.LoginPlayer.EnterTargetMode(SelectBaneTarget, Config.selectCreatureTargetMode);
      caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {3 - nbTargets} cible(s)", ColorConstants.Orange);

      if(nbTargets > 1)
      {
        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Value = 1;

        switch (caster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Value)
        {
          case (int)Spell.Bane: _ = caster.ActionCastSpellAt(Spell.Bane, caster); break;
          case (int)Spell.Firebrand: _ = caster.ActionCastSpellAt(Spell.Firebrand, caster); break;
        }
      }
    }
  }
}
