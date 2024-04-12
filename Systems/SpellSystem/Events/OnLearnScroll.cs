using Anvil.API.Events;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnLearnScroll(OnItemScrollLearn onScrollLearn)
    {
      NwCreature oPC = onScrollLearn.Creature;
      onScrollLearn.PreventLearnScroll = true;

      if (!Players.TryGetValue(onScrollLearn.Creature, out Player player))
        return;

      NwItem oScroll = onScrollLearn.Scroll;
      int spellId = SpellUtils.GetSpellIDFromScroll(oScroll);
      NwSpell spell = NwSpell.FromSpellId(spellId);

      if (spellId < 0 || spell.InnateSpellLevel > 10)
      {
        LogUtils.LogMessage($"LEARN SPELL FROM SCROLL - Player : {oPC.Name}, sort {spell.Name.ToString()} ({spellId}), SpellLevel : {spell.InnateSpellLevel} - INVALID", LogUtils.LogType.Learnables);
        oPC.ControllingPlayer.SendServerMessage("HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.", ColorConstants.Red);
        return;
      }

      if (player.learnableSpells.TryGetValue(spellId, out LearnableSpell learnable))
      {
        /*if (oScroll.GetObjectVariable<LocalVariableInt>("_ONE_USE_ONLY").HasValue && Config.env == Config.Env.Prod)
        {
          player.oid.SendServerMessage("Vous avez déjà retiré tout ce qui était possible de ce parchemin. Essayez d'en trouver une autre version pour en apprendre davantage", ColorConstants.Orange);
          return;
        }

        if (!learnable.canLearn) 
        {
          learnable.canLearn = true;
          oPC.ControllingPlayer.SendServerMessage($"L'étude des informations supplémentaires contenues dans ce parchemin vous permettra d'accéder à un niveau de maîtrise supérieur du sort {StringUtils.ToWhitecolor(learnable.name)}.", new Color(32, 255, 32));
        }
        else
        {*/

        if (learnable.currentLevel > 0)
        {
          if (!learnable.learntFromClasses.Contains(CustomClass.Wizard))
          {
            learnable.learntFromClasses.Add(CustomClass.Wizard);
            oPC.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(learnable.name)} est désormais inscrit dans votre livre de sorts de magicien", StringUtils.brightGreen);
            return;
          }
          else
          {
            oPC.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(learnable.name)} est déjà inscrit dans votre livre de sorts de magicien", ColorConstants.Orange);
            return;
          }
        }
        else
        {
          learnable.acquiredPoints += learnable.currentLevel > 1 ? (learnable.pointsToNextLevel - (5000 * (learnable.currentLevel - 1) * learnable.multiplier)) / 5
          : 1000 * learnable.multiplier;
          oPC.ControllingPlayer.SendServerMessage($"Les informations supplémentaires contenues dans ce parchemin vous permettent d'affiner votre connaissance du sort {StringUtils.ToWhitecolor(learnable.name)}. Votre étude sera plus rapide.", StringUtils.brightGreen);
        }
        //}
      }
      else
      {
        LearnableSpell learnableScroll = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId], CustomClass.Wizard);
        player.learnableSpells.Add(spellId, learnableScroll);
        oPC.ControllingPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(spell.Name.ToString())} a été ajouté à votre liste d'apprentissage de magicien et est désormais disponible pour étude.");

        switch(spell.SpellSchool)
        {
          case SpellSchool.Abjuration:

            if (player.learnableSkills.ContainsKey(CustomSkill.WizardAbjuration))
              learnableScroll.acquiredPoints += learnableScroll.pointsToNextLevel / 2;

            break;

          case SpellSchool.Divination:

            if (player.learnableSkills.ContainsKey(CustomSkill.WizardDivination))
              learnableScroll.acquiredPoints += learnableScroll.pointsToNextLevel / 2;

            break;

          case SpellSchool.Enchantment:

            if (player.learnableSkills.ContainsKey(CustomSkill.WizardEnchantement))
              learnableScroll.acquiredPoints += learnableScroll.pointsToNextLevel / 2;

            break;

          case SpellSchool.Evocation:

            if (player.learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
              learnableScroll.acquiredPoints += learnableScroll.pointsToNextLevel / 2;

            break;

          case SpellSchool.Illusion:

            if (player.learnableSkills.ContainsKey(CustomSkill.WizardIllusion))
              learnableScroll.acquiredPoints += learnableScroll.pointsToNextLevel / 2;

            break;

          case SpellSchool.Conjuration:

            if (player.learnableSkills.ContainsKey(CustomSkill.WizardInvocation))
              learnableScroll.acquiredPoints += learnableScroll.pointsToNextLevel / 2;

            break;
        }

        LogUtils.LogMessage($"SPELL SYSTEM - Player : {oPC.Name} vient d'ajouter {spell.Name.ToString()} ({spellId}) à sa liste d'apprentissage", LogUtils.LogType.Learnables);
      }

      if (player.TryGetOpenedWindow("learnables", out Player.PlayerWindow learnableWindow))
      {
        Player.LearnableWindow window = (Player.LearnableWindow)learnableWindow;
        window.LoadLearnableList(window.currentList);
      }

      if (oScroll.StackSize > 1)
        oScroll.StackSize -= 1;
      else
        oScroll.Destroy();
    }
  }
}

