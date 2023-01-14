using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ImGuiNET;
using QQ;
using System.Numerics;
using XIVChatToQQ;

namespace XIV
{
  internal class XIVChatToQQConfigWindow : Window
  {
    public Configuration Configuration { get; }
    public XIVChatToQQConfigWindow(Configuration configuration) : base("XIVChatToQQ", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar)
    {
      Size = new Vector2(400, 260);
      Configuration = configuration;
    }
    public void Open()
    {
      this.IsOpen = true;
    }
    public void Close()
    {
      this.IsOpen = false;
    }

    public override void Draw()
    {
      if (!this.IsOpen)
      {
        return;
      }
      ImGui.BeginChild("", new Vector2(0, 220));
      ImGui.InputText(":账号", ref Configuration.account, 12);
      ImGui.InputText(":密码", ref Configuration.password, 30);
      ImGui.InputText(":目标好友", ref Configuration.targetFriend, 12);
      if (ImGui.Button("登录"))
      {
        Close();
        new QQBot(Configuration);
        Svc.PluginInterface.SavePluginConfig(Configuration);
        PluginLog.Log("Settings saved.");
      }
      ImGui.Separator();
      if (ImGui.Button("保存并关闭"))
      {
        this.Close();
        Svc.PluginInterface.SavePluginConfig(Configuration);
        PluginLog.Log("Settings saved.");
      }
      ImGui.EndChild();
      ImGui.End();

    }
  }
}
