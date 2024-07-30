using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastSelectTargets(OnSpellAction onSpellAction)
    {
      if (onSpellAction.Caster.IsLoginPlayerCharacter(out NwPlayer player))
      {
        switch (onSpellAction.Spell.SpellType)
        {
          case Spell.Bane:
          case Spell.Bless:
          case Spell.Firebrand:

            if (player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").HasNothing)
            {
              onSpellAction.PreventSpellCast = true;

              if(onSpellAction.Spell.SpellType == Spell.Firebrand && onSpellAction.Feat.Id == CustomSkill.MonkEtreinteDeLenfer
                && onSpellAction.Caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value = 4;
              else
                player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value = 3;

              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_CASTING_CLASS").Value = onSpellAction.ClassIndex;
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value = onSpellAction.Spell.Id;
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

            if((player.ControlledCreature.KnowsFeat((Feat)CustomSkill.EnchantementPartage) 
              || (onSpellAction.Spell.SpellType == Spell.HoldPerson && onSpellAction.Feat.Id == CustomSkill.MonkPoigneDuVentDuNord
                && onSpellAction.Caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire)))
              && player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").HasNothing)
            {
              onSpellAction.PreventSpellCast = true;

              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value = 2;
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_CASTING_CLASS").Value = onSpellAction.ClassIndex;
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value = onSpellAction.Spell.Id;

              if (onSpellAction.TargetObject is NwGameObject target)
              {
                player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_0").Value = target;
                player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value = 1;
              }

              player.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
              player.SendServerMessage("Sélectionnez jusqu'à deux cibles", ColorConstants.Orange);
            }
            else
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();

            return;
          }

        player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_1").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_2").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_3").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_CASTING_CLASS").Delete();
        player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Delete();
      }
    }
    private static async void SelectAdditionnalSpellTargets(ModuleEvents.OnPlayerTarget selection)
    {
      NwCreature caster = selection.Player.LoginCreature;
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value;
    
      if (selection.IsCancelled)
      {
        if (nbTargets > 0)
        {
          caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Value = 1;
          await caster.ClearActionQueue();
          await caster.ActionCastSpellAt((Spell)caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value, caster);
        }
        
        return;
      }

      if (selection.TargetObject is not NwGameObject target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
        caster.LoginPlayer.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_SPELL_TARGET_{nbTargets}").Value = target;

      nbTargets += 1;
      caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value = nbTargets;
      int remaningTargets = caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value - nbTargets;
      
      if (remaningTargets < 1)
      {
        caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Value = 1;

        await caster.ClearActionQueue();
        await caster.ActionCastSpellAt((Spell)caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value, caster);
      }
      else
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureTargetMode);
        caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {remaningTargets} cible(s)", ColorConstants.Orange);
      }
    }
  }
}
