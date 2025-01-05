using VortexVise.Core.Enums;
using VortexVise.Core.Models;

namespace VortexVise.Core.GameGlobals
{
    /// <summary>
    /// Game attributes.
    /// </summary>
    public static class GameCore
    {
        /// <summary>
        /// Defines tickrate the game and server runs at. This tick defines the minimal amount of game simulations per second that need to occur even if the performs badly and cause low FPS. This will ensure a consistent game behavior independently of low performance.
        /// </summary>
        public static int GameTickRate { get; private set; } = 60;
        /// <summary>
        /// Defines internal game rendering resolution.
        /// </summary>
        public static int GameScreenWidth { get; set; } = 960;
        /// <summary>
        /// Defines internal game rendering resolution.
        /// </summary>
        public static int GameScreenHeight { get; set; } = 540;
        public static string GameName { get; set; } = "Vortex Vise";
        /// <summary>
        /// Global Font Size
        /// </summary>
        public static int MenuFontSize { get; set; } = 32;
        /// <summary>
        /// If true the game will exit.
        /// </summary>
        public static bool GameShouldClose { get; set; } = false;
        /// <summary>
        /// This is used to indicate if the game is running in dedicated server mode.
        /// </summary>
        public static bool IsServer { get; set; } = false;
        /// <summary>
        /// Define if its a network game.
        /// </summary>
        public static bool IsNetworkGame { get; set; } = false;
        /// <summary>
        /// Player profile
        /// </summary>
        public static PlayerProfile PlayerOneProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerOne", Gamepad = GamepadSlot.Disconnected };
        /// <summary>
        /// Player profile
        /// </summary>
        public static PlayerProfile PlayerTwoProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerTwo", Gamepad = GamepadSlot.Disconnected };
        /// <summary>
        /// Player profile
        /// </summary>
        public static PlayerProfile PlayerThreeProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerThree", Gamepad = GamepadSlot.Disconnected };
        /// <summary>
        /// Player profile
        /// </summary>
        public static PlayerProfile PlayerFourProfile { get; set; } = new() { Id = Guid.NewGuid(), Name = "PlayerFour", Gamepad = GamepadSlot.Disconnected };
    }
}
