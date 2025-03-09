var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var streamerbotRoot = Argument<string>("streamerbotFolder", null);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        CleanDirectory($"./AezshmaStreamerBotLib/bin/{configuration}");
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetBuild("./AezshmaStreamerBotLib.sln", new DotNetBuildSettings {
            Configuration = configuration,
        });
    });

Task("DeployToStreamerbot")
    .WithCriteria(() => streamerbotRoot != null)
    .IsDependentOn("Build")
    .Does(() => {
        CopyFileToDirectory(
        $"./AezshmaStreamerBotLib/bin/{configuration}/AezshmaStreamerBotLib.dll",
        $"{streamerbotRoot}/dlls");
    });
    
Task("RunStreamerbot")
    .IsDependentOn("DeployToStreamerbot")
    .Does(() => {
        StartProcess($"{streamerbotRoot}/Streamer.bot.exe");
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);