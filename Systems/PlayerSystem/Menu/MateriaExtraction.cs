using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Anvil.API;
using Anvil.Services;

using Microsoft.Data.Sqlite;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MateriaExtractionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<float> progress = new("progress");
        private readonly NuiBind<string> readableRemainingTime = new("readableRemainingTime");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private double extractionRemainingTime { get; set; }
        private double extractionTotalDuration { get; set; }
        private NwItem extractor { get; set; }
        private NwGameObject targetMateria { get; set; }
        private ScheduledTask extractionProgress { get; set; }
        private ResourceType resourceType = ResourceType.Ore;
        private int resourceExtractionSkill = CustomSkill.OreExtraction;
        private int resourceExtractionSpeedSkill = CustomSkill.OreExtractionSpeed;
        private int resourceYieldSkill = CustomSkill.OreExtractionYield;
        private int resourceSafetySkill = CustomSkill.OreExtractionSafe;
        private int resourceDurableSkill = CustomSkill.OreExtractionDurable;
        private int resourceAdvancedSkill = CustomSkill.OreExtractionAdvanced;
        private int resourceMasterySkill = CustomSkill.OreExtractionMastery;

        public MateriaExtractionWindow(Player player, NwItem extractor, NwGameObject oTarget) : base(player)
        {
          windowId = "materiaExtraction";

          rootColumn = new NuiColumn() { Children = new List<NuiElement>() { new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiProgress(progress) { Width = 485, Height = 35, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(white, drawListRect, readableRemainingTime) } }
          } } } };

          CreateWindow(extractor, oTarget);
        }
        public void CreateWindow(NwItem extractor, NwGameObject oTarget)
        {
          if (oTarget is null || oTarget.Tag != "mineable_materia")
            return;

          if (player.oid.LoginCreature.DistanceSquared(oTarget) > 50)
          {
            player.oid.SendServerMessage("Vous êtes trop éloigné pour démarrer le processus d'extraction.", ColorConstants.Red);
            return;
          }

          this.extractor = extractor;

          SelectExtractionSkill(oTarget.ResRef);
          SetExtractionTime();

          foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_MINING_BEAM"))
            targetMateria.RemoveEffect(eff);

          this.targetMateria = oTarget;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, 495, 45) : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 250, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 495, 45);

          window = new NuiWindow(rootColumn, "")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = false,
            Border = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, GetReadableExtractionTime());
            progress.SetBindValue(player.oid, nuiToken.Token, 0);
            drawListRect.SetBindValue(player.oid, nuiToken.Token, new(300, 15, 151, 20));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            Effect eRay = Effect.Beam(VfxType.BeamDisintegrate, oTarget, BodyNode.Chest);
            eRay = Effect.LinkEffects(eRay, Effect.SkillDecrease(NwSkill.FromSkillType(Skill.Hide), 100));
            eRay = Effect.LinkEffects(eRay, Effect.SkillDecrease(NwSkill.FromSkillType(Skill.MoveSilently), 100));
            eRay.SubType = EffectSubType.Supernatural;
            eRay.Tag = "_MINING_BEAM";

            player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, eRay, TimeSpan.FromSeconds(extractionRemainingTime));

            extractionProgress?.Dispose();
            extractionProgress = player.scheduler.ScheduleRepeating(HandleExtractionProgress, TimeSpan.FromSeconds(1));
          }
        }

        private void HandleExtractionProgress()
        {
          if (player.oid == null || player.oid.LoginCreature == null || targetMateria == null || player.oid.LoginCreature.Area != targetMateria.Area || extractor == null 
            || extractor.Possessor != player.oid.LoginCreature || !IsOpen || player.oid.LoginCreature.IsInCombat || player.oid.LoginCreature.IsResting
            || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.CastSpell || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.AttackObject
            || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.CounterSpell || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.SetTrap
            || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.ItemCastSpell)
          {
            CancelExtraction();
            return;
          }

          extractionRemainingTime -= 1;
          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, GetReadableExtractionTime());
          progress.SetBindValue(player.oid, nuiToken.Token, (float)((extractionTotalDuration - extractionRemainingTime) / extractionTotalDuration));

          foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.EffectType == EffectType.Invisibility || e.EffectType == EffectType.ImprovedInvisibility))
          {
            player.oid.LoginCreature.RemoveEffect(eff);
            player.oid.SendServerMessage("Le bruit et les vibrations liées à l'extraction permettent de vous localiser facilement malgré tout effet d'invisibilité.", ColorConstants.Red);
          }

          if (extractionRemainingTime < 1)
          {
            extractionProgress?.Dispose();
            CloseWindow();

            if (targetMateria == null)
            {
              player.oid.SendServerMessage("La veine de matéria que vous tentiez d'extraire a été ruinée, impossible d'en retirer quoique ce soit.", ColorConstants.Red);
              return;
            }

            if (extractor == null)
            {
              player.oid.SendServerMessage("Votre extracteur a été détruit. Impossible de procéder à l'extraction.", ColorConstants.Red);
              return;
            }

            if (player.oid.LoginCreature.DistanceSquared(targetMateria) > 50)
            {
              player.oid.SendServerMessage("Vous êtes trop éloigné du dépôt. Impossible de finaliser le processus d'extraction.", ColorConstants.Red);
              return;
            }

            int miningYield = GetMiningYield();
            int grade = GetResourceGrade();
            int remainingMateria = targetMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;

            if (miningYield >= remainingMateria)
            {
              miningYield = remainingMateria;
              HandleMateriaDestruction(remainingMateria);
            }
            else
            {

              HandleMateriaUpdate(remainingMateria, remainingMateria - miningYield);
              targetMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value -= miningYield;
            }

            CreateSelectedResourceInInventory(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == resourceType && r.grade == grade), miningYield);

            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_MINING_BEAM"))
              player.oid.LoginCreature.RemoveEffect(eff);

            ItemUtils.HandleCraftToolDurability(player, extractor, "EXTRACTOR", resourceSafetySkill);
            player.oid.SendServerMessage($"Vous parvenez à extraire {StringUtils.ToWhitecolor(miningYield)} unité(s) de matéria de niveau de concentration {StringUtils.ToWhitecolor(grade)}", new Color(32, 255, 32));

            return;
          }
        }
        private void SelectExtractionSkill(string type)
        {
          switch (type)
          {
            case "mineable_tree":
              resourceType = ResourceType.Wood;
              resourceExtractionSkill = CustomSkill.WoodExtraction;
              resourceExtractionSpeedSkill = CustomSkill.WoodExtractionSpeed;
              resourceYieldSkill = CustomSkill.WoodExtractionYield;
              resourceSafetySkill = CustomSkill.WoodExtractionSafe;
              resourceDurableSkill = CustomSkill.WoodExtractionDurable;
              resourceAdvancedSkill = CustomSkill.WoodExtractionAdvanced;
              resourceMasterySkill = CustomSkill.WoodExtractionMastery;
              break;

            case "mineable_animal":
              resourceType = ResourceType.Pelt;
              resourceExtractionSkill = CustomSkill.PeltExtraction;
              resourceExtractionSpeedSkill = CustomSkill.PeltExtractionSpeed;
              resourceYieldSkill = CustomSkill.PeltExtractionYield;
              resourceSafetySkill = CustomSkill.PeltExtractionSafe;
              resourceDurableSkill = CustomSkill.PeltExtractionDurable;
              resourceAdvancedSkill = CustomSkill.PeltExtractionAdvanced;
              resourceMasterySkill = CustomSkill.PeltExtractionMastery;
              break;
          }
        }
        private void SetExtractionTime()
        {
          extractionTotalDuration = Config.env == Config.Env.Prod ? Config.extractionBaseDuration : 10;
          extractionTotalDuration -= extractionTotalDuration * (int)(player.learnableSkills[CustomSkill.MateriaScanning].totalPoints * 0.05);
          extractionTotalDuration -= player.learnableSkills.ContainsKey(resourceExtractionSkill) ? extractionTotalDuration * (int)(player.learnableSkills[resourceExtractionSkill].totalPoints * 0.05) : 0;
          extractionTotalDuration -= player.learnableSkills.ContainsKey(resourceExtractionSpeedSkill) ? extractionTotalDuration * (int)(player.learnableSkills[resourceExtractionSpeedSkill].totalPoints * 0.05) : 0;
          extractionTotalDuration -= player.learnableSkills.ContainsKey(resourceAdvancedSkill) ? extractionTotalDuration * (int)(player.learnableSkills[resourceAdvancedSkill].totalPoints * 0.05) : 0;
          extractionTotalDuration -= player.learnableSkills.ContainsKey(resourceMasterySkill) ? extractionTotalDuration * (int)(player.learnableSkills[resourceMasterySkill].totalPoints * 0.05) : 0;
          
          
          extractionTotalDuration -= extractionTotalDuration * (extractor.LocalVariables.Where(l => l.Name.StartsWith("ENCHANTEMENT_CUSTOM_EXTRACTOR_SPEED_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100);

          extractionRemainingTime = extractionTotalDuration;
        }
        private string GetReadableExtractionTime()
        {
          return new TimeSpan(TimeSpan.FromSeconds(extractionRemainingTime).Hours, TimeSpan.FromSeconds(extractionRemainingTime).Minutes, TimeSpan.FromSeconds(extractionRemainingTime).Seconds).ToString();
        }
        private void CancelExtraction()
        {
          extractionProgress?.Dispose();
          CloseWindow();

          foreach (Effect eff in player.oid?.LoginCreature?.ActiveEffects.Where(e => e.Tag == "_MINING_BEAM"))
            player.oid.LoginCreature.RemoveEffect(eff);

          /*if (player.oid != null && player.oid.LoginCreature != null)
            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_RESOURCE_EXTRACTION_HIDE_DEBUFF"))
              player.oid.LoginCreature.RemoveEffect(eff);*/

          player.oid?.SendServerMessage("Tentative d'extraction annulée.", ColorConstants.Orange);
        }
        private int GetMiningYield()
        {
          double miningYield = Config.extractionBaseYield;
          miningYield += miningYield * (int)(player.learnableSkills[CustomSkill.MateriaScanning].totalPoints * 0.05);
          miningYield += player.learnableSkills.ContainsKey(resourceExtractionSkill) ? miningYield * (int)(player.learnableSkills[resourceExtractionSkill].totalPoints * 0.05) : 0;
          miningYield += player.learnableSkills.ContainsKey(resourceYieldSkill) ? miningYield * (int)(player.learnableSkills[resourceYieldSkill].totalPoints * 0.05) : 0;
          miningYield += player.learnableSkills.ContainsKey(resourceAdvancedSkill) ? miningYield * (int)(player.learnableSkills[resourceAdvancedSkill].totalPoints * 0.05) : 0;
          miningYield += player.learnableSkills.ContainsKey(resourceMasterySkill) ? miningYield * (int)(player.learnableSkills[resourceMasterySkill].totalPoints * 0.05) : 0;
          miningYield += miningYield * (extractor.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_EXTRACTOR_YIELD_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100);

          return (int)miningYield;
        }
        private int GetResourceGrade()
        {
          // Plus la ressource est de niveau important, plus la chance de critical success est basse et plus celle de failure est élevée
          int grade = targetMateria.GetObjectVariable<LocalVariableInt>("_GRADE").Value;
          int gradeChance = (grade - 1) * 2;
          int skill = player.learnableSkills[CustomSkill.MateriaScanning].totalPoints;
          skill += player.learnableSkills.ContainsKey(resourceExtractionSkill) ? skill * (int)(player.learnableSkills[resourceExtractionSkill].totalPoints * 0.01) : 0;
          skill += player.learnableSkills.ContainsKey(resourceAdvancedSkill) ? skill * (int)(player.learnableSkills[resourceAdvancedSkill].totalPoints * 0.01) : 0;
          skill += player.learnableSkills.ContainsKey(resourceMasterySkill) ? skill * (int)(player.learnableSkills[resourceMasterySkill].totalPoints * 0.01) : 0;
          skill += extractor.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_EXTRACTOR_QUALITY_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value);

          int random = NwRandom.Roll(Utils.random, 100);

          if (random - gradeChance + skill < 1 && grade > 1)
          {
            grade -= 1;
            player.oid.SendServerMessage("Cette extraction est plus difficile que d'habitude. Vous ne parvenez à extraire qu'une matéria de moindre qualité.");
          }

          if (random + gradeChance + skill > 100 && grade < 8)
          {
            grade += 1;
            player.oid.SendServerMessage("Votre compétence vous permet d'extraire une matéria de qualité supérieure.");
          }

          return grade;
        }
        private async void HandleMateriaDestruction(int remainingMateria)
        {
          int materiaGrade = targetMateria.GetObjectVariable<LocalVariableInt>("_GRADE").Value;
          string resRef = targetMateria.ResRef;
          Location location = targetMateria.Location;

          if (player.learnableSkills.ContainsKey(resourceDurableSkill))
          {
            if(NwRandom.Roll(Utils.random, 100) <= player.learnableSkills[resourceDurableSkill].totalPoints)
            {
              player.oid.SendServerMessage("Votre maîtrise de l'extraction vous a permis de conserver le dépôt intact malgré son épuisement total.");
              targetMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = 0;

              try
              {
                using (var connection = new SqliteConnection(Config.dbPath))
                {
                  connection.Open();

                  var sqlCommand = connection.CreateCommand();
                  sqlCommand.CommandText = $"UPDATE areaResourceStock SET quantity = 0 " +
                                        $"WHERE type = '{resRef}' and quantity = {remainingMateria} and grade = {materiaGrade} and location = '{SqLiteUtils.SerializeLocation(location)}' ";

                  await sqlCommand.ExecuteNonQueryAsync();
                }
              }
              catch (Exception e) { Utils.LogMessageToDMs($"Update Query - Materia Block - {e.Message}"); }

              return;
            }
          }

          targetMateria.Destroy(); // TODO : envisager animation de destruction pour le placeable ?

          try
          {
            using (var connection = new SqliteConnection(Config.dbPath))
            {
              connection.Open();

              var sqlCommand = connection.CreateCommand();
              sqlCommand.CommandText = $"DELETE from areaResourceStock " +
                                    $"WHERE type = '{resRef}' and quantity = {remainingMateria} and grade = {materiaGrade} and location = '{SqLiteUtils.SerializeLocation(location)}' ";

              await sqlCommand.ExecuteNonQueryAsync();
            }
          }
          catch (Exception e) { Utils.LogMessageToDMs($"Delete Query - Materia Block - {e.Message}"); }
        }
        private async void HandleMateriaUpdate(int previousMateria, int remainingMateria)
        {
          int materiaGrade = targetMateria.GetObjectVariable<LocalVariableInt>("_GRADE").Value;
          string resRef = targetMateria.ResRef;
          Location location = targetMateria.Location;

          try
          {
            using (var connection = new SqliteConnection(Config.dbPath))
            {
              connection.Open();

              var sqlCommand = connection.CreateCommand();
              sqlCommand.CommandText = $"UPDATE areaResourceStock SET quantity = {remainingMateria} " +
                                    $"WHERE type = '{resRef}' and quantity = {previousMateria} and grade = {materiaGrade} and location = '{SqLiteUtils.SerializeLocation(location)}' ";

              await sqlCommand.ExecuteNonQueryAsync();
            }
          }
          catch (Exception e)
          {
            Utils.LogMessageToDMs($"Update Query - Materia Block - {e.Message}");
          }
        }
        private void CreateSelectedResourceInInventory(CraftResource selection, int quantity)
        {
          NwItem pcResource = NwItem.Create("craft_resource", player.oid.LoginCreature.Location);
          pcResource.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value = selection.type.ToString();
          pcResource.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value = selection.grade;
          pcResource.Name = selection.name;
          pcResource.Description = selection.description;
          pcResource.Weight = selection.weight;
          pcResource.Appearance.SetSimpleModel(selection.icon);
          pcResource.StackSize = quantity;

          player.oid.LoginCreature.AcquireItem(pcResource, false);
        }
      }
    }
  }
}
