using System.Threading.Tasks;
namespace NWN.Systems
{
  public abstract partial class Learnable
  {
    private async Task<string> GetAsyncDescription()
    {
      return await StringUtils.DownloadGoogleDoc(_description.Replace("google", ""));
    }
  }
}
