using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static readonly Dictionary<JobType, Func<Player, bool, bool>> HandleSpecificJobCompletion = new()
    {
      { JobType.ItemCreation, CompleteItemCreation },
      { JobType.ItemUpgrade, CompleteItemUpgrade },
      { JobType.Renforcement, CompleteItemReinforcement },
      { JobType.Recycling, CompleteItemRecycling },
      { JobType.Repair, CompleteItemRepair },
      { JobType.BlueprintCopy, CompleteBlueprintCopy },
      { JobType.BlueprintResearchMaterialEfficiency, CompleteBlueprintMaterialResearch },
      { JobType.BlueprintResearchTimeEfficiency, CompleteBlueprintTimeResearch },
      { JobType.Enchantement, CompleteItemEnchantement },
      { JobType.Mining, CompleteMining },
      { JobType.WoodCutting, CompleteWoodCutting },
      { JobType.Pelting, CompletePelting },
    };

    public enum JobType
    {
      [Description("Invalide")]
      Invalid = 0,
      [Description("Création_artisanale")]
      ItemCreation = 1,
      [Description("Amélioration_artisanale")]
      ItemUpgrade = 2,
      [Description("Copie_de_patron")]
      BlueprintCopy = 3,
      [Description("Recherche_en_rendement")]
      BlueprintResearchMaterialEfficiency = 4,
      [Description("Recherche_en_efficacité")]
      BlueprintResearchTimeEfficiency = 5,
      [Description("Enchantement")]
      Enchantement = 6,
      [Description("Recyclage")]
      Recycling = 7,
      [Description("Renforcement")]
      Renforcement = 8,
      [Description("Réparations")]
      Repair = 9,
      [Description("Réactivation_d'_enchantement")]
      EnchantementReactivation = 10,
      [Description("Alchimie")]
      Alchemy = 11,
      [Description("Extraction_minérale_passive")]
      Mining = 12,
      [Description("Extraction_arboricole_passive")]
      WoodCutting = 13,
      [Description("Extraction_animale_passive")]
      Pelting = 14,
    }

    public class CraftJob
    {
      public readonly JobType type;
      public readonly string icon;
      public double remainingTime { get; set; }
      public string originalSerializedItem { get; set; }
      public string serializedCraftedItem { get; set; }
      public string enchantementTag { get; set; }
      public DateTime? progressLastCalculation { get; set; }
      public DateTime? startTime { get; set; }

      public CraftJob(Player player, NwItem oBlueprint, double jobDuration, NwItem tool) // Item Craft
      {
        try
        {
          // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
          if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").HasValue)
          {
            oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value -= 1;

            if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value < 1)
              oBlueprint.Destroy();
          }

          int baseItemType = oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;

          try
          {
            icon = NwBaseItem.FromItemId(baseItemType).WeaponFocusFeat.IconResRef;
          }
          catch(Exception)
          {
            icon = "clockwork";
          }

          remainingTime = jobDuration;
          type = JobType.ItemCreation;
          NwItem craftedItem = (BaseItemType)baseItemType == BaseItemType.Armor ? NwItem.Create(Armor2da.armorTable[oBlueprint.GetObjectVariable<LocalVariableInt>("_ARMOR_BASE_AC").Value].craftResRef, player.oid.LoginCreature.Location)
            : NwItem.Create(BaseItems2da.baseItemTable[baseItemType].craftedItem, player.oid.LoginCreature.Location);

          Craft.Collect.System.AddCraftedItemProperties(craftedItem, 1);
          craftedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = player.oid.LoginCreature.OriginalName;

          if (NwRandom.Roll(Utils.random, 100) <= tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_QUALITY_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value))
            craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;

          serializedCraftedItem = craftedItem.Serialize().ToBase64EncodedString();
          craftedItem.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem oBlueprint, double jobDuration, NwItem upgradedItem, NwItem tool) // Item Upgrade
      {
        try
        {
          // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
          if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").HasValue)
          {
            oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value -= 1;

            if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value < 1)
              oBlueprint.Destroy();
          }

          icon = upgradedItem.BaseItem.WeaponFocusFeat.IconResRef;
          remainingTime = jobDuration;
          type = JobType.ItemUpgrade;

          if (NwRandom.Roll(Utils.random, 100) <= tool.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_CRAFT_QUALITY_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value))
            upgradedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;

          originalSerializedItem = upgradedItem.Serialize().ToBase64EncodedString();

          Craft.Collect.System.AddCraftedItemProperties(upgradedItem, upgradedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value + 1);
          upgradedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = player.oid.LoginCreature.OriginalName;

          DelayItemSerialization(upgradedItem);
          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      private async void DelayItemSerialization(NwItem upgradedItem) // La suppression d'item property ne s'exécute qu'après la fin du script en cours. Je suis donc obligé de mettre un faux délai avant serialization
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0));
        serializedCraftedItem = upgradedItem.Serialize().ToBase64EncodedString();
        upgradedItem.Destroy();
      }
      public CraftJob(Player player, NwItem oBlueprint, NwItem tool, JobType type) // Item Renforcement & Recycling
      {
        try
        {
          this.type = type;

          switch (type)
          {
            case JobType.Renforcement:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).EpicWeaponFocusFeat.IconResRef;
              StartRenforcement(player, oBlueprint, tool);
              break;
            case JobType.Recycling:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).EpicWeaponFocusFeat.IconResRef;
              StartRecycling(player, oBlueprint, tool);
              break;
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem oBlueprint, JobType type) // Blueprint Copy
      {
        try
        {
          this.type = type;

          switch (type)
          {
            case JobType.BlueprintCopy:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).WeaponFocusFeat.IconResRef;
              StartBlueprintCopy(player, oBlueprint);
              break;
            case JobType.BlueprintResearchMaterialEfficiency:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).WeaponSpecializationFeat.IconResRef;
              StartBlueprintMaterialResearch(player, oBlueprint);
              break;
            case JobType.BlueprintResearchTimeEfficiency:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).EpicWeaponFocusFeat.IconResRef;
              StartBlueprintTimeResearch(player, oBlueprint);
              break;
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem item, double jobDuration, JobType jobType) // Repair
      {
        try
        {
          icon = item.BaseItem.WeaponFocusFeat.IconResRef;
          remainingTime = jobDuration;
          type = jobType;

          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          NwItem repairedItem = NwItem.Create(BaseItems2da.baseItemTable[(int)item.BaseItem.ItemType].craftedItem, player.oid.LoginCreature.Location);

          if (player.learnableSkills.ContainsKey(CustomSkill.RepairCareful))
            repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = (int)(repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * (0.95 + player.learnableSkills[CustomSkill.RepairCareful].totalPoints / 100));

          repairedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value;
          repairedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").Value += 1;

          serializedCraftedItem = repairedItem.Serialize().ToBase64EncodedString();

          item.Destroy();
          repairedItem.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem item, NwSpell spell, JobType jobType) // Enchantement NEW => Inscription
      {
        try
        {
          icon = spell.IconResRef;
          type = jobType;

          Learnable learnable = SkillSystem.learnableDictionary[Spells2da.spellTable.GetRow(spell.Id).inscriptionSkill];
          remainingTime = GetInscriptionTimeCost(learnable, player, item);
          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          NwItem enchantedItem = item.Clone(NwModule.Instance.StartingLocation);

          enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
          if (enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
            enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();

          int i = 0;

          while (enchantedItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasValue && i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value)
            i++;

          if (i >= item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value)
            LogUtils.LogMessage($"Inscription {player.oid.LoginCreature.Name} - {spell.Name.ToString()} - {item.Name} - Impossible de trouver un slot valide libre", LogUtils.LogType.Craft);

          enchantedItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value = spell.Id;

          HandleDamageModifierInscription(enchantedItem, learnable.id);

          item.Destroy();
          DelayItemSerialization(enchantedItem);
          player.oid.ApplyInstantVisualEffectToObject((VfxType)832, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem item, NwSpell spell, int index, JobType jobType) // Enchantement OLD
      {
        try
        {
          icon = spell.IconResRef;
          type = jobType;

          remainingTime = ItemUtils.GetBaseItemCost(item) * 10 * spell.InnateSpellLevel;

          //if (player.learnableSkills.ContainsKey(CustomSkill.Enchanteur))
            //remainingTime *= player.learnableSkills[CustomSkill.Enchanteur].bonusReduction;

          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          NwItem enchantedItem = item.Clone(player.oid.LoginCreature.Location);

          enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
          if (enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
            enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();

          int i = 0;

          while (enchantedItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasValue && i < 20)
            i++;

          if (i > 19)
            Utils.LogMessageToDMs($"SYSTEME ENCHANTEMENT - {player.oid.LoginCreature.Name} - {spell.Name.ToString()} - {item.Name} - Impossible de trouver un slot valide libre");

          enchantedItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value = spell.Id;
          enchantementTag = AddCraftedEnchantementProperties(enchantedItem, spell, index, player, i);

          item.Destroy();
          DelayItemSerialization(enchantedItem);
          player.oid.ApplyInstantVisualEffectToObject((VfxType)832, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, ResourceType resourceType, double consumedTime, string icon) // Passive repeatable mining
      {
        try
        {
          type = resourceType switch
          {
            ResourceType.Plank => JobType.WoodCutting,
            ResourceType.Leather => JobType.Pelting,
            _ => JobType.Mining,
          };
          remainingTime = 3600;
          startTime = DateTime.Now.AddSeconds(-consumedTime);
          this.icon = icon;

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(SerializableCraftJob serializedJob)
      {
        type = serializedJob.type;
        icon = serializedJob.icon;
        remainingTime = serializedJob.remainingTime;
        originalSerializedItem = serializedJob.originalSerializedItem;
        serializedCraftedItem = serializedJob.serializedItem;
        enchantementTag = serializedJob.enchantementTag;
        progressLastCalculation = serializedJob.progressLastCalculation;
        startTime = serializedJob.startTime;

        if (progressLastCalculation.HasValue)
        {
          remainingTime -= (DateTime.Now - progressLastCalculation.Value).TotalSeconds;
          progressLastCalculation = null;
        }
      }

      public class SerializableCraftJob
      {
        public JobType type;
        public double remainingTime { get; set; }
        public string originalSerializedItem { get; set; }
        public string serializedItem { get; set; }
        public string enchantementTag { get; set; }
        public string icon { get; set; }
        public DateTime? progressLastCalculation { get; set; }
        public DateTime? startTime { get; set; }

        public SerializableCraftJob()
        {

        }
        public SerializableCraftJob(CraftJob baseJob)
        {
          type = baseJob.type;
          icon = baseJob.icon;
          remainingTime = baseJob.remainingTime;
          originalSerializedItem = baseJob.originalSerializedItem;
          serializedItem = baseJob.serializedCraftedItem;
          baseJob.progressLastCalculation = DateTime.Now;
          progressLastCalculation = DateTime.Now;
          enchantementTag = baseJob.enchantementTag;
          startTime = baseJob.startTime;
        }
      }
      public void HandleGenericJobCompletion(Player player)
      {
        if (player.TryGetOpenedWindow("activeCraftJob", out Player.PlayerWindow craftWindow))
          if (player.craftJob.type != JobType.Mining && player.craftJob.type != JobType.WoodCutting && player.craftJob.type != JobType.Pelting)
            craftWindow.CloseWindow();

        player.craftJob = null;
        player.oid.PlaySound("gui_level_up");
        player.oid.ExportCharacter();
      }
      public void HandleCraftJobCancellation(Player player)
      {
        if (type != JobType.Invalid)
          HandleSpecificJobCompletion[type].Invoke(player, false);

        if (player.TryGetOpenedWindow("activeCraftJob", out Player.PlayerWindow craftWindow))
          craftWindow.CloseWindow();

        player.craftJob = null;
        player.oid.ExportCharacter();
      }
      public string GetReadableJobCompletionTime()
      {
        TimeSpan timespan = TimeSpan.FromSeconds(remainingTime);
        return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
      }
      private void StartBlueprintCopy(Player player, NwItem oBlueprint)
      {
        remainingTime = player.GetItemMateriaCost(oBlueprint) * 2 * player.learnableSkills[CustomSkill.BlueprintCopy].bonusReduction;

        originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

        NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
        clone.Name = oBlueprint.Name.Replace("original", "copié");
        clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value = player.learnableSkills.ContainsKey(CustomSkill.BlueprintEfficiency) ? 10 + player.learnableSkills[CustomSkill.BlueprintEfficiency].totalPoints : 10;

        serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
        oBlueprint.Destroy();
        clone.Destroy();

        player.oid.ApplyInstantVisualEffectToObject((VfxType)631, player.oid.ControlledCreature);
      }
      private void StartBlueprintMaterialResearch(Player player, NwItem oBlueprint)
      {
        remainingTime = player.GetItemMateriaCost(oBlueprint) * 2 * (1 - (player.learnableSkills[CustomSkill.BlueprintMetallurgy].totalPoints * 5 / 100));

        if (player.learnableSkills.ContainsKey(CustomSkill.AdvancedCraft))
          remainingTime *= (1 - (player.learnableSkills[CustomSkill.AdvancedCraft].totalPoints * 3 / 100));

        originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

        NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
        clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value += 1;

        serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
        oBlueprint.Destroy();
        clone.Destroy();

        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      private void StartBlueprintTimeResearch(Player player, NwItem oBlueprint)
      {
        remainingTime = player.GetItemMateriaCost(oBlueprint) * 2 * (1 - (player.learnableSkills[CustomSkill.BlueprintResearch].totalPoints * 5 / 100));

        if (player.learnableSkills.ContainsKey(CustomSkill.AdvancedCraft))
          remainingTime *= (1 - (player.learnableSkills[CustomSkill.AdvancedCraft].totalPoints * 3 / 100));

        originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

        NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
        clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value += 1;

        serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
        oBlueprint.Destroy();
        clone.Destroy();

        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      private void StartRenforcement(Player player, NwItem item, NwItem tool)
      {
        try
        {
          remainingTime = player.GetItemReinforcementTime(item, tool);
          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value += item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 5 / 100;
          item.GetObjectVariable<LocalVariableInt>("_REINFORCEMENT_LEVEL").Value += 1;

          serializedCraftedItem = item.Serialize().ToBase64EncodedString();
          item.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      private void StartRecycling(Player player, NwItem item, NwItem tool)
      {
        try
        {
          remainingTime = player.GetItemRecycleTime(item, tool);

          originalSerializedItem = item.Serialize().ToBase64EncodedString();
          item.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)818, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
    }
    private static bool CompleteBlueprintCopy(Player player, bool completed)
    {
      if (string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
      {
        player.oid.SendServerMessage("Il semble y avoir eu une erreur. Votre travail artisanal a été annulé.");
        player.craftJob = null;
        return true;
      }

      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)631, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteBlueprintMaterialResearch(Player player, bool completed)
    {
      if (string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
      {
        player.oid.SendServerMessage("Il semble y avoir eu une erreur. Votre travail artisanal a été annulé.");
        player.craftJob = null;
        return true;
      }

      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteBlueprintTimeResearch(Player player, bool completed)
    {
      if (string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
      {
        player.oid.SendServerMessage("Il semble y avoir eu une erreur. Votre travail artisanal a été annulé.");
        player.craftJob = null;
        return true;
      }

      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemCreation(Player player, bool completed)
    {
      if (completed)
      {
        if(player.craftJob.serializedCraftedItem == null)
        {
          player.oid.SendServerMessage("Erreur technique - Votre objet n'a pas pu être créé. Veuillez contacter le staff et leur fournir les renseignements qui pourront les aider à déterminer l'origine du problème.", ColorConstants.Red);
          Utils.LogMessageToDMs($"CRAFT ERROR - {player.craftJob.type} - {player.oid.LoginCreature.Name}");
          return true;
        }

        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer la création de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);

        int artisanExceptionnelLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanExceptionnel) ? player.learnableSkills[CustomSkill.ArtisanExceptionnel].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
        {
          item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          player.oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", ColorConstants.Cyan);
        }

        int artisanAppliqueLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanApplique) ? player.learnableSkills[CustomSkill.ArtisanApplique].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
        {
          int artisanFocusLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanFocus) ? 5 * (player.learnableSkills[CustomSkill.ArtisanFocus].totalPoints + 1) : 5;

          item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value *= (1 + artisanFocusLevel / 100);
          player.oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", ColorConstants.Cyan);
        }
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler la création d'un objet artisanal.", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemUpgrade(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer l'amélioration de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);

        int artisanExceptionnelLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanExceptionnel) ? player.learnableSkills[CustomSkill.ArtisanExceptionnel].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
        {
          item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          player.oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", ColorConstants.Cyan);
        }

        int artisanAppliqueLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanApplique) ? player.learnableSkills[CustomSkill.ArtisanApplique].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
        {
          item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value *= (1 + 20 / 100);
          player.oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", ColorConstants.Cyan);
        }
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'anuller l'amélioration de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemReinforcement(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le renforcement de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le renforcement de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemRecycling(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = NwItem.Deserialize(player.craftJob.originalSerializedItem.ToByteArray());

        ResourceType resourceType = ItemUtils.GetResourceTypeFromItem(item);
        int grade = item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue ? item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value : 1;
        double quantity = player.GetItemRecycleGain(item);

        CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == resourceType && r.grade == grade);

        if (resource != null)
          resource.quantity += (int)quantity;
        else
          player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == resourceType && r.grade == grade), (int)quantity));

        player.oid.SendServerMessage($"Le recyclage de : {item.Name.ColorString(ColorConstants.White)} vous rapporte {quantity.ToString().ColorString(ColorConstants.White)} unités de matéria de qualité {((int)grade).ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)818, player.oid.ControlledCreature);

        item.Destroy();
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le recyclage de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemRepair(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer la réparation de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler la réparation de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemEnchantement(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer l'enchantement de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1055, player.oid.ControlledCreature);

        /*int enchanteurChanceuxLevel = player.learnableSkills.ContainsKey(CustomSkill.EnchanteurChanceux) ? player.learnableSkills[CustomSkill.EnchanteurChanceux].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= enchanteurChanceuxLevel)
        {
          item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          player.oid.SendServerMessage("Votre talent d'enchanteur vous a permit de ne pas consommer d'emplacement !", ColorConstants.Orange);
        }

        int enchanteurExpertLevel = player.learnableSkills.ContainsKey(CustomSkill.EnchanteurExpert) ? player.learnableSkills[CustomSkill.EnchanteurExpert].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= enchanteurExpertLevel * 2)
        {
          if(player.craftJob.enchantementTag.StartsWith("ENCHANTEMENT_CUSTOM_"))
          {
            item.GetObjectVariable<LocalVariableInt>(player.craftJob.enchantementTag).Value *= 15 / 10;
            item.GetObjectVariable<LocalVariableInt>($"{player.craftJob.enchantementTag}_DURABILITY").Value *= 15 / 10;
          }
          else
          {
            ItemProperty oldIp = item.ItemProperties.FirstOrDefault(ip => ip.Tag == player.craftJob.enchantementTag);
            item.RemoveItemProperty(oldIp);

            oldIp.IntParams[3] += 1; // IntParams[3] = CostTableValue
            item.AddItemProperty(oldIp, EffectDuration.Permanent);
          }
          
          player.oid.SendServerMessage("Votre talent d'enchanteur vous a permis d'obtenir un effet plus puissant !", ColorConstants.Orange);
        }*/
      }
      else // cancelled
      {
        ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler la création d'un objet artisanal.", ColorConstants.Orange);
      }

      return true;
    }
    private static string AddCraftedEnchantementProperties(NwItem craftedItem, NwSpell spell, int index, Player enchanter, int slot)
    {
      return spell.Id switch
      {
        883 or 884 or 885 or 886 or 887 or 888 or 889 or 889 or 890 or 891 or 892 or 893 => GetCraftToolEnchantementProperties(craftedItem, spell, index, slot),
        _ => GetCraftEnchantementProperties(craftedItem, spell, SpellUtils.enchantementCategories[spell.Id][index], enchanter.characterId),
      };
    }
    private static string GetCraftEnchantementProperties(NwItem craftedItem, NwSpell spell, ItemProperty ip, int enchanterId)
    {
      ItemProperty existingIP = craftedItem.ItemProperties.FirstOrDefault(i => i.DurationType == EffectDuration.Permanent && i.Property.RowIndex == ip.Property.RowIndex && i.SubType?.RowIndex == ip.SubType?.RowIndex && i.Param1TableValue?.RowIndex == ip.Param1TableValue?.RowIndex);

      if (existingIP != null)
      {
        craftedItem.RemoveItemProperty(existingIP);

        if (ip.Property.PropertyType == ItemPropertyType.DamageBonus
          || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup
          || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
          || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment)
        {
          int newRank = ItemPropertyDamageCost2da.GetRankFromCostValue(ip.IntParams[3]); // IntParams[3] = CostTableValue
          int existingRank = ItemPropertyDamageCost2da.GetRankFromCostValue(existingIP.IntParams[3]); // IntParams[3] = CostTableValue

          if (existingRank > newRank)
            newRank = existingRank + 1;
          else
            newRank += 1;

          ip.IntParams[3] = ItemPropertyDamageCost2da.GetDamageCostValueFromRank(newRank); // IntParams[3] = CostTableValue
        }
        else if (ip.Property.PropertyType == ItemPropertyType.AcBonus
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsAlignmentGroup
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsDamageType
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsRacialGroup
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsSpecificAlignment
          || ip.Property.PropertyType == ItemPropertyType.AttackBonus
          || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup
          || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup
          || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment)
        {
          ip.IntParams[3] += existingIP.IntParams[3];
        }
        else
        {
          if (existingIP.IntParams[3] > ip.IntParams[3])
            ip.IntParams[3] = existingIP.IntParams[3] + 1;
          else
            ip.IntParams[3] += 1;
        }
      }

      ip.Tag = $"ENCHANTEMENT_{spell.Id}_{ip.Property.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}_{enchanterId}";
      craftedItem.AddItemProperty(ip, EffectDuration.Permanent);

      return ip.Tag;
    }
    private static string GetCraftToolEnchantementProperties(NwItem craftedItem, NwSpell spell, int index, int slot)
    {
      var enchantement = spell.Id switch
      {
        887 => "DETECTOR",
        888 => "DETECTOR",
        889 => "DETECTOR",
        890 => "DETECTOR",
        891 => "CRAFT",
        892 => "CRAFT",
        893 => "CRAFT",
        894 => "CRAFT",
        _ => "EXTRACTOR",
      };
    
    var type = index switch
      {
        0 => "YIELD",
        1 => "SPEED",
        2 => "QUALITY",
        4 => "ACCURACY",
        _ => "RESIST",
      };

      int existingEnchantement = 1 + craftedItem.LocalVariables.Count(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_{enchantement}_{type}_") && !l.Name.Contains("_DURABILITY"));
      
      craftedItem.GetObjectVariable<LocalVariableInt>($"ENCHANTEMENT_CUSTOM_{enchantement}_{type}_{existingEnchantement}_{slot}").Value = 2 * spell.InnateSpellLevel /** enchanter.learnableSpells[spell.Id].currentLevel*/; // TODO : Prendre en compte le niveau des sorts;
      craftedItem.GetObjectVariable<LocalVariableInt>($"ENCHANTEMENT_CUSTOM_{enchantement}_{type}_{existingEnchantement}_{slot}_DURABILITY").Value = craftedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value * Config.baseCraftToolDurability;
      
      if (!craftedItem.ItemProperties.Any(ip => ip.Property.PropertyType == ItemPropertyType.CastSpell && ip.SubType.RowIndex == 329)) // 329 = Activate Item
        craftedItem.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)329, IPCastSpellNumUses.UnlimitedUse), EffectDuration.Permanent);

      return $"ENCHANTEMENT_CUSTOM_{enchantement}_{type}_{existingEnchantement}_{slot}";
    }
    private static bool CompleteMining(Player player, bool completed)
    {
      if (completed)
      {
        double elapsedTime = (DateTime.Now - player.craftJob.startTime.Value).TotalSeconds;

        if (elapsedTime < 3600)
          elapsedTime = 3600;

        int nbCycles = (int)Math.Truncate(elapsedTime / 3600);
        double alreadyConsumedTime = elapsedTime - (nbCycles * 3600);
        double miningYield = nbCycles * 900;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.OreExtraction) ? miningYield * player.learnableSkills[CustomSkill.OreExtraction].bonusMultiplier : miningYield;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.OreExtractionYield) ? miningYield * player.learnableSkills[CustomSkill.OreExtractionYield].bonusMultiplier : miningYield;

        CraftResource resourceStock = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Ore && r.grade == 1);

        if (resourceStock != null)
          resourceStock.quantity += (int)miningYield;
        else
        {
          CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Ore && r.grade == 1);
          player.craftResourceStock.Add(new CraftResource(resource, (int)miningYield));
        }

        player.RescheduleRepeatableJob(ResourceType.Ore, elapsedTime, player.craftJob.icon);
        player.oid.SendServerMessage($"La fin de votre job d'extraction passive de matéria minérale vous rapporte : {(int)miningYield} unités de matéria !", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler votre job de récolte passive de matéria minérale.", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteWoodCutting(Player player, bool completed)
    {
      if (completed)
      {
        double elapsedTime = (DateTime.Now - player.craftJob.startTime.Value).TotalSeconds;

        if (elapsedTime < 3600)
          elapsedTime = 3600;

        int nbCycles = (int)Math.Truncate(elapsedTime / 3600);
        double alreadyConsumedTime = elapsedTime - (nbCycles * 3600);
        double miningYield = nbCycles * 900;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.WoodExtraction) ? miningYield * player.learnableSkills[CustomSkill.WoodExtraction].bonusMultiplier : miningYield;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.WoodExtractionYield) ? miningYield * player.learnableSkills[CustomSkill.WoodExtractionYield].bonusMultiplier : miningYield;

        CraftResource resourceStock = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);

        if (resourceStock != null)
          resourceStock.quantity += (int)miningYield;
        else
        {
          CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);
          player.craftResourceStock.Add(new CraftResource(resource, (int)miningYield));
        }

        player.RescheduleRepeatableJob(ResourceType.Ore, alreadyConsumedTime, player.craftJob.icon);
        player.oid.SendServerMessage($"La fin de votre job d'extraction passive de matéria arboricole vous rapporte : {(int)miningYield} unités de matéria, directement envoyées à l'entrepôt !", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler votre job de récolte passive de matéria arboricole.", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompletePelting(Player player, bool completed)
    {
      if (completed)
      {
        double elapsedTime = (DateTime.Now - player.craftJob.startTime.Value).TotalSeconds;

        if (elapsedTime < 3600)
          elapsedTime = 3600;

        int nbCycles = (int)Math.Truncate(elapsedTime / 3600);
        double alreadyConsumedTime = elapsedTime - (nbCycles * 3600);
        double miningYield = nbCycles * 900;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.PeltExtraction) ? miningYield * player.learnableSkills[CustomSkill.PeltExtraction].bonusMultiplier : miningYield;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.PeltExtractionYield) ? miningYield * player.learnableSkills[CustomSkill.PeltExtractionYield].bonusMultiplier : miningYield;

        CraftResource resourceStock = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);

        if (resourceStock != null)
          resourceStock.quantity += (int)miningYield;
        else
        {
          CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);
          player.craftResourceStock.Add(new CraftResource(resource, (int)miningYield));
        }

        player.RescheduleRepeatableJob(ResourceType.Ore, alreadyConsumedTime, player.craftJob.icon);
        player.oid.SendServerMessage($"La fin de votre job d'extraction passive de matéria animale vous rapporte : {(int)miningYield} unités de matéria, directement envoyées à l'entrepôt !", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler votre job de récolte passive de matéria animale, directement envoyées à l'entrepôt !", ColorConstants.Orange);
      }

      return true;
    }
    private static void HandleDamageModifierInscription(NwItem newItem, int learnableId)
    {
      if(learnableId == CustomInscription.Polaire || learnableId == CustomInscription.Sismique || learnableId == CustomInscription.Incendiaire || learnableId == CustomInscription.Electrocution)
      {
        for (int i = 0; i < newItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          switch(newItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
          {
            case CustomInscription.Polaire:
            case CustomInscription.Sismique:
            case CustomInscription.Incendiaire:
            case CustomInscription.Electrocution:
              newItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Delete();
              newItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
              break;
          }
      }
    }
    private static double GetInscriptionTimeCost(Learnable learnable, Player player, NwItem item)
    {
      double remainingTime = ItemUtils.GetBaseItemCost(item) * 10 * learnable.multiplier;

      return item.BaseItem.ItemType switch
      {
        BaseItemType.SmallShield or BaseItemType.LargeShield or BaseItemType.TowerShield => remainingTime * player.GetShieldInscriptionSkillScore(),
        BaseItemType.Armor or BaseItemType.Helmet or BaseItemType.Cloak or BaseItemType.Boots or BaseItemType.Gloves or BaseItemType.Bracer or BaseItemType.Belt => remainingTime * player.GetArmorInscriptionSkillScore(),
        BaseItemType.Amulet or BaseItemType.Ring => remainingTime * player.GetOrnamentInscriptionSkillScore(),
        _ => remainingTime * player.GetWeaponInscriptionSkillScore(),
      };
    }
  }
}
