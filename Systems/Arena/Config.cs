using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;
using NWNX.API.Events;
using NWNX.Services;
using static NWN.Systems.Arena.Utils;

namespace NWN.Systems.Arena
{
  public static class Config
  {
    public const string PVE_ARENA_AREA_RESREF = "c2c_arena";
    public const string PVE_ARENA_WAYPOINT_TAG = "waypoint";
    public const string PVE_ARENA_MONSTER_WAYPOINT_TAG = "_ARENA_MONSTER_SPAWN";
    public const string PVE_ENTRY_WAYPOINT_TAG = "arena_entry_waypoint";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_TAG = "PullRopeChain";
    public const string PVE_ARENA_PULL_ROPE_CHAIN_ON_USED_SCRIPT = "arena_chain_ou";
    public const string PVE_ARENA_CREATURE_ON_DEATH_SCRIPT = "arena_ondeath";
    public const string PVE_ARENA_CHALLENGER_VARNAME = "PVE_ARENA_CHALLENGER";
    public const string PVE_ARENA_CREATURE_TAG = "pve_arena_creature";
    public const int roundMax = 8;
    public enum Difficulty
    {
      Level1 = 1,
      Level2,
      Level3,
      Level4,
      Level5,
    }

    public static Dictionary<uint, ArenaMalus> arenaMalusDictionary = new Dictionary<uint, ArenaMalus>()
    {
      { 1, new ArenaMalus("Sorts de soins interdits", 3.01, ApplyNoHealingSpellMalus) },
      { 2, new ArenaMalus("Invocations interdites", 2.99, ApplyNoSummonsMalus) },
      { 3, new ArenaMalus("Magie offensive interdite", 2.89, ApplyNoOffensiveSpellsMalus) },
      { 4, new ArenaMalus("Magie défensive interdite", 3.14, ApplyNoBuffsMalus) },
      { 5, new ArenaMalus("Magie interdite", 3.16, ApplyNoMagicMalus) },
      { 6, new ArenaMalus("Accessoires interdits", 2.9, ApplyNoAccessoriesMalus) },
      { 7, new ArenaMalus("Armure interdite", 2.7, ApplyNoArmorMalus) },
      { 8, new ArenaMalus("Armes interdites", 3.08, ApplyNoWeaponsMalus) },
      { 9, new ArenaMalus("Utilisation d'objets interdite", 3.03, ApplyNoUseableItemMalus) },
      { 10, new ArenaMalus("Ralentissement", 1.9, ApplySlowMalus) },
      { 11, new ArenaMalus("Mini", 2.71, ApplyMiniMalus) },
      { 12, new ArenaMalus("Poison", 2, ApplyPoisonMalus) },
      { 13, new ArenaMalus("Crapaud", 2.72, ApplyFrogMalus) },
      { 14, new ArenaMalus("Temps x5", 2.47, ApplyTimeX5DamageMalus) },
      { 15, new ArenaMalus("1/2 HP", 2.60, ApplyHealthHalvedMalus) },
      { 16, new ArenaMalus("Echec des sorts", 2.52, ApplySpellFailureMalus) },
      { 17, new ArenaMalus("1/2 HP + Echec des sorts", 2.73, ApplyHealthHalvedAndSpellFailureMalus) },
      { 18, new ArenaMalus("Dissipation", 2.56, ApplyDissipationMalus) },
      { 19, new ArenaMalus("Chance", 0, ApplyNoMalus) },
      { 20, new ArenaMalus("Soins", 1, ApplyNoMalusAndHeal) },
    };

