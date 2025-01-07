using Steamworks;
using VortexVise.Core;
using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Models;
using VortexVise.Core.Scenes;
using VortexVise.Core.Services;
using VortexVise.Core.Utilities;
using VortexVise.Desktop.Models;
using VortexVise.Desktop.Services;
using ZeroElectric.Vinculum;

// ...

try
{
    SteamClient.Init(480);
    foreach (var friend in SteamFriends.GetFriends())
    {
        Console.WriteLine($"{friend.Id}: {friend.Name}");
        Console.WriteLine($"{friend.IsOnline} / {friend.SteamLevel}");

    }
}
catch (System.Exception e)
{
    // Couldn't init for some reason (steam is closed etc)
}
SteamClient.Shutdown();

// Run game 
GameServices services = new GameServices(new AssetService(), new CollisionService(), new InputService(), new RendererService(), new SoundService(), new WindowService());
Game game = new Game(services);
game.Init<FontAsset,MusicAsset,SoundAsset,TextureAsset>();
// Main Game Loop
while (!GameCore.GameShouldClose)
{
    game.Update<PlayerCamera>();
}
game.Unload();
