using System.Collections.Generic;
using NWN.Core;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    public class Lootable
    {
      public struct Config
      {
        public Gold? gold { get; set; }
        public List<Item> items { get; set; }

        public Config(List<Item> items, Gold? gold = null)
        {
          this.gold = gold;
          this.items = items;
        }

        public void GenerateLoot(uint oContainer)
        {
          var containerTag = NWScript.GetTag(oContainer);

          if (NWScript.GetHasInventory(oContainer) == 0)
          {
            ThrowException($"Can't GenerateLoot : Object '{containerTag}' has no inventory.");
          }

          if (gold != null)
          {
            gold.GetValueOrDefault().Generate(oContainer);
          }

          GenerateItems(oContainer);
        }

        private void GenerateItems(uint oContainer)
        {
          foreach (var item in items)
          {
            item.Generate(oContainer);
          }
        }
      }

      public struct Gold
      {
        public uint min { get; set; }
        public uint max { get; set; }
        public uint chance { get; set; }

        public Gold(uint max, uint min = 0, uint chance = 100)
        {
          this.min = min;
          this.max = max;
          this.chance = chance;
        }

        public void Generate(uint oContainer, string goldResRef = "nw_it_gold001")
        {
          if (Utils.random.Next(1, 100) <= chance)
          {
            var goldCount = Utils.random.Next((int)min, (int)max);

            if (NWScript.GetObjectType(oContainer) == NWScript.OBJECT_TYPE_CREATURE)
            {
              NWScript.GiveGoldToCreature(oContainer, goldCount);
            }
            else
            {
              NWScript.CreateItemOnObject(goldResRef, oContainer, goldCount);
            }
          }
        }
      }

      public struct Item
      {
        public string chestTag { get; set; }
        public uint count { get; set; }
        public uint chance { get; set; }

        public Item(string chestTag, uint count = 1, uint chance = 100)
        {
          this.chestTag = chestTag;
          this.count = count;
          this.chance = chance;
        }

        public void Generate(uint oContainer)
        {
          List<uint> loots;
          if (chestTagToLootsDic.TryGetValue(chestTag, out loots))
          {
            if (loots.Count > 0)
            {
              for (var i = 0; i < count; i++)
              {
                if (Utils.random.Next(1, 100) <= chance)
                {
                  NWScript.CopyItem(
                      loots[Utils.random.Next(0, loots.Count - 1)],
                      oContainer,
                      1
                  );
                }
              }
            }
          }
          else
          {
            ThrowException($"Invalid chest tag '{chestTag}'");
          }
        }
      }
    }
  }
}
