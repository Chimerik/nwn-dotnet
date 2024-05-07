using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializePlayerTlk()
      {
        if (learnableSkills.ContainsKey(CustomSkill.FighterArcaneArcher))
        {
          new StrRef(8).SetPlayerOverride(oid, "Archer-Mage");
          oid.SetTextureOverride("fighter", "ir_archer");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterChampion))
        {
          new StrRef(8).SetPlayerOverride(oid, "Champion");
          oid.SetTextureOverride("fighter", "champion");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterWarMaster))
        {
          new StrRef(8).SetPlayerOverride(oid, "Maître de Guerre");
          oid.SetTextureOverride("fighter", "warmaster");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.FighterEldritchKnight))
        {
          new StrRef(8).SetPlayerOverride(oid, "Guerrier Occulte");
          oid.SetTextureOverride("fighter", "eldritchknight");
        }

        if (learnableSkills.ContainsKey(CustomSkill.BarbarianBerseker))
        {
          new StrRef(5213).SetPlayerOverride(oid, "Berseker");
          oid.SetTextureOverride("barbarian", "berseker");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianTotem))
        {
          new StrRef(5213).SetPlayerOverride(oid, "Voie du Totem");
          oid.SetTextureOverride("barbarian", "totem");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianWildMagic))
        {
          new StrRef(5213).SetPlayerOverride(oid, "Voie de la Magie Sauvage");
          oid.SetTextureOverride("barbarian", "wildmagic");
        }
        
        if (learnableSkills.ContainsKey(CustomSkill.RogueThief))
        {
          new StrRef(16).SetPlayerOverride(oid, "Voleur");
          oid.SetTextureOverride("rogue", "thief");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueAssassin))
        {
          new StrRef(16).SetPlayerOverride(oid, "Assassin");
          oid.SetTextureOverride("rogue", "assassin");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueArcaneTrickster))
        {
          new StrRef(16).SetPlayerOverride(oid, "Escroc Arcanique");
          oid.SetTextureOverride("rogue", "arcane_trickster");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.RogueConspirateur))
        {
          new StrRef(16).SetPlayerOverride(oid, "Conspirateur");
          oid.SetTextureOverride("rogue", "conspirateur");
        }

        if (learnableSkills.ContainsKey(CustomSkill.MonkPaume))
        {
          new StrRef(10).SetPlayerOverride(oid, "Voie de la Paume");
          oid.SetTextureOverride("monk", "monk_paume");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.MonkOmbre))
        {
          new StrRef(10).SetPlayerOverride(oid, "Voie de l'Ombre");
          oid.SetTextureOverride("monk", "monk_shadow");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.MonkElements))
        {
          new StrRef(10).SetPlayerOverride(oid, "Voie des Eléments");
          oid.SetTextureOverride("monk", "monk_elements");
        }

        if (learnableSkills.ContainsKey(CustomSkill.WizardAbjuration))
        {
          new StrRef(20).SetPlayerOverride(oid, "Abjurateur");
          oid.SetTextureOverride("wizard", "abjuration");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardDivination))
        {
          new StrRef(20).SetPlayerOverride(oid, "Devin");
          oid.SetTextureOverride("wizard", "divination");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEnchantement))
        {
          new StrRef(20).SetPlayerOverride(oid, "Enchanteur");
          oid.SetTextureOverride("wizard", "enchantement");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Evocateur");
          oid.SetTextureOverride("wizard", "evocation");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Illusionniste");
          oid.SetTextureOverride("wizard", "illusion");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardEvocation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Invocateur");
          oid.SetTextureOverride("wizard", "invocation");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardNecromancie))
        {
          new StrRef(20).SetPlayerOverride(oid, "Nécromancien");
          oid.SetTextureOverride("wizard", "necromancie");
        }
        else if (learnableSkills.ContainsKey(CustomSkill.WizardTransmutation))
        {
          new StrRef(20).SetPlayerOverride(oid, "Transmutateur");
          oid.SetTextureOverride("wizard", "transmutation");
        }

        if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDuSavoir))
        {
          new StrRef(2).SetPlayerOverride(oid, "Collège du Savoir");
          oid.SetTextureOverride("bard", "college_lore");
        }
      }
    }
  }
}
