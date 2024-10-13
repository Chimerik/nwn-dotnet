using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void LuneRadieuse(NwCreature caster)
    {
      if (!caster.ActiveEffects.Any(e => e.Tag == EffectSystem.PolymorphEffectTag))
      {
        caster.ControllingPlayer?.SendServerMessage("Vous devez être sous Forme Sauvage pour faire usage de cette capacité", ColorConstants.Orange);
        caster.SetFeatRemainingUses((Feat)CustomSkill.DruideLuneRadieuse, 0);
        return;
      }

      NwItem weapon = caster.GetItemInSlot(InventorySlot.CreatureLeftWeapon);

      bool activation = false;

      foreach (var ip in weapon.ItemProperties)
      {
        if (ip.Property.PropertyType == ItemPropertyType.DamageBonus && ip.SubType.RowIndex < 5)
        {
          activation = true;
          ItemProperty luneRadiant = ItemProperty.DamageBonus(IPDamageType.Divine, (IPDamageBonus)ip.IntParams[3]);
          luneRadiant.Tag = $"LUNERADIANTE_{ip.SubType.RowIndex}";

          weapon.RemoveItemProperty(ip);
          weapon.AddItemProperty(luneRadiant, EffectDuration.Permanent);

        }
      }

      if (activation)
      {
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Lune Radieuse", StringUtils.gold, true, true);
      }
      else
      {
        foreach (var ip in weapon.ItemProperties)
        {
          if (ip.Tag.Contains("LUNERADIANTE_"))
          {
            ItemProperty luneRadiant = ItemProperty.DamageBonus((IPDamageType)int.Parse(ip.Tag.Split("_")[1]), (IPDamageBonus)ip.IntParams[3]);

            weapon.RemoveItemProperty(ip);
            weapon.AddItemProperty(luneRadiant, EffectDuration.Permanent);
          }
        }
      }
    }
  }
}
