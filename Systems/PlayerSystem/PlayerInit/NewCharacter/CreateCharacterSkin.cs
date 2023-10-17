using System.Threading.Tasks;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private async void CreateCharacterSkin()
      {
        if (oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin) is not null)
          return;

        NwItem pcSkin = await NwItem.Create("peaudejoueur", oid.LoginCreature);
        pcSkin.Name = $"Propriétés de {oid.LoginCreature.Name}";
        oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
      }
    }
  }
}
