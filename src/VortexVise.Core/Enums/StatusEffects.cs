﻿//----------------------------------------------------------------------------------
// Enumerators Definition
//----------------------------------------------------------------------------------
namespace VortexVise.Core.Enums;

/// <summary>
/// StatusEffects
/// </summary>
public enum StatusEffects
{
    /// <summary>
    /// No status applied
    /// </summary>
    None,
    /// <summary>
    /// Cause slow, if accumulated it will cause freeze, fires will remove this effect.
    /// </summary>
    Cold,
    /// <summary>
    /// If it is wet, it can get stunned longer with eleticity, can remove fire, fire removes this but don't burn, increase cold effect 
    /// </summary>
    Wet,
    /// <summary>
    /// Cause constant damage, removes with water, cold or freeze, can spread to others if stay too close 
    /// </summary>
    Fire,
    /// <summary>
    /// Stuns, if target was wet it doubles stun time
    /// </summary>
    Eletricity,
    /// <summary>
    /// Covered in ice, is stunned, any hit causes double damage but remove this effect, this is only caused by stacking a certain amout of cold
    /// </summary>
    Freezed,
    /// <summary>
    /// Invert player controls
    /// </summary>
    Confusion,
    /// <summary>
    /// Simple stun, any damage should remove this effect
    /// </summary>
    Dizzy,
    /// <summary>
    /// Inverts map horizontally and invert gravity for this entity, dude will fall upwards
    /// </summary>
    GetRotatedIdiot,
    /// <summary>
    /// Deals damage over time, fast
    /// </summary>
    Bleeding,
    /// <summary>
    /// Deals damage over time, slow
    /// </summary>
    Poison,
    /// <summary>
    /// Heal target
    /// </summary>
    Heal,
    /// <summary>
    /// Shrink entity making it and hitbox smaller
    /// </summary>
    Shrink,
    /// <summary>
    /// Make entity and hitbox bigger
    /// </summary>
    Enlarge,
    /// <summary>
    /// Special. Make thing invulnerable to any damage, knockback or status effect changes.
    /// </summary>
    Ivunerable,
};
