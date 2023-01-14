using Dalamud.Configuration;
using System;

namespace XIVChatToQQ
{
  [Serializable]
  public class Configuration : IPluginConfiguration
  {
    public int Version { get; set; } = 1;

    public string account = "";

    public string password = "";

    public string targetFriend = "";

    public string keystore = "";

    public string deviceJson = "";
  }
}