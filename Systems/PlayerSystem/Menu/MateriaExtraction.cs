using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MateriaExtractionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<string> remainingTime = new NuiBind<string>("remainingTime");
        private readonly NuiBind<float> progress = new NuiBind<float>("progress");

        private int extractionRemainingTime { get; set; }
        private int extractionTotalDuration { get; set; }
        private NwItem extractor { get; set; }
        private NwPlaceable targetMateria { get; set; }
        private ScheduledTask extractionProgress { get; set; }
        private Effect beamEffect { get; set; }
        private ResourceType resourceType = ResourceType.Ore;
        private int resourceExtractionSkill = CustomSkill.MineralExtraction;
        private int resourceExtractionSpeedSkill = CustomSkill.MineralExtractionSpeed;
        private int resourceYieldSkill = CustomSkill.MineralExtractionYield;
        private int resourceCriticalSuccessSkill = CustomSkill.MineralExtractionCriticalSuccess;
        private int resourceCriticalFailureSkill = CustomSkill.MineralExtractionCriticalFailure;

        public MateriaExtractionWindow(Player player, NwItem extractor, NwGameObject oTarget) : base(player)
        {
          windowId = "materiaExtraction";

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiProgress(progress) } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiText(remainingTime) { Tooltip = "Temps restant avant la fin de l'extraction" } } }
            }
          };

          CreateWindow(extractor, oTarget);
        }
        public void CreateWindow(NwItem extractor, NwGameObject oTarget)
        {
          if (!extractionProgress.IsCancelled)
            extractionProgress.Dispose();

          if (oTarget == null || !(oTarget is NwPlaceable materia) || oTarget.Tag != "mineable_materia")
            return;

          if(player.oid.ControlledCreature.DistanceSquared(materia) > 25)
          {
            player.oid.SendServerMessage("Vous êtes trop éloigné pour démarrer le processus d'extraction.", ColorConstants.Red);
            return;
          }

          SelectExtractionSkill(oTarget.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value);
          SetExtractionTime();

          if(targetMateria != null)
            foreach (Effect eff in targetMateria.ActiveEffects.Where(e => e.Tag == $"_{player.oid.CDKey}_MINING_BEAM"))
              targetMateria.RemoveEffect(eff);

          this.extractor = extractor;
          this.targetMateria = materia;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, "Extraction en cours")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          remainingTime.SetBindValue(player.oid, token, GetReadableExtractionTime());
          progress.SetBindValue(player.oid, token, 0);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;

          if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == "_RESOURCE_EXTRACTION_HIDE_DEBUFF"))
          {
            Effect eff = Effect.SkillDecrease(NwSkill.FromSkillType(Skill.Hide), 100);
            eff = Effect.LinkEffects(eff, Effect.SkillDecrease(NwSkill.FromSkillType(Skill.MoveSilently), 100));
            eff.SubType = EffectSubType.Supernatural;

            player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, eff, TimeSpan.FromSeconds(extractionTotalDuration));
          }

          Effect eRay = Effect.Beam(VfxType.BeamDisintegrate, extractor, BodyNode.Hand);
          eRay.Tag = $"_{player.oid.CDKey}_MINING_BEAM";
          oTarget.ApplyEffect(EffectDuration.Temporary, eRay, TimeSpan.FromSeconds(extractionRemainingTime));

          extractionProgress = ModuleSystem.scheduler.ScheduleRepeating(HandleExtractionProgress, TimeSpan.FromSeconds(1));
        }

        private void HandleExtractionProgress()
        {
          if(player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId) || player.oid.LoginCreature.IsInCombat || player.oid.LoginCreature.IsResting 
            || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.CastSpell || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.AttackObject
            || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.CounterSpell || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.SetTrap
            || player.oid.LoginCreature.CurrentAction == Anvil.API.Action.ItemCastSpell)
          {
            CancelExtraction();
            return;
          }

          extractionRemainingTime -= 1;
          remainingTime.SetBindValue(player.oid, token, GetReadableExtractionTime());
          progress.SetBindValue(player.oid, token, (extractionTotalDuration - extractionRemainingTime) / extractionTotalDuration);

          foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.EffectType ==  EffectType.Invisibility || e.EffectType == EffectType.ImprovedInvisibility))
          {
            player.oid.LoginCreature.RemoveEffect(eff);
            player.oid.SendServerMessage("Le bruit et les vibrations liées à l'extraction permettent de vous localiser facilement malgré tout effet d'invisibilité ou de silence.", ColorConstants.Red);
          }

          if (extractionRemainingTime < 1)
          {
            extractionProgress.Dispose();
            player.oid.NuiDestroy(token);

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

            Craft.Collect.System.UpdateResourceBlockInfo(targetMateria);

            int miningYield = GetMiningYield();
            int grade = GetResourceGrade();
            
            // retirer le yield de la matéria + mise à jour en BDD + destruction de la matéria et du placeable

            int remainingMateria = targetMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
            string areaTag = targetMateria.Area.Tag;

            if (miningYield >= remainingMateria)
            {
              miningYield = remainingMateria;
              HandleMateriaDestruction();
            }
            else
            {
              remainingMateria -= miningYield;
              targetMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = remainingMateria;
              HandleMateriaUpdate(remainingMateria);
            }

            // donner le yield au joueur
            Craft.Collect.System.CreateSelectedResourceInInventory(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == resourceType && r.grade == grade),
              player, miningYield);

            // retirer debuff stealth
            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_RESOURCE_EXTRACTION_HIDE_DEBUFF"))
              player.oid.LoginCreature.RemoveEffect(eff);

            player.oid.SendServerMessage($"Vous parvenez à extraire {miningYield.ToString().ColorString(ColorConstants.White)} unité(s) de matéria", new Color(32, 255, 32));

            return;
          }
        }
        private void SelectExtractionSkill(string type)
        {
          switch (type)
          {
            case "wood_spawn_wp":
              resourceType = ResourceType.Wood;
              resourceExtractionSkill = CustomSkill.WoodExtraction;
              resourceExtractionSpeedSkill = CustomSkill.WoodExtractionSpeed;
              resourceYieldSkill = CustomSkill.WoodExtractionYield;
              resourceCriticalSuccessSkill = CustomSkill.WoodExtractionCriticalSuccess;
              resourceCriticalFailureSkill = CustomSkill.WoodExtractionCriticalFailure;
              break;
            case "animal_spawn_wp":
              resourceType = ResourceType.Pelt;
              resourceExtractionSkill = CustomSkill.PeltExtraction;
              resourceExtractionSpeedSkill = CustomSkill.PeltExtractionSpeed;
              resourceYieldSkill = CustomSkill.PeltExtractionYield;
              resourceCriticalSuccessSkill = CustomSkill.PeltExtractionCriticalSuccess;
              resourceCriticalFailureSkill = CustomSkill.PeltExtractionCriticalFailure;
              break;
          }
        }
        private void SetExtractionTime()
        {
          extractionTotalDuration = 180;
          extractionTotalDuration -= player.learnableSkills.ContainsKey(resourceExtractionSkill) ? extractionTotalDuration * (int)(player.learnableSkills[resourceExtractionSkill].totalPoints * 0.05) : 0;
          extractionTotalDuration -= player.learnableSkills.ContainsKey(resourceExtractionSpeedSkill) ? extractionTotalDuration * (int)(player.learnableSkills[resourceExtractionSpeedSkill].totalPoints * 0.05) : 0;
          extractionRemainingTime = extractionTotalDuration;
        }
        private string GetReadableExtractionTime()
        {
          return new TimeSpan(TimeSpan.FromSeconds(extractionRemainingTime).Hours, TimeSpan.FromSeconds(extractionRemainingTime).Minutes, TimeSpan.FromSeconds(extractionRemainingTime).Seconds).ToString();
        }
        private void CancelExtraction()
        {
          extractionProgress.Dispose();

          if (targetMateria != null)
            foreach (Effect eff in targetMateria.ActiveEffects.Where(e => e.Tag == $"_{player.oid.CDKey}_MINING_BEAM"))
              targetMateria.RemoveEffect(eff);

          if (player.oid.LoginCreature != null)
            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_RESOURCE_EXTRACTION_HIDE_DEBUFF"))
              player.oid.LoginCreature.RemoveEffect(eff);

          if (player.oid != null)
            player.oid.SendServerMessage("Votre tentative d'extraction a été annulée.", ColorConstants.Orange);
        }
        private int GetMiningYield()
        {
          int miningYield = 10;
          miningYield += extractor.GetObjectVariable<LocalVariableInt>("_ITEM_LEVEL").Value * 5;
          miningYield = player.learnableSkills.ContainsKey(resourceExtractionSkill) ? miningYield + miningYield * player.learnableSkills[resourceExtractionSkill].totalPoints / 100 : miningYield;
          return miningYield = player.learnableSkills.ContainsKey(resourceYieldSkill) ? miningYield + miningYield * player.learnableSkills[resourceYieldSkill].totalPoints / 100 : miningYield;
        }
        private int GetResourceGrade()
        {
          int grade = targetMateria.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value - 1;
          int random = NwRandom.Roll(Utils.random, 100);
          int antiFailureSkill = player.learnableSkills.ContainsKey(resourceCriticalFailureSkill) ? player.learnableSkills[resourceCriticalFailureSkill].totalPoints : 0;
          int criticalSuccessSkill = player.learnableSkills.ContainsKey(resourceCriticalSuccessSkill) ? player.learnableSkills[resourceCriticalSuccessSkill].totalPoints : 0;

          if (random + antiFailureSkill < 11 && grade > 1)
            grade -= 1;

          if (random + criticalSuccessSkill > 100 && grade < 8)
            grade += 1;

          return grade;
        }
        private void HandleMateriaDestruction()
        {
          string areaTag = targetMateria.Area.Tag;
          int materiaId = targetMateria.GetObjectVariable<LocalVariableInt>("id").Value;

          targetMateria.Destroy();
          // TODO : Vérifier le tag des placeables mineables pour les variabiliser
          // TODO : envisager animation de destruction pour le placeable ?
          targetMateria.Area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(p => p.Tag == "mineable_tree" && p.GetObjectVariable<LocalVariableInt>("id").Value == materiaId).Destroy();
          NwWaypoint resourceWapoint = NwWaypoint.Create(targetMateria.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value, targetMateria.Location);

          SqLiteUtils.DeletionQuery("areaResourceStock",
          new Dictionary<string, string>() { { "areaTag", areaTag }, { "id", materiaId.ToString() } });
        }
        private void HandleMateriaUpdate(int remainingMateria)
        {
          foreach (Effect eff in targetMateria.ActiveEffects.Where(e => e.Tag == $"_{player.oid.CDKey}_MINING_BEAM"))
            targetMateria.RemoveEffect(eff);

          string areaTag = targetMateria.Area.Tag;
          string materiaId = targetMateria.GetObjectVariable<LocalVariableInt>("id").Value.ToString();

          // mise à jour BDD
          SqLiteUtils.UpdateQuery("areaResourceStock",
          new List<string[]>() { new string[] { "quantity", remainingMateria.ToString() }, new string[] { "lastChecked", DateTime.Now.ToString() } },
          new List<string[]>() { new string[] { "areaTag", areaTag }, new string[] { "id", materiaId.ToString() } });
        }
      }
    }
  }
}
