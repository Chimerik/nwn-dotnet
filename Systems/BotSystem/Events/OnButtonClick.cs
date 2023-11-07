using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace NWN.Systems
{
  public static partial class Bot
  {
    private static async Task OnButtonClick(SocketMessageComponent component)
    {
      try
      {
        string[] splitCommand = component.Data.CustomId.Split("/");
        switch (splitCommand[0])
        {
          case "send":

            foreach (var file in Directory.GetFiles($"../../../PortraitToValidate/{splitCommand[1]}", $"{splitCommand[2]}"))
            {
              string resName = (Portraits2da.portraitsTable.Count + 1).ToString();

              using var img = await Image.LoadAsync(file);
              img.Mutate(c => c.Resize(64, 128));
              img.Mutate(c => c.Rotate(RotateMode.Rotate180));
              await img.SaveAsTgaAsync($"../../../.local/share/Neverwinter Nights/development/po_{resName}m.tga");

              await File.AppendAllTextAsync($"../../../.local/share/Neverwinter Nights/development/portraits.2da", $"\n{resName} {resName} **** **** **** **** **** {splitCommand[1]}");
              File.Delete(file);
            }

            await component.UpdateAsync(x =>
            {
              x.Content = $"{splitCommand[1]} - Demande de validation de portrait ({splitCommand[2]}) => Validée par {component.User.Username}";
              x.Components = new ComponentBuilder().Build();
            });

            break;

          case "delete":

            foreach (var file in Directory.GetFiles($"/home/chim/PortraitToValidate/{splitCommand[1]}", $"{splitCommand[2]}"))
              File.Delete(file);

            await component.UpdateAsync(x =>
            {
              x.Content = $"{splitCommand[1]} - Demande de validation de portrait ({splitCommand[2]}) => Rejetée par {component.User.Username}";
              x.Components = new ComponentBuilder().Build();
            });

            break;
        }
      }
      catch(Exception e)
      {
        ModuleSystem.Log.Info(e.GetType() + "\n" + e.Message + "\n" + e.StackTrace);
      }
    }
  }
}
