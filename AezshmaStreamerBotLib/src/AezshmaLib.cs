using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AezshmaStreamerBotLib.vban;
using Streamer.bot.Plugin.Interface;

namespace AezshmaStreamerBotLib {
    public static class AezshmaLib {
        private const string DebugMessagePrefix = "[AEZ-DEBUG]";

        public static bool SendVbanTextCommand(CPHInlineBase bot, string ipAddress, string command) {
            try {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress address = IPAddress.Parse(ipAddress);

                VbanTextCommand vbanTextCommand = new VbanTextCommand(command);
                IPEndPoint ep = new IPEndPoint(address, 6980);
                s.SendTo(vbanTextCommand.ToBytes(), ep);

                return true;
            } catch (Exception exception) {
                bot.CPH.LogError($"{DebugMessagePrefix} {exception.Message}");
                return false;
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
    }
}