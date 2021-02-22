using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using System.Collections.Generic;
using static NWN.Systems.PlayerSystem;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static Dictionary<int, API.ItemProperty[]> enchantementCategories = new Dictionary<int, API.ItemProperty[]>()
    {
      {840, new API.ItemProperty[] { API.ItemProperty.AbilityBonus(IPAbility.Constitution, 1) } },
      {841, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Vermin, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidGoblinoid, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Animal, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidReptilian, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Vermin, 1) } },
      {842, new API.ItemProperty[] { API.ItemProperty.MassiveCritical(IPDamageBonus.Plus2) } },
      {843, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fear, 1) } },
      {844, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1) , API.ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1) } },
      {845, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidOrc, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Undead, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Beast, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HumanoidMonstrous, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.ShapeChanger, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidOrc, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Undead, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Beast, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HumanoidMonstrous, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.ShapeChanger, 1) } },
      {846, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.Elemental, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Fey, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Giant, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Construct, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Elemental, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Fey, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Giant, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Construct, 1) } },
      {847, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Death, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.MindAffecting, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Acid, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Cold, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Disease, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Divine, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Electrical, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Fire, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Negative, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Poison, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Positive, 1), API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Sonic, 1) } },
      {848, new API.ItemProperty[] { API.ItemProperty.ACBonusVsDmgType(IPDamageType.Bludgeoning, 1), API.ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 1), API.ItemProperty.ACBonusVsDmgType(IPDamageType.Slashing, 1) } },
      {849, new API.ItemProperty[] { API.ItemProperty.MassiveCritical(IPDamageBonus.Plus1d4) } },
      {850, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), API.ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1) , API.ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1) } },
      {851, new API.ItemProperty[] { API.ItemProperty.AbilityBonus(IPAbility.Strength, 1), API.ItemProperty.AbilityBonus(IPAbility.Dexterity, 1) } },
      {852, new API.ItemProperty[] { API.ItemProperty.Regeneration(1) } },
      {853, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Acid, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Divine, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Negative, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Electrical, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Positive, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Cold, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Magical, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Sonic, IPDamageImmunityType.Immunity5Pct) } },
      {854, new API.ItemProperty[] { API.ItemProperty.ACBonusVsRace(IPRacialType.Halfling, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Human, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HalfElf, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.HalfOrc, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Elf, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Gnome, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Dwarf, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.MagicalBeast, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Dragon, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Outsider, 1), API.ItemProperty.ACBonusVsRace(IPRacialType.Aberration, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Halfling, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Human, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HalfElf, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.HalfOrc, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Elf, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Gnome, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Dwarf, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.MagicalBeast, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Dragon, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Outsider, 1), API.ItemProperty.AttackBonusVsRace(IPRacialType.Aberration, 1) } },
      {855, new API.ItemProperty[] { API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Good, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Chaotic, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Neutral, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Evil, 1), API.ItemProperty.ACBonusVsAlign(IPAlignmentGroup.Lawful, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Good, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Chaotic, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Neutral, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Evil, 1), API.ItemProperty.AttackBonusVsAlign(IPAlignmentGroup.Lawful, 1) } },
      {856, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrow(IPSaveBaseType.Reflex, 1), API.ItemProperty.BonusSavingThrow(IPSaveBaseType.Fortitude, 1), API.ItemProperty.BonusSavingThrow(IPSaveBaseType.Will, 1) } },
      {857, new API.ItemProperty[] { API.ItemProperty.AbilityBonus(IPAbility.Intelligence, 1), API.ItemProperty.AbilityBonus(IPAbility.Wisdom, 1), API.ItemProperty.AbilityBonus(IPAbility.Charisma, 1) } },
      {858, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Acid, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Cold, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Electrical, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Negative, IPDamageBonus.Plus1d4) , API.ItemProperty.DamageBonus(IPDamageType.Positive, IPDamageBonus.Plus1d4) } },
      {853, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Acid, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Divine, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Negative, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Electrical, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Positive, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Cold, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Magical, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Sonic, IPDamageImmunityType.Immunity10Pct) } },
      {854, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Piercing, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Slashing, IPDamageImmunityType.Immunity5Pct), API.ItemProperty.DamageImmunity(IPDamageType.Bludgeoning, IPDamageImmunityType.Immunity5Pct) } },
      {855, new API.ItemProperty[] { API.ItemProperty.MassiveCritical(IPDamageBonus.Plus1d8) } },
      {856, new API.ItemProperty[] { API.ItemProperty.BonusSavingThrowVsX(IPSaveVs.Universal, 1) } },
      {857, new API.ItemProperty[] { API.ItemProperty.ACBonus(1) } },
      {858, new API.ItemProperty[] { API.ItemProperty.AttackBonus(1) } },
      {859, new API.ItemProperty[] { API.ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Sonic, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d4), API.ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d4) , API.ItemProperty.DamageBonus(IPDamageType.Bludgeoning, IPDamageBonus.Plus1d4) } },
      {854, new API.ItemProperty[] { API.ItemProperty.DamageImmunity(IPDamageType.Piercing, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Slashing, IPDamageImmunityType.Immunity10Pct), API.ItemProperty.DamageImmunity(IPDamageType.Bludgeoning, IPDamageImmunityType.Immunity10Pct) } },
      {858, new API.ItemProperty[] { API.ItemProperty.VampiricRegeneration(1) } },
      {859, new API.ItemProperty[] { API.ItemProperty.EnhancementBonus(1) } },
      {860, new API.ItemProperty[] { API.ItemProperty.Keen() } },
      {861, new API.ItemProperty[] { API.ItemProperty.Haste() } },
    };

    [ScriptHandler("on_ench_cast")]
    private void HandleItemEnchantement(CallInfo callInfo)
    {
      NwItem oTarget = NWScript.GetSpellTargetObject().ToNwObject<NwItem>();

      if (!(callInfo.ObjectSelf is NwPlayer) || oTarget == null || !Players.TryGetValue(callInfo.ObjectSelf, out Player player))
        return;

      NwPlayer oCaster = (NwPlayer)callInfo.ObjectSelf;

      if (!player.craftJob.CanStartJob(oCaster, NWScript.OBJECT_INVALID, Craft.Job.JobType.Enchantement))
        return;

      if(oTarget.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
      {
        oCaster.SendServerMessage($"{oTarget.Name} ne dispose d'aucun emplacement d'enchantement disponible !");
        return;
      }

      int spellId = NWScript.GetSpellId();

      if (enchantementCategories.ContainsKey(spellId))
        DrawEnchantementChoicePage(player, oTarget.Name, spellId, oTarget);
      else
      {
        oCaster.SendServerMessage("HRP - Propriétés de cet enchantement incorrectement définies. L'erreur a été remontée au staff");
        Utils.LogMessageToDMs($"ENCHANTEMENT - {spellId} - ItemProperties non présentes dans le dictionnaire.");
        return;
      }
    }
    private void DrawEnchantementChoicePage(Player player, string itemName, int spellId, NwItem oItem)
    {
      if (enchantementCategories[spellId].Length == 1)
      {
        HandleEnchantementChoice(player, enchantementCategories[spellId][0], spellId, oItem);
        return;
      }
      
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quel enchantement souhaitez-vous appliquer sur votre {itemName} ?"
      };

      foreach (API.ItemProperty ip in enchantementCategories[spellId])
        player.menu.choices.Add(($"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("itempropdef", "Name", NWScript.GetItemPropertyType(ip))))} - " +
          $"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString(NWScript.Get2DAString("itempropdef", "SubTypeResRef", NWScript.GetItemPropertyType(ip)), "Name", NWScript.GetItemPropertySubType(ip))))}", () => HandleEnchantementChoice(player, ip, spellId, oItem)));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }

    private static void HandleEnchantementChoice(Player player, API.ItemProperty ip, int spellId, NwItem oItem)
    {
      //player.oid.SendServerMessage($"ip string : {$"{spellId}_{(int)ip.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}"}");

      player.craftJob.Start(Craft.Job.JobType.Enchantement, null, player, NWScript.OBJECT_INVALID, oItem, $"{spellId}_{(int)ip.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}");
      player.oid.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(VfxType.ImpSuperHeroism));

      player.menu.Close();
    }
  }
}
