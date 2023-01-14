using ECommons.DalamudServices;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using XIVChatToQQ;
using XIVChatToQQ.Helper;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local

namespace QQ.Function;

public static class Friend
{
  /// <summary>
  /// On group message
  /// </summary>
  /// <param name="bot"></param>
  /// <param name="friend"></param>
  internal static void OnFriendMessage(Bot bot, FriendMessageEvent friend)
  {
    string chat = friend.Chain.ToString();
    JObject j = new JObject();
    j.Add("command", chat);
    HttpHelper.Request(j.ToString(), "http://localhost:37984/command");
  }
  public static async void SendFriendMessage(Bot bot, String message, String target)
  {
    if (target != "" && bot != null)
    {
      if (uint.TryParse(target, out uint uintTarget))
      {
        await bot.SendFriendMessage(uintTarget, message);
      }
    }
    else
    {
      Svc.Chat.Print("没有登陆?");
    }
  }
}