using Streamer.bot.Plugin.Interface;

namespace AezshmaStreamerBotLib
{
    public class AezshmaLib
    {
        public bool Test(CPHInlineBase bot)
        {
            bot.CPH.SendMessage("AezshmaLib.test() invoked");
            return true;
        }
    }
}