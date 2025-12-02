using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AezshmaStreamerBotLib.vban;
using Streamer.bot.Plugin.Interface;
using Streamer.bot.Plugin.Interface.Model;

namespace AezshmaStreamerBotLib {
    public static class AezshmaLib {
        private const string DebugMessagePrefix = "[AEZ-DEBUG]";

        private static readonly Dictionary<Settings, string> SettingNames = new Dictionary<Settings, string> {
            { Settings.TwitchSetRandomStreamTitle, "aez_StreamSetRandomTitle" },
            { Settings.TwitchRandomStreamTitleFile, "aez_StreamRandomTitleFile" }
        };

        private static readonly Random Random = new Random();
        private static readonly object SyncLock = new object();

        public static bool TwitchSetRandomStreamTitle(CPHInlineBase bot, bool isIncludeDate) {
            TwitchUserInfoEx userInfoEx = bot.CPH.TwitchGetExtendedUserInfoByLogin("aezshma");
            string existingTitle = userInfoEx.ChannelTitle;
            string[] titles = File.ReadAllLines(bot.CPH.GetGlobalVar<string>(SettingNames[Settings.TwitchRandomStreamTitleFile]));

            string title;
            do {
                lock (SyncLock) {
                    title = titles[Random.Next(0, titles.Length)];
                }
            } while (existingTitle.Contains(title));

            if (isIncludeDate) {
                title = $"{title} [{DateTime.Now:MM/dd/yyyy}]";
            }

            return bot.CPH.SetChannelTitle(title);
        }

        public static bool SendVbanCommand(CPHInlineBase bot, string ipAddress, int port, string command) {
            VbanClient client = new VbanClient(ipAddress, port);
            try {
                client.SendCommand(new VbanTextCommand(command));
                return true;
            } catch (Exception exception) {
                bot.CPH.LogError($"{DebugMessagePrefix} {exception.Message}");
                return false;
            } finally {
                client.Dispose();
            }
        }

        public static string SendVbanCommandWithResponse(CPHInlineBase bot, string ipAddress, int port, string command) {
            VbanClient client = new VbanClient(ipAddress, port);
            try {
                byte[] responseBytes = client.SendCommand(new VbanTextCommand(command));
                VbanTextCommand response = (VbanTextCommand)VbanPacket.FromBytes(responseBytes);
                return response.GetCommandText();
            } catch (Exception exception) {
                bot.CPH.LogError($"{DebugMessagePrefix} {exception.Message}");
                return null;
            } finally {
                client.Dispose();
            }
        }

        public static bool CommandsCommand(CPHInlineBase bot) {
            const int maxMessageLength = 495;
            if (!bot.CPH.TryGetArg("targetUserName", out string targetUserName))
                return _ReportError(bot, "Cannot find targetUserName");
            if (!bot.CPH.TryGetArg("commandsList", out List<string> commandsList))
                return _ReportError(bot, "Cannot find commandsList");

            List<string> messages = new List<string>();
            StringBuilder messageBuilder = new StringBuilder();

            int processed = 1;

            messageBuilder.Append($"Commands (@{targetUserName}): ");
            foreach (string command in commandsList) {
                bool addComma = processed < commandsList.Count;
                string commandString = addComma ? $"{command}, " : command;

                if (messageBuilder.Length + commandString.Length <= maxMessageLength) {
                    messageBuilder.Append(commandString);
                    if (processed >= commandsList.Count) {
                        messages.Add(messageBuilder.ToString());
                    }
                } else {
                    messages.Add(messageBuilder.ToString());
                    messageBuilder.Clear();
                    messageBuilder.Append(commandString);
                }

                processed += 1;
            }

            for (int i = 1; i <= messages.Count; i++) {
                string message = messages[i - 1];
                bot.CPH.SendMessage(message);
            }

            return true;
        }

        public static bool Test(CPHInlineBase bot) {
            bot.CPH.SendMessage("AezshmaLib.test() invoked");
            return true;
        }

        private static bool _ReportError(CPHInlineBase bot, string errorMessage = "") {
            if (bot.CPH.TryGetArg("actionName", out string actionName)) {
                string chatMessage =
                    $"@Aezshma some shit broke executing the {actionName} action. Better fix it!";
                if (errorMessage != "") {
                    chatMessage += $" Error: {errorMessage}";
                }

                bot.CPH.SendMessage(chatMessage);
            } else {
                bot.CPH.SendMessage("@Aezshma some shit broke *very badly* executing a command... check the logs!");
            }

            return false;
        }

        private enum Settings {
            TwitchSetRandomStreamTitle,
            TwitchRandomStreamTitleFile
        }
    }
}