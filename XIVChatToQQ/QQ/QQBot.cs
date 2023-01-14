using Dalamud.Logging;
using ECommons.DalamudServices;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using QQ.Function;
using System;
using Newtonsoft.Json;
using XIVChatToQQ;
using XIVChatToQQ.XIV;

namespace QQ;

public class QQBot
{

  private static Bot _bot = null!;
  private static string Status = "";

  internal Configuration Configuration { get; set; }

  public QQBot(Configuration configuration)
  {
    Configuration = configuration;
    _bot = BotFather.Create(GetConfig(), GetDevice(), GetKeyStore());
    // Print the log
    _bot.OnLog += (_, e) => PluginLog.Log(e.EventMessage);
    // Handle the captcha
    _bot.OnCaptcha += (s, e) =>
    {
      Svc.Chat.Print("验证");
      Verification verification = new Verification(s, e);
      Plugin.WindowSystem.AddWindow(verification);
      verification.IsOpen = true;
    };
    _bot.OnFriendMessage += Friend.OnFriendMessage;
    // Update the keystore
    if (_bot.Login() != null)
    {
      Svc.Chat.Print("Login succeeded");
      UpdateKeystore(_bot.KeyStore);
    }
  }
  /// <summary>
  /// Get bot config
  /// </summary>
  /// <returns></returns>
  private static BotConfig GetConfig()
  {
    return new BotConfig
    {
      EnableAudio = true,
      TryReconnect = true,
      HighwayChunkSize = 8192,
    };
  }
  /// <summary>
  /// Load or create device 
  /// </summary>
  /// <returns></returns>
  private BotDevice? GetDevice()
  {
    // Read the device from config
    if (Configuration.deviceJson != "")
    {
      return JsonConvert.DeserializeObject<BotDevice>(Configuration.deviceJson);
    }

    // Create new one
    var device = BotDevice.Default();
    {
      var deviceJson = JsonConvert.SerializeObject(device);
      Configuration.deviceJson = deviceJson;
    }

    return device;
  }

  /// <summary>
  /// Load or create keystore
  /// </summary>
  /// <returns></returns>
  private BotKeyStore? GetKeyStore()
  {
    // Read the device from config
    String account = Configuration.account;
    String password = Configuration.password;
    String keystore = Configuration.keystore;

    BotKeyStore newBotKeyStore = new(account, password);


    if (Configuration != null && Configuration.keystore != null && Configuration.keystore != "")
    {
      BotKeyStore oldBotKeyStore = JsonConvert.DeserializeObject<BotKeyStore>(Configuration.keystore);
      if (oldBotKeyStore != null && oldBotKeyStore.Account.Uin == newBotKeyStore.Account.Uin)
      {
        return JsonConvert.DeserializeObject<BotKeyStore>(Configuration.keystore);
      }

    }
    Svc.Chat.Print("账号:" + account + "密码" + password);
    return UpdateKeystore(newBotKeyStore);
  }

  /// <summary>
  /// Update keystore
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  private BotKeyStore UpdateKeystore(BotKeyStore key)
  {
    var keystore = JsonConvert.SerializeObject(key);
    Configuration.keystore = keystore;
    return key;
  }

  public static void SendFriendMessage(String message, string target)
  {
    Friend.SendFriendMessage(_bot, message, target);
  }
  public static void Logout()
  {
    _bot.Logout();
  }
  public static string getStatus()
  {
    if (_bot.GetOnlineStatus() != OnlineStatusEvent.Type.Offline)
    {
      Status = "����";
    }
    else
    {
      Status = "������";
    }
    return Status;
  }
  public static bool isOnline()
  {
    if (_bot != null)
    {
      return _bot.IsOnline();
    }
    else
    {
      return false;
    }
  }
}