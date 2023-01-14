
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using QQ;
using System;
using XIV;
using Dalamud.Game.ClientState;

namespace XIVChatToQQ
{
  public sealed class Plugin : IDalamudPlugin
  {
    internal static Plugin Instance { get; private set; } = null!;
    public string Name => "XIVChatToQQ";
    [PluginService]
    internal DalamudPluginInterface pluginInterface { get; init; }
    [PluginService]
    internal CommandManager CommandManager { get; init; }
    [PluginService]
    internal ClientState clientState { get; init; }
    private static bool _loggedIn = false;
    private const string commandName = "/test";
    internal Configuration Config { get; }

    public static WindowSystem WindowSystem = new("XIVChatToQQ");
    public Plugin()
    {
      Instance = this;
      ECommons.ECommons.Init(pluginInterface, this);
      Config = pluginInterface!.GetPluginConfig() as Configuration ?? new Configuration();
      Svc.Chat.ChatMessage += Chat_ChatMessage;
      this.clientState!.Login += Login;
      this.clientState!.Logout += Logout;
      if (clientState.IsLoggedIn == true)
      {
        _loggedIn = true;
      }
      XIVChatToQQConfigWindow xivChatToQQConfigWindow = new XIVChatToQQConfigWindow(Config);
      WindowSystem.AddWindow(xivChatToQQConfigWindow);

      pluginInterface.UiBuilder.OpenConfigUi += delegate { xivChatToQQConfigWindow.IsOpen = true; };
      pluginInterface.UiBuilder.Draw += WindowSystem.Draw;

      this.CommandManager!.AddHandler(commandName, new CommandInfo(OnCommand)
      {
        HelpMessage = "A useful message to display in /test",
        ShowInHelp = true
      });


      if (this.Config.deviceJson != "" && this.Config.keystore != "" && !QQBot.isOnline())
      {
        new QQBot(this.Config);
      }
    }

    private void Logout(object? sender, EventArgs e)
    {
      _loggedIn = true;
    }

    private void Login(object? sender, EventArgs e)
    {
      _loggedIn = false;
    }
    private void Chat_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
      Message(type, ref sender, ref message);
    }
    private void Message(XivChatType type, ref SeString sender, ref SeString message)
    {
      if (QQBot.isOnline())
      {
        string QQMessage = "";
        switch (type)
        {
          case XivChatType.Say:
            QQMessage = "[说话]:";
            break;
          case XivChatType.Shout:
            QQMessage = "[喊话]:";
            break;
          case XivChatType.FreeCompany:
            QQMessage = "[部队]:";
            break;
          case XivChatType.Yell:
            QQMessage = "[呼喊]:";
            break;
          case XivChatType.TellOutgoing:
            QQMessage = "[私聊]:";
            break;
          default:
            break;
        }
        if (QQMessage != "")
        {
          QQBot.SendFriendMessage(QQMessage + sender + ":" + message.TextValue, this.Config.targetFriend);
        }
      }
    }

    public void Dispose()
    {
      this.CommandManager.RemoveHandler(commandName);
      Svc.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
      QQBot.Logout();
    }

    private void OnCommand(string command, string args)
    {
      QQBot.SendFriendMessage(args, this.Config.targetFriend);
    }

  }

}
