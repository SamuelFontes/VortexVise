enum GameMode
{
    DeathMatch = 0, /// Free for all
    TeamDeathMatch, /// Team based death match
    Survival /// Rogue like survival mode
};

enum GameScene /// GameScreen, defines what scene the game is in 
{
    UNKNOWN = -1, /// Used for transitioning screens
    LOGO = 0, /// Game and library logo
    MENU, /// Title screen
    OPTIONS, /// Options screen
    GAMEPLAY, /// Gameplay screen
    ENDING /// Ending screen
};

enum WeaponType /// Weapon type
{
    Pistol,
    SMG,
    Shotgun,
    MeleeBlunt,
    MeleeCut,
    Granade,
    Mine,
    Bazoka,
    Heal,

};

enum StatusEffects /// StatusEffects
{
    None, /// No status applied
    Cold, /// Cause slow, if accumulated it will cause freeze, fires will remove this effect.
    Wet, /// If it is wet, it can get stunned longer with eleticity, can remove fire, fire removes this but don't burn, increase cold effect 
    Fire, /// Cause constant damage, removes with water, cold or freeze, can spread to others if stay too close 
    Eletricity, /// Stuns, if target was wet it doubles stun time
    Freezed, /// Covered in ice, is stunned, any hit causes double damage but remove this effect, this is only caused by stacking a certain amout of cold
    Confusion, /// Invert player controls
    Dizzy, /// Simple stun, any damage should remove this effect
    GetRotatedIdiot, /// Inverts map horizontally and invert gravity for this entity, dude will fall upwards
    Bleeding, /// Deals damage over time, fast
    Poison, /// Deals damage over time, slow
    Heal, /// Heal target
    Shrink, /// Shrink entity making it and hitbox smaller
    Enlarge, /// Make entity and hitbox bigger
    Ivunerable, /// Special. Make thing invulnerable to any damage, knockback or status effect changes.
};


enum MatchStates
{
    Warmup,
    Playing,
    EndScreen,
    Voting,
    Lobby,
};
