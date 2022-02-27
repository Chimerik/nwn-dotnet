using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;

using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Collect.System;

namespace NWN.Systems.Craft
{
  public class Job
  {
    public readonly JobType type;
    public bool active { get; set; }
    public int baseItemType { get; }
    public string craftedItem { get; set; }
    public double remainingTime { get; set; }
    public string material { get; set; }
    public bool isCancelled { get; set; }
    private readonly PlayerSystem.Player player;

    public Job(int baseItemType, string material, double time, PlayerSystem.Player player, string item = "")
    {
      PlayerSystem.Log.Info($"Craft new job from {player.oid.LoginCreature.Name} - base item : {baseItemType} - remaining Time {time} - material {material}");
      this.baseItemType = baseItemType;
      this.craftedItem = item;
      remainingTime = time;
      this.material = material;
      this.isCancelled = false;
      this.player = player;
      this.active = true;

      switch (baseItemType)
      {
        case -14:
          this.type = JobType.Enchantement;
          break;
        case -17:
          this.type = JobType.Repair;
          break;
        case -18:
          this.type = JobType.EnchantementReactivation;
          break;
        case -19:
          this.type = JobType.Alchemy;
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
      Enchantement = 5,
      Recycling = 6,
      Repair = 8,
      EnchantementReactivation = 9,
      Alchemy = 10,
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
      player.SendServerMessage($"Attention, votre travail précédent n'est pas terminé. Lancer un nouveau travail signifie perdre la totalité du travail en cours !", ColorConstants.Magenta);
      player.SendServerMessage($"Utilisez une seconde fois le plan pour confirmer l'annulation du travail en cours.", ColorConstants.Silver);
      this.isCancelled = true;

      Task waitSpellUsed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(60));
        ResetCancellation();
      });
    }
    public bool CanStartJob(NwPlayer player, NwItem blueprint, JobType type)
    {
      if (IsActive() && !isCancelled)
      {
        AskCancellationConfirmation(player);
        return false;
      }

      if (this.isCancelled)
      {
        if (craftedItem != "")
          ItemUtils.DeserializeAndAcquireItem(craftedItem, player.LoginCreature);
      }

      return true;
    }
    public void Start(JobType type, PlayerSystem.Player player, NwItem oItem, NwGameObject oTarget = null, string sMaterial = "")
    {
      switch (type)
      {
        case JobType.Enchantement:
          StartEnchantementCraft(oTarget, sMaterial);
          break;
        case JobType.Repair:
          StartItemRepair(oItem, oTarget, sMaterial);
          break;
        case JobType.EnchantementReactivation:
          StartEnchantementReactivationCraft(oTarget, sMaterial);
          break;
        case JobType.Alchemy:
          StartAlchemyCraft(oTarget, sMaterial);
          break;
      }

    }
    public void StartItemRepair(NwItem oItem, NwGameObject oTarget, string sMaterial)
    {
      player.craftJob.isCancelled = false;
      int iMineralCost = (int)player.GetItemMateriaCost(oItem);
      double iJobDuration = player.GetItemCraftTime(oItem, iMineralCost);

      if (oTarget.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value == player.oid.LoginCreature.Name)
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
          player.oid.SendServerMessage($"Il vous manque {materials.Value.ToString().ColorString(ColorConstants.White)} unités de {materials.Key.ColorString(ColorConstants.White)} pour réaliser ce travail artisanal.", ColorConstants.Red);
          return;
        }
        else if (player.materialStock[materials.Key] < materials.Value)
        {
          player.oid.SendServerMessage($"Il vous manque {(materials.Value - player.materialStock[materials.Key]).ToString().ColorString(ColorConstants.White)} unités de {materials.Key.ColorString(ColorConstants.White)} pour réaliser ce travail artisanal.", ColorConstants.Red);
          return;
        }
      }

      foreach (KeyValuePair<string, int> materials in repairMaterials)
        player.materialStock[materials.Key] -= materials.Value;

      //player.oid.SendServerMessage($"Vous venez de démarrer la réparation de l'objet artisanal : {blueprint.name.ColorString(ColorConstants.White)} en {sMaterial.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels sur la forge

      player.craftJob = new Job(-17, sMaterial, iJobDuration, player, oTarget.Serialize().ToBase64EncodedString());
      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(ColorConstants.White)} ne sera pas disponible jusqu'à la fin du travail artisanal.", ColorConstants.Orange);

      // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
      int iBlueprintRemainingRuns = oItem.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value;
      if (iBlueprintRemainingRuns == 1)
        oItem.Destroy();
      if (iBlueprintRemainingRuns > 0)
        oItem.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value -= 1;

      ItemUtils.DecreaseItemDurability(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand));
    }
    private void StartEnchantementCraft(NwGameObject oTarget, string ipString)
    {
      Spell spellId = (Spell)int.Parse(ipString.Remove(ipString.IndexOf("_")));
      int baseCost = ItemUtils.GetBaseItemCost((NwItem)oTarget);
      int enchanteurLevel = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Enchanteur))
        enchanteurLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Enchanteur, player.learntCustomFeats[CustomFeats.Enchanteur]);

      float iJobDuration = baseCost * 10 * NwSpell.FromSpellType(spellId).InnateSpellLevel * (100 - enchanteurLevel) / 100;
      player.craftJob = new Job(-14, ipString, iJobDuration, player, oTarget.Serialize().ToBase64EncodedString()); // -14 = JobType enchantement

      player.oid.SendServerMessage($"Vous venez de démarrer l'enchantement de : {oTarget.Name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels

      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(ColorConstants.White)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.", ColorConstants.Orange);

      player.craftJob.isCancelled = false;
    }
    private void StartEnchantementReactivationCraft(NwGameObject oTarget, string ipString)
    {
      string[] IPproperties = ipString.Split("_");
      Spell spellId = (Spell)int.Parse(IPproperties[0]);
      int costValue = int.Parse(IPproperties[1]);
      int originalEnchanterId = int.Parse(IPproperties[2]);
      int baseCost = ItemUtils.GetBaseItemCost((NwItem)oTarget);
      int enchanteurLevel = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Enchanteur))
        enchanteurLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Enchanteur, player.learntCustomFeats[CustomFeats.Enchanteur]);

      float iJobDuration = baseCost * 10 * NwSpell.FromSpellType(spellId).InnateSpellLevel * (100 - enchanteurLevel) * costValue;
      if (player.characterId == originalEnchanterId)
        iJobDuration -= iJobDuration / 4;

      player.craftJob = new Job(-18, spellId.ToString(), iJobDuration, player, oTarget.Serialize().ToBase64EncodedString()); // -18 = JobType enchantementReactivation

      player.oid.SendServerMessage($"Vous venez de démarrer la réactivation d'enchantement de : {oTarget.Name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
      // TODO : afficher des effets visuels

      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(ColorConstants.White)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.", ColorConstants.Orange);

      player.craftJob.isCancelled = false;
    }
    private void StartAlchemyCraft(NwGameObject oTarget, string ipString)
    {
      int alchemistLevel = 0;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.Alchemist))
        alchemistLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Alchemist, player.learntCustomFeats[CustomFeats.Alchemist]);

      float iJobDuration = player.alchemyCauldron.nBrowsedCases * 60 * (100 - 2 * alchemistLevel) / 100;
      player.craftJob = new Job(-19, ipString, iJobDuration, player, oTarget.Serialize().ToBase64EncodedString()); // -19 = JobType Alchemy

      player.oid.SendServerMessage($"Vous venez de démarrer la création d'une potion !", new Color(32, 255, 32));

      oTarget.Destroy();
      player.craftJob.isCancelled = false;
    }
    public void CreateCraftJournalEntry()
    {
      if (!IsActive())
        return;

      player.playerJournal.craftJobCountDown = DateTime.Now.AddSeconds(remainingTime);

      JournalEntry journalEntry = new JournalEntry();
      if(remainingTime > 0)
        journalEntry.Name = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.craftJobCountDown - DateTime.Now))}";
      else
        journalEntry.Name = $"Travail artisanal - Terminé !";

      switch (this.type)
      {
        case JobType.Enchantement:
          journalEntry.Text = $"Enchantement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          Effect enchantementVfx = Effect.VisualEffect((VfxType)832);
          enchantementVfx.Tag = "VFX_ENCHANTEMENT";
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, enchantementVfx);
          break;
        case JobType.Recycling:
          journalEntry.Text = $"Recyclage en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          player.oid.ApplyInstantVisualEffectToObject((VfxType)818, player.oid.ControlledCreature);
          break;
        case JobType.Repair:
          journalEntry.Text = $"Réparation en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
          break;
        case JobType.EnchantementReactivation:
          journalEntry.Text = $"Enchantement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          Effect reactivationVfx = Effect.VisualEffect((VfxType)832);
          reactivationVfx.Tag = "VFX_ENCHANTEMENT";
          journalEntry.Text = $"Ré-enchantement en cours : {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          break;
      }

      journalEntry.QuestTag = "craft_job";
      journalEntry.Priority = 1;
      journalEntry.QuestDisplayed = true;
      player.oid.AddCustomJournalEntry(journalEntry);
    }
    public void CancelCraftJournalEntry()
    {
      JournalEntry journalEntry = player.oid.GetJournalEntry("craft_job");

      switch (type)
      {
        case JobType.Enchantement:
          foreach (Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);
          journalEntry.Name = $"Enchantement en pause";
          break;
        case JobType.Recycling:
          journalEntry.Name = $"Recyclage en pause";
          break;
        case JobType.Repair:
          journalEntry.Name = $"Réparations en pause";
          break;
        case JobType.EnchantementReactivation:
          foreach (Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);
          journalEntry.Name = $"Ré-enchantement en pause";
          break;
      }

      journalEntry.QuestTag = "craft_job";
      journalEntry.QuestDisplayed = false;
      player.oid.AddCustomJournalEntry(journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
    public void CloseCraftJournalEntry()
    {
      JournalEntry journalEntry = player.oid.GetJournalEntry("craft_job");

      switch (type)
      {
        case JobType.Enchantement:
          foreach (Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1055, player.oid.ControlledCreature);
          journalEntry.Name = $"Enchantement terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          break;
        case JobType.Recycling:
          journalEntry.Name = $"Recyclage terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          player.oid.ApplyInstantVisualEffectToObject((VfxType)818, player.oid.ControlledCreature);
          break;
        case JobType.Repair:
          journalEntry.Name = $"Réparations terminées - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
          break;
        case JobType.EnchantementReactivation:
          foreach (Effect vfx in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "VFX_ENCHANTEMENT"))
            player.oid.LoginCreature.RemoveEffect(vfx);

          player.oid.ApplyInstantVisualEffectToObject((VfxType)872, player.oid.ControlledCreature);
          journalEntry.Name = $"Ré-enchantement terminé - {NwItem.Deserialize(craftedItem.ToByteArray()).Name}";
          break;
      }

      journalEntry.QuestTag = "craft_job";
      journalEntry.QuestCompleted = true;
      journalEntry.QuestDisplayed = false;
      player.oid.AddCustomJournalEntry(journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
  }
}
