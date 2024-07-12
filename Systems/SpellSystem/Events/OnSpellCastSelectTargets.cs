using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

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

                player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value = 3;
                player.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
                player.SendServerMessage("Sélectionnez jusqu'à trois cibles", ColorConstants.Orange);
              }
              else
                player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();

              return;

          case (Spell)CustomSpell.AmitieAnimale:
          case Spell.CharmPerson:
          case Spell.HoldPerson:
          //case Spell.HoldAnimal:
          case Spell.HoldMonster:
          case Spell.DominateAnimal:
          //case Spell.DominatePerson:

            if(player.ControlledCreature.KnowsFeat((Feat)CustomSkill.EnchantementPartage)
              && player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").HasNothing)
            {
              onSpellCast.PreventSpellCast = true;

              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value = 2;
              player.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
              player.SendServerMessage("Sélectionnez jusqu'à deux cibles", ColorConstants.Orange);
            }
            else
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();

            return;
          }

        player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>("_SPELL_TARGET_1").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>("_SPELL_TARGET_2").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>("_SPELL_TARGET_3").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();
      }
    }
    private static void SelectAdditionnalSpellTargets(ModuleEvents.OnPlayerTarget selection)
    {
      NwCreature caster = selection.Player.LoginCreature;
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;

      if(selection.TargetObject is not NwCreature target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
        caster.LoginPlayer.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      if (selection.IsCancelled)
      {
        if (nbTargets > 0)
        {
          caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Value = 1;
          _ = caster.ActionCastSpellAt((Spell)caster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Value, caster);          
        }
        
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value += 1;
      caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_SPELL_TARGET_{nbTargets + 1}").Value = target;
      caster.LoginPlayer.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
      
      int totalTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value;
      caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {totalTargets - nbTargets} cible(s)", ColorConstants.Orange);
      
      if (nbTargets > totalTargets - 2)
      {
        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Value = 1;
        _ = caster.ActionCastSpellAt((Spell)caster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Value, caster);
      }
    }
  }
}
