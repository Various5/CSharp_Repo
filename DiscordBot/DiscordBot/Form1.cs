using System;
using System.Windows.Forms;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace DiscordBot
{
    public partial class Form1 : Form
    {
        private DiscordSocketClient _client;
        private IGuild _guild;
        private ITextChannel _channel;

        public Form1()
        {
            InitializeComponent();
        }

        private Task Log(LogMessage msg)
        {
            rtxt_debug.AppendText($"{msg.Source}: {msg.Message}\n");
            return Task.CompletedTask;
        }

        private async void btn_startbot_Click_1(object sender, EventArgs e)
        {
            rtxt_debug.AppendText("Bot started\n");
            btn_startbot.Enabled = false;
            btn_stopbot.Enabled = true;

            _client = new DiscordSocketClient();
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, txtKey1.Text);
            await _client.StartAsync();

            _client.Ready += async () =>
            {
                _guild = _client.GetGuild(ulong.Parse(txtKey2.Text));
                _channel = await _guild.GetTextChannelAsync(ulong.Parse(txtKey3.Text));

                rtxt_debug.AppendText("Bot is online and ready!\n");
            };
        }

        private async void btn_stopbot_Click_1(object sender, EventArgs e)
        {
            rtxt_debug.AppendText("Bot stopped\n");
            btn_startbot.Enabled = true;
            btn_stopbot.Enabled = false;

            await _client.LogoutAsync();
            await _client.StopAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
