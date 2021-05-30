using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Collect.System;

namespace NWN.Systems.Craft
{
  public class Job
  {
    public JobType type;
    public int baseItemType { get; }
    public string craftedItem { get; set; }
    public float remainingTime { get; set; }
    public string material { get; set; }
    public Boolean isCancelled { get; set; }
    private readonly PlayerSystem.Player player;

    public Job(int baseItemType, string material, float time, PlayerSystem.Player player, string item = "")
    {
      ModuleSystem.Log.Info($"Craft new job from {player.oid.LoginCreature.Name} - base item : {baseItemType} - remaining Time {time} - material {material}");
      //this.name = name;
      this.baseItemType = baseItemType;
      this.craftedItem = item;
      remainingTime = time;
      this.material = material;
      this.isCancelled = false;
      this.player = player;

      switch (baseItemType)
      {
        case -11:
          this.type = JobType.BlueprintCopy;
          break;
        case -12:
          this.type = JobType.BlueprintResearchMaterialEfficiency;
          break;
        case -13:
          this.type = JobType.BlueprintResearchTimeEfficiency;
          break;
        case -14:
          this.type = JobType.Enchantement;
          break;
        case -15:
          this.type = JobType.Recycling;
          break;
        case -16:
          this.type = JobType.Renforcement;
          break;
        case -17:
          this.type = JobType.Repair;
          break;
        case -18:
          this.type = JobType.EnchantementReactivation;
          break;
        default:
          this.type = JobType.Item;
          break;
      }

      if (IsActive())
      {
        player.oid.ExportCharacter();
        this.CreateCraftJournalEntry();
      }
    }
    public enum JobType
    {
      Invalid = 0,
      Item = 1,
      BlueprintCopy = 2,
      BlueprintResearchMaterialEfficiency = 3,
      BlueprintResearchTimeEfficiency = 4,
      Enchantement = 5,
      Recycling = 6,
      Renforcement = 7,
      Repair = 8,
      EnchantementReactivation = 9,
    }
    public Boolean IsActive()
    {
      if (baseItemType == -10)
        return false;
      return true;
    }
    public void ResetCancellation()
    {
      this.isCancelled = false;
    }
    public void AskCancellationConfirmation(NwPlayer player)
    {
      player.SendServerMessage($"Attention, votre travail précédent n'est pas terminé. Lancer un nouveau travail signifie perdre la totalité du travail en cours !", Color.MAGENTA);
      player.SendServerMessage($"Utilisez une seconde fois le plan pour confirmer l'annulation du travail en cours.", Color.SILVER);
      this.isCancelled = true;

      Task waitSpellUsed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(60));
        ResetCancellation();
      });
    }
    public Boolean CanStartJob(NwPlayer player, NwItem blueprint, JobType type)
    {
      if (blueprint != null)
      {
        if (type == JobType.BlueprintCopy || type == JobType.BlueprintResearchMaterialEfficiency || type == JobType.BlueprintResearchTimeEfficiency) // Dans le cas d'une copie ou d'une recherche de BP
        {
          if (!IsBlueprintOriginal(blueprint))
          {
            player.SendServerMessage("Il vous faut un patron original afin d'effectuer une recherche ou une copie.", Color.ORANGE);
            return false;
          }
        }

        switch (type)
        {
          case JobType.BlueprintResearchTimeEfficiency:
            if (blueprint.GetLocalVariable<int>("_BLUEPRINT_TIME_EFFICIENCY").Value >= 10)
            {
              player.SendServerMessage("Ce patron dispose déjà d'un niveau de recherche maximal.", Color.ORANGE);
              return false;
            }
            break;
          case JobType.BlueprintResearchMaterialEfficiency:
            if (blueprint.GetLocalVariable<int>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value >= 10)
            {
              player.SendServerMessage("Ce patron dispose déjà d'un niveau de recherche métallurgique maximal.", Color.ORANGE);
              return false;
            }
            break;
        }
      }

      if (IsActive() && !isCancelled)
      {
        AskCancellationConfirmation(player);
        return false;
      }

      if (this.isCancelled)
      {
        if(craftedItem != "")
          player.LoginCreature.AcquireItem(NwItem.Deserialize(craftedItem.ToByteArray()));
      }

      return true;
    }
    private Boolean IsBlueprintOriginal(NwItem oBlueprint)
    {
      if (oBlueprint.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value > 0)
        return false;
      else
        return true;
    }
    public void Start(JobType type, Blueprint blueprint, PlayerSystem.Player player, NwItem oItem, NwGameObject oTarget = null, string sMaterial = "")
    {
      switch (type)
      {
        case JobType.Item:
          StartItemCraft(blueprint, oItem, oTarget, sMaterial);
          break;
        case JobType.Enchantement:
          StartEnchantementCraft(oTarget, sMaterial);
          break;
        case JobType.Recycling:
          StartRecycleCraft(oTarget, sMaterial);
          break;
        case JobType.BlueprintCopy:
          StartBlueprintCopy(player, oItem, blueprint);
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          StartBlueprintTimeEfficiencyResearch(player, oItem, blueprint);
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          StartBlueprintMaterialEfficiencyResearch(player, oItem, blueprint);
          break;
        case JobType.Renforcement:
          StartRenforcementCraft(oTarget);
          break;
        case JobType.Repair:
          StartItemRepair(blueprint, oItem, oTarget, sMaterial);
          break;
        case JobType.EnchantementReactivation:
          StartEnchantementReactivationCraft(oTarget, sMaterial);
          break;
      }

    }
    public void StartItemCraft(Blueprint blueprint, NwItem oItem, NwGameObject oTarget, string sMaterial)
    {
      int iMineralCost = blueprint.GetBlueprintMineralCostForPlayer(player.oid, oItem);
      float iJobDuration = blueprint.GetBlueprintTimeCostForPlayer(player.oid, oItem);

      int materialType = 0;
      if (Enum.TryParse(material, out MineralType myMineralType))
        materialType = (int)myMineralType;
      else if (Enum.TryParse(material, out PlankType myPlankType))
        materialType = (int)myPlankType;
      else if (Enum.TryParse(material, out LeatherType myLeatherType))
        materialType = (int)myLeatherType;

      iMineralCost -= iMineralCost * (int)materialType / 10;
      player.craftJob.isCancelled = false;

      if (!player.materialStock.ContainsKey(sMaterial))
      {
        player.oid.SendServerMessage($"Il vous manque {iMineralCost.ToString().ColorString(Color.WHITE)} unités de {sMaterial.ColorString(Color.WHITE)} pour réaliser ce travail artisanal.", Color.RED);
        return;
      }
      else if (player.materialStock[sMaterial] < iMineralCost)
      {
        player.oid.SendServerMessage($"Il vous manque {(iMineralCost - player.materialStock[sMaterial]).ToString().ColorString(Color.WHITE)} unités de {sMaterial.ColorString(Color.WHITE)} pour réaliser ce travail artisanal.", Color.RED);
        return;
      }
        
      player.materialStock[sMaterial] -= iMineralCost;

      player.oid.SendServerMessage($"Vous venez de démarrer la fabrication de l'objet artisanal : {blueprint.name.ColorString(Color.WHITE)} en {sMaterial.ColorString(Color.WHITE)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels sur la forge

      if (oTarget.Tag == blueprint.craftedItemTag) // En cas d'amélioration d'un objet, on détruit l'original
      {
        player.craftJob = new Job(blueprint.baseItemType, sMaterial, iJobDuration, player, oTarget.Serialize().ToBase64EncodedString());
        oTarget.Destroy();
        player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail artisanal.", Color.ORANGE);
      }
      else
        player.craftJob = new Job(blueprint.baseItemType, sMaterial, iJobDuration, player);

      // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
      int iBlueprintRemainingRuns = oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value;
      if (iBlueprintRemainingRuns == 1)
        oItem.Destroy();
      if (iBlueprintRemainingRuns > 0)
        oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value -= 1;

      ItemUtils.DecreaseItemDurability(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand));
    }
    public void StartItemRepair(Blueprint blueprint, NwItem oItem, NwGameObject oTarget, string sMaterial)
    {
      player.craftJob.isCancelled = false;
      int iMineralCost = blueprint.GetBlueprintMineralCostForPlayer(player.oid, oItem);
      float iJobDuration = blueprint.GetBlueprintTimeCostForPlayer(player.oid, oItem);

      if (oTarget.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value == player.oid.LoginCreature.Name)
      {
        iMineralCost -= iMineralCost / 4;
        iJobDuration -= iJobDuration / 4;
      }

      int materialType = 0;
      Dictionary<string, int> repairMaterials = new Dictionary<string, int>();
      if (Enum.TryParse(material, out MineralType myMineralType))
      {
        iMineralCost -= iMineralCost * (int)materialType / 10;

        for (int i = (int)materialType - 1; i == 1; i--)
        {
          repairMaterials.Add(myMineralType.ToString(), iMineralCost);
          iMineralCost += iMineralCost * (10 / 100);
        }
      }
      else if (Enum.TryParse(material, out PlankType myPlankType))
      {
        iMineralCost -= iMineralCost * (int)materialType / 10;

        for (int i = (int)materialType; i == 1; i--)
        {
          repairMaterials.Add(myMineralType.ToString(), iMineralCost);
          iMineralCost += iMineralCost * (10 / 100);
        }
      }
      else if (Enum.TryParse(material, out LeatherType myLeatherType))
      {
        iMineralCost -= iMineralCost * (int)materialType / 10;

        for (int i = (int)materialType; i == 1; i--)
        {
          repairMaterials.Add(myMineralType.ToString(), iMineralCost);
          iMineralCost += iMineralCost * (10 / 100);
        }
      }

      foreach (KeyValuePair<string, int> materials in repairMaterials)
      {
        if (!player.materialStock.ContainsKey(materials.Key))
        {
          player.oid.SendServerMessage($"Il vous manque {materials.Value.ToString().ColorString(Color.WHITE)} unités de {materials.Key.ColorString(Color.WHITE)} pour réaliser ce travail artisanal.", Color.RED);
          return;
        }
        else if (player.materialStock[materials.Key] < materials.Value)
        {
          player.oid.SendServerMessage($"Il vous manque {(materials.Value - player.materialStock[materials.Key]).ToString().ColorString(Color.WHITE)} unités de {materials.Key.ColorString(Color.WHITE)} pour réaliser ce travail artisanal.", Color.RED);
          return;
        }
      }

      foreach (KeyValuePair<string, int> materials in repairMaterials)
        player.materialStock[materials.Key] -= materials.Value;

      player.oid.SendServerMessage($"Vous venez de démarrer la réparation de l'objet artisanal : {blueprint.name.ColorString(Color.WHITE)} en {sMaterial.ColorString(Color.WHITE)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels sur la forge

      player.craftJob = new Job(-17, sMaterial, iJobDuration, player, oTarget.Serialize().ToBase64EncodedString());
      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail artisanal.", Color.ORANGE);

      // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
      int iBlueprintRemainingRuns = oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value;
      if (iBlueprintRemainingRuns == 1)
        oItem.Destroy();
      if (iBlueprintRemainingRuns > 0)
        oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value -= 1;

      ItemUtils.DecreaseItemDurability(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand));
    }
    public void StartBlueprintCopy(PlayerSystem.Player player, NwItem oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintCopy))
      {
        int BPCopyLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.BlueprintCopy))
          BPCopyLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BlueprintCopy, player.learntCustomFeats[CustomFeats.BlueprintCopy]);

        int timeCost = blueprint.mineralsCost * 200;
        float iJobDuration =  timeCost * (100 - (BPCopyLevel * 5)) / 100;
        player.craftJob = new Job(-11, "", iJobDuration, player, oBlueprint.Serialize().ToBase64EncodedString()); // - 11 = blueprint copy
      }
    }
    private void StartEnchantementCraft(NwGameObject oTarget, string ipString)
    {
      int spellId = int.Parse(ipString.Remove(ipString.IndexOf("_")));

      if (!float.TryParse(NWScript.Get2DAString("spells", "Wiz_Sorc", spellId), out float spellLevel))
      {
        player.oid.SendServerMessage("HRP - Le niveau de sort de votre enchantement est incorrectement configuré. Le staff a été prévenu !");
        Utils.LogMessageToDMs($"ENCHANTEMENT - {player.oid.LoginCreature.Name} - spell level introuvable pour spellid : {spellId}");
        return;
      }

      if (spellLevel < 1)
        spellLevel = 0.5f;

      int baseCost = ItemUtils.GetBaseItemCost((NwItem)oTarget);
      int enchanteurLevel = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Enchanteur))
        enchanteurLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Enchanteur, player.learntCustomFeats[CustomFeats.Enchanteur]);

      float iJobDuration = baseCost * 10 * spellLevel * (100 - enchanteurLevel);
      player.craftJob = new Job(-14, ipString, iJobDuration, player, oTarget.Serialize().ToBase64EncodedString()); // -14 = JobType enchantement

      player.oid.SendServerMessage($"Vous venez de démarrer l'enchantement de : {NWScript.GetName(oTarget).ColorString(Color.WHITE)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels

      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.", Color.ORANGE);

      player.craftJob.isCancelled = false;
    }
    private void StartEnchantementReactivationCraft(NwGameObject oTarget, string ipString)
    {
      string[] IPproperties = ipString.Split("_");
      int spellId = int.Parse(IPproperties[0]);
      int costValue = int.Parse(IPproperties[1]);
      int originalEnchanterId = int.Parse(IPproperties[2]);

      if (!float.TryParse(NWScript.Get2DAString("spells", "Wiz_Sorc", spellId), out float spellLevel))
      {
        player.oid.SendServerMessage("HRP - Le niveau de sort de votre enchantement est incorrectement configuré. Le staff a été prévenu !");
        Utils.LogMessageToDMs($"ENCHANTEMENT - {player.oid.LoginCreature.Name} - spell level introuvable pour spellid : {spellId}");
        return;
      }

      if (spellLevel < 1)
        spellLevel = 0.5f;

      int baseCost = ItemUtils.GetBaseItemCost((NwItem)oTarget);
      int enchanteurLevel = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Enchanteur))
        enchanteurLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Enchanteur, player.learntCustomFeats[CustomFeats.Enchanteur]);

      float iJobDuration = baseCost * 10 * spellLevel * (100 - enchanteurLevel) * costValue;
      if (player.characterId == originalEnchanterId)
        iJobDuration -= iJobDuration / 4;

      player.craftJob = new Job(-18, spellId.ToString(), iJobDuration, player, oTarget.Serialize().ToBase64EncodedString()); // -18 = JobType enchantementReactivation

      player.oid.SendServerMessage($"Vous venez de démarrer la réactivation d'enchantement de : {NWScript.GetName(oTarget).ColorString(Color.WHITE)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels

      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.", Color.ORANGE);

      player.craftJob.isCancelled = false;
    }
    private void StartRecycleCraft(NwGameObject oTarget, string material)
    {
      NwItem item = (NwItem)oTarget;

      int baseItemType = (int)item.BaseItemType;
      int baseCost = ItemUtils.GetBaseItemCost(item);

      float iJobDuration = baseCost * 125;

      if (this.player.learntCustomFeats.ContainsKey(CustomFeats.Recycler))
        iJobDuration -= iJobDuration * 1 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Recycler, this.player.learntCustomFeats[CustomFeats.Recycler]) / 100;

      item.GetLocalVariable<int>("_BASE_COST").Value = baseCost;
      player.craftJob = new Job(-15, material, iJobDuration, player, item.Serialize().ToBase64EncodedString()); // -15 = JobType recyclage

      player.oid.SendServerMessage($"Vous venez de démarrer le recyclage de : {item.Name.ColorString(Color.WHITE)}", Color.ORANGE);
      // TODO : afficher des effets visuels

      item.Destroy();
      player.oid.SendServerMessage($"L'objet {item.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail de recyclage.", Color.RED);

      player.craftJob.isCancelled = false;
    }
    private void StartRenforcementCraft(NwGameObject oTarget)
    {
      NwItem item = (NwItem)oTarget;

      if(item.GetLocalVariable<int>("_REINFORCEMENT_LEVEL").Value >= 10)
      {
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.WHITE)} a déjà été renforcé au maximum des capacités du matériau.", Color.ORANGE);
        return;
      }

      int renforcementLevel = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.Renforcement))
        renforcementLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Renforcement, player.learntCustomFeats[CustomFeats.Renforcement]);

      int baseItemType = (int)item.BaseItemType;
      int baseCost = ItemUtils.GetBaseItemCost(item);

      float iJobDuration = baseCost * 100 * (100 - renforcementLevel * 5) / 100;

      item.GetLocalVariable<int>("_BASE_COST").Value = baseCost;
      player.craftJob = new Job(-16, material, iJobDuration, player, item.Serialize().ToBase64EncodedString()); // -16 = JobType recyclage

      player.oid.SendServerMessage($"Vous venez de démarrer le renforcement de : {item.Name.ColorString(Color.WHITE)}", Color.ORANGE);
      // TODO : afficher des effets visuels

      item.Destroy();
      player.oid.SendServerMessage($"L'objet {item.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail de renforcement.", Color.RED);

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintMaterialEfficiencyResearch(PlayerSystem.Player player, NwItem oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchMaterialEfficiency))
      {
        int metallurgyLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.Metallurgy))
          metallurgyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Metallurgy, player.learntCustomFeats[CustomFeats.Metallurgy]);

        int advancedCraftLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.AdvancedCraft))
          advancedCraftLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AdvancedCraft, player.learntCustomFeats[CustomFeats.AdvancedCraft]);

        float iJobDuration = blueprint.mineralsCost * 200 * (100 - (metallurgyLevel * 5 + advancedCraftLevel * 3)) / 100;
        player.craftJob = new Job(-12, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 12 = recherche ME
        oBlueprint.Destroy();
        player.oid.SendServerMessage($"L'objet {oBlueprint.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail de recherche métallurgique.", Color.ORANGE);
      }
    }
    public void StartBlueprintTimeEfficiencyResearch(PlayerSystem.Player player, NwItem oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchTimeEfficiency))
      {
        int researchLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.Research))
          researchLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Research, player.learntCustomFeats[CustomFeats.Research]);

        int advancedCraftLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.AdvancedCraft))
          advancedCraftLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AdvancedCraft, player.learntCustomFeats[CustomFeats.AdvancedCraft]);

        float iJobDuration = blueprint.mineralsCost * 200 * (100 - (researchLevel * 5 + advancedCraftLevel * 3)) / 100;
        
        player.craftJob = new Job(-13, "", iJobDuration, player, oBlueprint.Serialize().ToBase64EncodedString()); // -13 = recherche TE
        oBlueprint.Destroy();
        player.oid.SendServerMessage($"L'objet {oBlueprint.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail de recherche d'efficacité.", Color.ORANGE);
      }
    }
    public void CreateCraftJournalEntry()
    {
      if (!IsActive())
        return;

      player.playerJournal.craftJobCountDown = DateTime.Now.AddSeconds(remainingTime);

      JournalEntry journalEntry = new JournalEntry();
      if(remainingTime > 0)
        journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.craftJobCountDown - DateTime.Now))}";
      else
        journalEntry.sName = $"Travail artisanal - Terminé !";

      switch (this.type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sText = $"Copie de {NwItem.Deserialize(craftedItem.ToByteArray()).Name} en cours";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 631);
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sText = $"Recherche métallurgique en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 792);
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sText = $"Recherche d'efficacité en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 792);
          break;
        case JobType.Enchantement:
          journalEntry.sText = $"Enchantement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          API.Effect enchantementVfx = API.Effect.VisualEffect((VfxType)832);
          enchantementVfx.Tag = "VFX_ENCHANTEMENT";
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, enchantementVfx);
          break;
        case JobType.Recycling:
          journalEntry.sText = $"Recyclage en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 818);
          break;
        case JobType.Renforcement:
          journalEntry.sText = $"Renforcement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 829);
          break;
        case JobType.Repair:
          journalEntry.sText = $"Réparation en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1501);
          break;
        case JobType.EnchantementReactivation:
          journalEntry.sText = $"Enchantement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          API.Effect reactivationVfx = API.Effect.VisualEffect((VfxType)832);
          reactivationVfx.Tag = "VFX_ENCHANTEMENT";
          journalEntry.sText = $"Ré-enchantement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          break;
        default:
          journalEntry.sText = $"Fabrication en cours : {blueprintDictionnary[baseItemType].name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1501);
          break;
      }

      journalEntry.sTag = "craft_job";
      journalEntry.nPriority = 1;
      journalEntry.nQuestDisplayed = 1;
      PlayerPlugin.AddCustomJournalEntry(player.oid.LoginCreature, journalEntry);
    }
    public void CancelCraftJournalEntry()
    {
      JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid.LoginCreature, "craft_job");

      switch (type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sName = $"Travail artisanal en pause - Copie de patron";
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sName = $"Travail artisanal en pause - Recherche métallurgique";
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sName = $"Travail artisanal en pause - Recherche en efficacité";
          break;
        case JobType.Enchantement:
          foreach (API.Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);
          journalEntry.sText = $"Enchantement en pause";
          break;
        case JobType.Recycling:
          journalEntry.sText = $"Recyclage en pause";
          break;
        case JobType.Renforcement:
          journalEntry.sText = $"Renforcement en pause";
          break;
        case JobType.Repair:
          journalEntry.sText = $"Réparations en pause";
          break;
        case JobType.EnchantementReactivation:
          foreach (API.Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);
          journalEntry.sText = $"Ré-enchantement en pause";
          break;
        default:
          journalEntry.sName = $"Travail artisanal en pause - {blueprintDictionnary[baseItemType].name}";
          break;
      }

      journalEntry.sTag = "craft_job";
      journalEntry.nQuestDisplayed = 0;
      PlayerPlugin.AddCustomJournalEntry(player.oid.LoginCreature, journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
    public void CloseCraftJournalEntry()
    {
      JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid.LoginCreature, "craft_job");

      switch (type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sName = $"Travail artisanal terminé - Copie de patron";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 631);
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sName = $"Travail artisanal terminé - Recherche métallurgique";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 792);
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sName = $"Travail artisanal terminé - Recherche en efficacité";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 792);
          break;
        case JobType.Enchantement:
          foreach (API.Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);

          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1055);

          journalEntry.sName = $"Enchantement terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          break;
        case JobType.Recycling:
          journalEntry.sName = $"Recyclage terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 818);
          break;
        case JobType.Renforcement:
          journalEntry.sName = $"Renforcement terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 829);
          break;
        case JobType.Repair:
          journalEntry.sName = $"Réparations terminées - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1501);
          break;
        case JobType.EnchantementReactivation:
          foreach (API.Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);

          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 872);

          journalEntry.sName = $"Ré-enchantement terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          break;
        default:
          journalEntry.sName = $"Travail artisanal terminé - {blueprintDictionnary[baseItemType].name}";

          PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1501);
          break;
      }

      journalEntry.sTag = "craft_job";
      journalEntry.nQuestCompleted = 1;
      journalEntry.nQuestDisplayed = 0;
      PlayerPlugin.AddCustomJournalEntry(player.oid.LoginCreature, journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
  }
}