    private static void ApplyNoMagicMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOMAGIC", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOMAGIC", "L'interdiction d'usage de sorts a été levée.");
    }
    private static void ApplyNoHealingSpellMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOHEALMAGIC", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOHEALMAGIC", "L'interdiction d'usage de magie curative a été levée.");
    }

    private static void ApplyNoSummonsMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOSUMMON", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOSUMMON", "L'interdiction d'usage d'invocations a été levée.");
    }
    private static void ApplyNoOffensiveSpellsMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOOFFENSIVEMAGIC", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOOFFENSIVEMAGIC", "L'interdiction d'usage de magie offensive a été levée.");
    }
    private static void ApplyNoBuffsMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOBUFF", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOBUFF", "L'interdiction d'usage de magie défensive a été levée.");
    }
    
    private static void ApplyNoArmorMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOARMOR", 109);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOARMOR", "L'interdiction de port d'armure a été levée.");
    }
    private static void ApplyNoWeaponsMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOWEAPON", 109);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOARMOR", "L'interdiction de port d'arme a été levée.");
    }
    private static void ApplyNoAccessoriesMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOACCESSORY", 109);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOARMOR", "L'interdiction de port d'accessoire a été levée.");
    }
    private static void ApplyNoUseableItemMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_NOUSEABLEITEM", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_NOUSEABLEITEM", "L'interdiction de l'utilisation d'objets a été levée.");
    }
    private static void ApplySlowMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_SLOW", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_SLOW", "Le handicap de ralentissement a été levé.");
    }
    private static void ApplyMiniMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_MINI", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_MINI", "Le handicap de miniaturisation a été levé.");
    }
    
    private static void ApplyFrogMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_FROG", 51);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_FROG", "Le handicap de métamorphose a été levé.");
    }
    
    private static void ApplyPoisonMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_POISON", 20);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_POISON", "Le handicap d'empoisonnement a été levé.");
    }
    public static void ApplyTimeX5DamageMalus(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));

      int damage = (DateTime.Now - player.pveArena.dateArenaEntered).Seconds / 10;
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Magical));
    }
    private static void ApplyHealthHalvedMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_HALF_HEALTH", 20);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_HALF_HEALTH", "Le handicap de résilience a été levé.");
    }
    private static void ApplySpellFailureMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_SPELL_FAILURE", 20);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_SPELL_FAILURE", "Le handicap d'échec des sorts a été levé.");
    }
    private static void ApplyHealthHalvedAndSpellFailureMalus(PlayerSystem.Player player)
    {
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_HALF_HEALTH", 20);
      SpellUtils.ApplyCustomEffectToTarget(player.oid, "CUSTOM_EFFECT_SPELL_FAILURE", 20);
      RemoveArenaMalus(player, "CUSTOM_EFFECT_HALF_HEALTH", "Le handicap de résilience a été levé.");
      RemoveArenaMalus(player, "CUSTOM_EFFECT_SPELL_FAILURE", "Le handicap d'échec des sorts a été levé.");
    }
    private static void ApplyDissipationMalus(PlayerSystem.Player player)
    {
      foreach (NwCreature summon in player.oid.Henchmen)
      {
        summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
        summon.Destroy();
      }

      player.oid.ApplyEffect(EffectDuration.Instant, Effect.DispelMagicAll(100));
      player.oid.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfDispelDisjunction));
    }
    private static void ApplyNoMalus(PlayerSystem.Player player)
    {
      player.oid.SendServerMessage("Quelle chance, aucun handicap !", Color.PINK);
    }
    private static void ApplyNoMalusAndHeal(PlayerSystem.Player player)
    {
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingG));
      player.oid.ApplyEffect(EffectDuration.Instant, Effect.Heal(player.oid.MaxHP / 2));
      player.oid.SendServerMessage("Quelle chance, aucun handicap et des soins gratuits !", Color.PINK);
    }
    public static RoundCreatures[] GetNormalEncounters(Difficulty difficulty) {
    switch(difficulty)
    {
        default: throw new Exception($"PvE Arena: Invalid normal encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "goblinfooder", "goblinfooder", "gobelinfrondeur", "gobelinfrondeur", "gobelinfourbe" },
            points: 1  
          ),
          new RoundCreatures(
            resrefs: new string[] { "bat_sewer", "bat_sewer", "bat_sewer", "bat_sewer" },
            points: 1
          ),
          new RoundCreatures(
            resrefs: new string[] { "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect", "rat_sewer_infect"   },
            points: 2
          ),
          new RoundCreatures(
            resrefs: new string[] { "dog_sewer_starve", "dog_sewer_starve", "dog_sewer_starve" },
            points: 5
          ),
          new RoundCreatures(
            resrefs: new string[] { "rat_meca", "rat_meca", "rat_meca", "rat_meca" },
            points: 6
          ),
          new RoundCreatures(
            resrefs: new string[] { "crab_meca", "crab_meca", "pingu_meca", "pingu_meca" },
            points: 4
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }

    public static RoundCreatures[] GetEliteEncounters(Difficulty difficulty)
    {
      switch (difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid elite encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "cutter_meca", "cutter_meca" },
            points: 10
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }

    public static RoundCreatures[] GetBossEncounters(Difficulty difficulty)
    {
      switch (difficulty)
      {
        default: throw new Exception($"PvE Arena: Invalid boss encounter for difficulty={difficulty}");

        case Difficulty.Level1: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "rat_meca", "rat_meca", "rat_meca", "dog_meca_defect" },
            points: 20
          ),
        };

        case Difficulty.Level2: return new RoundCreatures[]
        {
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
          new RoundCreatures(
            resrefs: new string[] { "xxx", "xxx" },
            points: 666
          ),
        };
      }
    }
  }
}
