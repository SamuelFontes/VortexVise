﻿using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.GameLogic;

namespace VortexVise.Core.States
{
    /// <summary>
    /// GameState
    /// Store the information generated by the simulation of all the gameplay logic.
    /// This is used to generate the next frame/state.
    /// This is used to draw the entire frame to the screen.
    /// This will be sent over the network by the host to the other players.
    /// </summary>
    public class GameState
    {
        public ulong Tick { get; set; }
        public Guid OwnerId { get; set; }
        public double CurrentTime { get; set; }
        public float Gravity { get; set; }
        public float MatchTimer { get; set; }
        public bool IsRunning { get; set; } = true;
        public MatchStates MatchState { get; set; }
        public List<PlayerState> PlayerStates { get; set; } = [];
        public List<WeaponDropState> WeaponDrops { get; set; } = [];
        public List<DamageHitBoxState> DamageHitBoxes { get; set; } = [];
        /// <summary>
        /// List of animations like player deaths and explosions
        /// </summary>
        public List<AnimationState> Animations { get; set; } = [];
        public List<KillFeedState> KillFeedStates { get; set; } = [];

        /// <summary>
        /// Reset the game state
        /// </summary>
        public void ResetGameState()
        {
            foreach (var playerState in PlayerStates)
            {
                playerState.IsDead = false;
                playerState.Position = GameMatch.PlayerSpawnPoint;
                playerState.Velocity = new(0, 0);
                playerState.WeaponStates.Clear();
                playerState.HookState.IsHookReleased = false;
                playerState.HeathPoints = GameMatch.DefaultPlayerHeathPoints;
                playerState.Animation.Rotation = 0;
                playerState.Animation.State = 0;
                playerState.LastPlayerHitId = Guid.Empty;
                playerState.Stats.Kills = 0;
                playerState.Stats.Deaths = 0;
                playerState.JetPackFuel = GameMatch.DefaultJetPackFuel;
            }
            WeaponLogic.WeaponRotation = 0;
            WeaponDrops.Clear();
            foreach (var spawn in GameMatch.CurrentMap.ItemSpawnPoints)
            {
                var weapon = GameAssets.Gameplay.Weapons.OrderBy(x => Guid.NewGuid()).First();
                var weaponDrop = new WeaponDropState(new WeaponState(weapon, weapon.Ammo, weapon.Ammo, false, weapon.ReloadDelay, 0), spawn.Deserialize());
                WeaponDrops.Add(weaponDrop);
            }
            MatchState = MatchStates.Warmup;
            MatchTimer = 4;
            Tick = 0;
        }
    }
}
