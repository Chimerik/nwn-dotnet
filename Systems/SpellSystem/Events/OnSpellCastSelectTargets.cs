using System.Linq;
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
        int nbTargets = 1;

        if (player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").HasNothing)
        {
          if (onSpellAction.TargetObject is not null
            && player.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoGemellite))
          {
            byte sourceCost = (byte)(onSpellAction.Spell.InnateSpellLevel < 1 ? 1 
              : onSpellAction.Spell.MasterSpell is not null ? onSpellAction.Spell.MasterSpell.InnateSpellLevel : onSpellAction.Spell.InnateSpellLevel);

            if (sourceCost > EnsoUtils.GetSorcerySource(player.LoginCreature))
            {
              player.SendServerMessage("Source magique insuffisante pour dédoubler ce sort", ColorConstants.Red);
            }
            else
            {
              EnsoUtils.DecrementSorcerySource(player.LoginCreature, sourceCost);
              EffectUtils.RemoveTaggedParamEffect(player.LoginCreature, CustomSkill.EnsoGemellite, EffectSystem.MetamagieEffectTag);

              nbTargets += 1;
            }
          }
          else
          {
            switch (onSpellAction.Spell.SpellType)
            {
              case Spell.Bane:
              case Spell.Bless: 
              case Spell.MagicMissile: 
              case Spell.Firebrand: nbTargets += 2;  break;
              case Spell.CharmPersonOrAnimal:
              case Spell.CharmPerson:
              //case Spell.HoldAnimal:
              case Spell.HoldMonster:
              case Spell.DominateAnimal:
                //case Spell.DominatePerson:

                if (player.ControlledCreature.KnowsFeat((Feat)CustomSkill.EnchantementPartage))
                  nbTargets += 1;

                break;

              case Spell.HoldPerson:

                if ((player.ControlledCreature.KnowsFeat((Feat)CustomSkill.EnchantementPartage)
                  || (onSpellAction.Feat.Id == CustomSkill.MonkPoigneDuVentDuNord && onSpellAction.Caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))))
                {
                  nbTargets += 1;
                }

                break;

              case (Spell)CustomSpell.DechargeOcculte: nbTargets = onSpellAction.Caster.Level > 4 ? onSpellAction.Caster.Level > 10 ? onSpellAction.Caster.Level > 17 ? 4 : 3 : 2 : 1; break;
            }
          }

          if (nbTargets > 1)
          {
            onSpellAction.PreventSpellCast = true;

            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_TO_SELECT").Value = nbTargets;
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_CASTING_CLASS").Value = onSpellAction.ClassIndex;
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value = onSpellAction.Spell.Id;

            if (onSpellAction.TargetObject is NwGameObject target)
            {
              player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_0").Value = target;
              player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Value = 1;
            }

            player.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureMagicTargetMode);
            player.SendServerMessage($"Sélectionnez jusqu'à {nbTargets} cibles", ColorConstants.Orange);
          }
          else
          {
            player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_1").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_2").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableObject<NwGameObject>>("_SPELL_TARGET_3").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGETS_SELECTED").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_CASTING_CLASS").Delete();
            player.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Delete();
          }
        }
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
          await caster.ActionCastSpellAt((Spell)caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value, caster, MetaMagic.None);
        }
        
        return;
      }

      if (selection.TargetObject is not NwGameObject target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureMagicTargetMode);
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
        await caster.ActionCastSpellAt((Spell)caster.GetObjectVariable<LocalVariableInt>("_SPELL_TARGET_SPELL_ID").Value, caster, MetaMagic.None);
      }
      else
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalSpellTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {remaningTargets} cible(s)", ColorConstants.Orange);
      }
    }
  }
}
