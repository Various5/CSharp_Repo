using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

class Program
{
    static Task Main(string[] args)
    {
        return new Program().MainAsync();
    }

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    private Program()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
        });

        _commands = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
        });

        _client.Log += Log;
        _commands.Log += Log;



        _client.UserJoined += AnnounceUserJoined;
        _client.MessageReceived += HandleCommandAsync;
    }

    private Task AnnounceUserJoined(SocketGuildUser user)
    {
        var channel = _client.GetChannel(938325476534009888) as SocketTextChannel;
        channel.SendMessageAsync($"Welcome to the server, {user.Mention}!");
        return Task.CompletedTask;
    }

    private Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();

        return Task.CompletedTask;
    }



    private async Task MainAsync()
    {
        await _client.LoginAsync(TokenType.Bot, "MTA3NTMzNDkzNzQzNzI5NDY1Mg.GtHPxr.3cBR-Ou9AXxjE27TaV6s-kcaEqUtC_Lob3Vbec");
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var msg = arg as SocketUserMessage;
        if (msg == null) return;

        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

        // Check if the message is in the provided channel ID.
        var channel = msg.Channel as SocketTextChannel;
        if (channel == null || channel.Id != 938325476534009888) return;

        // Check if the message contains the word "hello bot".
        if (msg.Content.Contains("hello bot"))
        {
            // Send the response to the same channel where the message was received.
            await channel.SendMessageAsync("Hello human!");
        }
    }
}
