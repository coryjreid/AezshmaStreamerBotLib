using System.Collections.Generic;
using System.Text;
using Streamer.bot.Plugin.Interface;

namespace AezshmaStreamerBotLib {
    public class AezshmaLib {
        private const string DebugMessagePrefix = "[AEZ-DEBUG]";
        
        public bool CommandsCommand(CPHInlineBase bot) {
            const int maxMessageLength = 495;
            if (!bot.CPH.TryGetArg("targetUserName", out string targetUserName)) return _ReportError(bot, "Cannot find targetUserName");
            if (!bot.CPH.TryGetArg("commandsList", out List<string> commandsList)) return _ReportError(bot, "Cannot find commandsList");

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
                // bot.CPH.LogError($"[AEZ-DEBUG]: Total Messages: {messages.Count}");
                // bot.CPH.LogError($"[AEZ-DEBUG]: Total Commands: {commandsList.Count}");
                // bot.CPH.LogError($"[AEZ-DEBUG]: Total Processed Commands: {processed}");
                // bot.CPH.LogError($"[AEZ-DEBUG]: Message ({i}/{messages.Count}) {message}");
                bot.CPH.SendMessage(message);
            }

            return true;
        }

        public bool Test(CPHInlineBase bot) {
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