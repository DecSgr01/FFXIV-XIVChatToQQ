using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using System.Numerics;

namespace XIVChatToQQ.XIV
{
   
    internal class Verification : Window
    {
        public string smsCode = "";
        public string SliderTicket = "";
        private readonly Bot Bot;
        private readonly CaptchaEvent CaptchaEvent;
        public Verification(Bot s, CaptchaEvent e) : base("Verification",ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar)
        {
            IsOpen = false;
            this.Size = new Vector2(350, 200);
            Bot = s;
            CaptchaEvent = e;
            switch (CaptchaEvent.Type)
            {
                case CaptchaEvent.CaptchaType.Sms:
                    Svc.Chat.Print("手机:"+ CaptchaEvent.Phone);
                    break;
                case CaptchaEvent.CaptchaType.Slider:
                    Svc.Chat.Print("链接:" + CaptchaEvent.SliderUrl);
                    break;
                default:
                case CaptchaEvent.CaptchaType.Unknown:
                    break;
            }
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public override void Draw()
        {
            if (!IsOpen)
            {
                return;
            }
            ImGui.BeginChild("", new Vector2(0, 220));
            switch (CaptchaEvent.Type)
            {
                case CaptchaEvent.CaptchaType.Sms:
                    ImGui.Text("请输入短信验证码");
                    ImGui.Text("手机:" + CaptchaEvent.Phone);
                    ImGui.InputText(":验证码", ref smsCode, 6);
                    if (ImGui.Button("确定"))
                    {
                        Bot.SubmitSmsCode(smsCode);
                        Close();
                        Plugin.WindowSystem.RemoveWindow(this);
                    }
                    break;
                case CaptchaEvent.CaptchaType.Slider:
                    ImGui.Text("请从游戏输入框复制链接后在浏览器打开进行验证");
                    ImGui.Text("验证完成后将响应复制到此输入框点击确定");
                    ImGui.InputText(":响应", ref SliderTicket, 1024);
                    if (ImGui.Button("确定"))
                    {
                        Bot.SubmitSliderTicket(SliderTicket);
                        Close();
                        Plugin.WindowSystem.RemoveWindow(this);
                    }
                    break;
                default:
                case CaptchaEvent.CaptchaType.Unknown:
                    break;
            }
            ImGui.EndChild();
            ImGui.End();
        }
    }
}