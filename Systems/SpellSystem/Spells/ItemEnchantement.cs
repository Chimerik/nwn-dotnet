using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void Enchantement(OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
      if (player.craftJob != null)
      {
        player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
        return;
      }

      if (onSpellCast.TargetObject is not NwItem targetItem || targetItem == null || targetItem.RootPossessor != player.oid.ControlledCreature)
      {
        player.oid.SendServerMessage("Cible invalide.", ColorConstants.Red);
        return;
      }

      LearnableSkill inscription = player.learnableSkills[player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CASTING_INSCRIPTION").Value];

      switch (targetItem.BaseItem.ItemType)
      {
        case BaseItemType.Sickle:

          switch (inscription.id)
          {
            case CustomInscription.Pourfendeur:
            case CustomInscription.PourfendeurAberration:
            case CustomInscription.PourfendeurAnimal:
            case CustomInscription.PourfendeurArtificiel:
            case CustomInscription.PourfendeurBien:
            case CustomInscription.PourfendeurChaos:
            case CustomInscription.PourfendeurDemiElfe:
            case CustomInscription.PourfendeurDemiOrc:
            case CustomInscription.PourfendeurDragon:
            case CustomInscription.PourfendeurElementaire:
            case CustomInscription.PourfendeurElfe:
            case CustomInscription.PourfendeurExtérieur:
            case CustomInscription.PourfendeurGnome:
            case CustomInscription.PourfendeurGobelins:
            case CustomInscription.PourfendeurGéant:
            case CustomInscription.PourfendeurHalfelin:
            case CustomInscription.PourfendeurHumain:
            case CustomInscription.PourfendeurHumanoïdes:
            case CustomInscription.PourfendeurLoi:
            case CustomInscription.PourfendeurMagie:
            case CustomInscription.PourfendeurMal:
            case CustomInscription.PourfendeurMonstres:
            case CustomInscription.PourfendeurMétamorphes:
            case CustomInscription.PourfendeurNain:
            case CustomInscription.PourfendeurNeutralité:
            case CustomInscription.PourfendeurNonVie:
            case CustomInscription.PourfendeurOrc:
            case CustomInscription.PourfendeurReptilien:
            case CustomInscription.PourfendeurVermine:
            case CustomInscription.Zèle:
            case CustomInscription.Sismique:
            case CustomInscription.Incendiaire:
            case CustomInscription.Polaire:
            case CustomInscription.Electrocution:
            case CustomInscription.ForceEtHonneur:
            case CustomInscription.MaîtreDeSonDestin:
            case CustomInscription.DanseAvecLaMort:
            case CustomInscription.Sadisme:
            case CustomInscription.Masochisme:
            case CustomInscription.QueDuMuscle:
            case CustomInscription.VengeanceSeraMienne:
            case CustomInscription.AccueillezLaFaucheuse:
            case CustomInscription.Givroclaste:
            case CustomInscription.Pyroclaste:
            case CustomInscription.Electroclaste:
            case CustomInscription.Séismoclaste:
            case CustomInscription.Aiguillon:
            case CustomInscription.Rasoir:
            case CustomInscription.Fracasseur:
            case CustomInscription.Fureur:
            case CustomInscription.Pénétration:
            case CustomInscription.Barbelé:
            case CustomInscription.Atrocité:
            case CustomInscription.Handicapant:
            case CustomInscription.Pesanteur:
            case CustomInscription.Venimeuse:
            case CustomInscription.Mutisme:
            case CustomInscription.Extension:
              player.oid.SendServerMessage($"L'inscription {StringUtils.ToWhitecolor(inscription.name)} ne peut pas être calligraphiée sur un focus", ColorConstants.Red);
              return;
          }

          break;

        case BaseItemType.MagicStaff:

          switch(inscription.id)
          {
            case CustomInscription.Vampirisme:
            case CustomInscription.Zèle:
            case CustomInscription.Sismique:
            case CustomInscription.Incendiaire:
            case CustomInscription.Polaire:
            case CustomInscription.Electrocution:
              player.oid.SendServerMessage($"L'inscription {StringUtils.ToWhitecolor(inscription.name)} ne peut pas être calligraphiée sur un bourdon", ColorConstants.Red);
              return;
          }

          break;

        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
          if(inscription.id < CustomInscription.Blindé || inscription.id > CustomInscription.RepousseElementaire)
          {
            player.oid.SendServerMessage($"L'inscription {StringUtils.ToWhitecolor(inscription.name)} ne peut pas être calligraphiée sur un bouclier", ColorConstants.Red);
            return;
          }
          break;

        case BaseItemType.Amulet:
        case BaseItemType.Ring:
          if (inscription.id < CustomInscription.OnApprendDeSesErreurs || inscription.id > CustomInscription.Extension)
          {
            player.oid.SendServerMessage($"L'inscription {StringUtils.ToWhitecolor(inscription.name)} ne peut pas être calligraphiée sur un ornement", ColorConstants.Red);
            return;
          }
          break;

        case BaseItemType.Armor:
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
        case BaseItemType.Belt:
          if (inscription.id < CustomInscription.Cuirassé || inscription.id > CustomInscription.GardeElementaire)
          {
            player.oid.SendServerMessage($"L'inscription {StringUtils.ToWhitecolor(inscription.name)} ne peut pas être calligraphiée sur une pièce d'armure", ColorConstants.Red);
            return;
          }
          break;

        default:
          if (inscription.id < CustomInscription.Pourfendeur || inscription.id > CustomInscription.MateriaProductionSpeedSupreme)
          {
            player.oid.SendServerMessage($"L'inscription {StringUtils.ToWhitecolor(inscription.name)} ne peut pas être calligraphiée sur une arme", ColorConstants.Red);
            return;
          }
          break;
      }

      int skill = CustomSkill.CalligrapheFourbisseur;
      int masterSkill = CustomSkill.CalligrapheFourbisseurMaitre;
      int scienceSkill = CustomSkill.CalligrapheFourbisseurScience;
      int expertSkill = CustomSkill.CalligrapheFourbisseurExpert;

      switch(targetItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
          skill = CustomSkill.CalligrapheBlindeur;
          masterSkill = CustomSkill.CalligrapheBlindeurMaitre;
          scienceSkill = CustomSkill.CalligrapheBlindeurScience;
          expertSkill = CustomSkill.CalligrapheCiseleurExpert;
          break;

        case BaseItemType.Amulet:
        case BaseItemType.Ring:
          skill = CustomSkill.CalligrapheCiseleur;
          masterSkill = CustomSkill.CalligrapheCiseleurMaitre;
          scienceSkill = CustomSkill.CalligrapheCiseleurScience;
          expertSkill = CustomSkill.CalligrapheCiseleurExpert;
          break;

        case BaseItemType.Armor:
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
        case BaseItemType.Belt:
          skill = CustomSkill.CalligrapheArmurier;
          masterSkill = CustomSkill.CalligrapheArmurierMaitre;
          scienceSkill = CustomSkill.CalligrapheArmurierScience;
          expertSkill = CustomSkill.CalligrapheArmurierExpert;
          break;
      }

      double skillPoints = player.learnableSkills.ContainsKey(skill) ? player.learnableSkills[skill].totalPoints * 0.02 : 0;

      if (skillPoints == 0) 
      {
        player.oid.SendServerMessage($"Il est nécessaire de connaître les bases de la calligraphie sur {targetItem.BaseItem.ItemType} avant de pouvoir commencer ce travail !", ColorConstants.Red);
        return;
      }

      if (targetItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
      {
        player.oid.SendServerMessage($"{targetItem.Name.ColorString(ColorConstants.White)} n'a plus d'emplacement d'inscription disponible.", ColorConstants.Red);
        return;
      }

      NwItem inscriptionTool = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (inscriptionTool is null || !inscriptionTool.IsValid)
      {
        player.oid.SendServerMessage("Votre main droite doit être équipée d'un objet disposant d'une inscription de calligraphie pour pouvoir commencer ce travail.", ColorConstants.Red);
        return;
      }

      bool inscriptionCraft = false;
      double reduction = 0;

      for (int i = 0; i < inscriptionTool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        int inscriptionId = inscriptionTool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value;
        if (inscriptionId >= CustomInscription.MateriaInscriptionDurabilityMinor && inscriptionId <= CustomInscription.MateriaInscriptionSpeedSupreme)
          inscriptionCraft = true;

        switch(inscriptionId)
        {
          case CustomInscription.MateriaInscriptionYieldMinor: reduction += 0.02; break;
          case CustomInscription.MateriaInscriptionYield: reduction += 0.04; break;
          case CustomInscription.MateriaInscriptionYieldMajor: reduction += 0.06; break;
          case CustomInscription.MateriaInscriptionYieldSupreme: reduction += 0.08; break;
        }
      }

      if(!inscriptionCraft)
      {
        player.oid.SendServerMessage("Votre main droite doit être équipée d'un objet disposant d'une inscription de calligraphie pour pouvoir commencer ce travail.", ColorConstants.Red);
        return;
      }

      skillPoints += player.learnableSkills.ContainsKey(masterSkill) ? player.learnableSkills[masterSkill].totalPoints * 0.02 : 0;
      skillPoints += player.learnableSkills.ContainsKey(scienceSkill) ? player.learnableSkills[scienceSkill].totalPoints * 0.03 : 0;
      skillPoints += player.learnableSkills.ContainsKey(expertSkill) ? player.learnableSkills[expertSkill].totalPoints * 0.03 : 0;
     
      double cost = Math.Pow(2, inscription.multiplier) * 100;
      cost *= 1 - skillPoints;
      cost *= 1 - reduction;

      int availableInflux = 0;

      foreach (NwItem item in player.oid.LoginCreature.Inventory.Items)
        if (item.Tag == "dose_influx_pur")
          availableInflux += item.StackSize;

      if(availableInflux < cost)
      {
        player.oid.SendServerMessage($"Il vous manque {(int)cost - availableInflux} dose(s) d'influx pur pour pouvoir commencer ce travail !", ColorConstants.Red);
        return;
      }

      foreach (NwItem item in player.oid.LoginCreature.Inventory.Items)
      {
        if (item.Tag == "dose_influx_pur")
        {
          if (item.StackSize > cost)
          {
            item.StackSize -= (int)cost;
            break;
          }
          else if (item.StackSize == cost)
            item.Destroy();
          else if (item.StackSize < cost)
          {
            item.Destroy();
            cost -= item.StackSize;
          }
        }
      }

      player.craftJob = new PlayerSystem.CraftJob(player, targetItem, inscription, PlayerSystem.JobType.Enchantement);
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CASTING_INSCRIPTION").Delete();

      ItemUtils.HandleCraftToolDurability(player, inscriptionTool, CustomInscription.MateriaInscriptionDurability, skill);
    }
  }
}
