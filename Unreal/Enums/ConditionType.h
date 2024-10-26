#pragma once

#include "CoreMinimal.h"
#include "ConditionType.generated.h"

UENUM(BlueprintType)
enum class EConditionType : uint8
{
    None UMETA(DisplayName = "None"),
    Burning UMETA(DisplayName = "Burning"),
    Bleeding UMETA(DisplayName = "Bleeding"),
    Electrified UMETA(DisplayName = "Electrified"),
    Chilling UMETA(DisplayName = "Chilling"),
    Poisoned UMETA(DisplayName = "Poisoned"),
    Healing UMETA(DisplayName = "Healing"),
    Stunned UMETA(DisplayName = "Stunned"),
    Slowed UMETA(DisplayName = "Slowed"),
    Snared UMETA(DisplayName = "Snared"),
    Frozen UMETA(DisplayName = "Frozen"),
    Feared UMETA(DisplayName = "Feared"),
    Cursed UMETA(DisplayName = "Cursed"),
    Silenced UMETA(DisplayName = "Silenced"),
    Confused UMETA(DisplayName = "Confused"),
    Paralyzed UMETA(DisplayName = "Paralyzed"),
    Blinded UMETA(DisplayName = "Blinded"),
    Weakened UMETA(DisplayName = "Weakened"),
    Invulnerable UMETA(DisplayName = "Invulnerable"),
    Fortified UMETA(DisplayName = "Fortified"),
    Regenerating UMETA(DisplayName = "Regenerating"),
    Shielded UMETA(DisplayName = "Shielded"),
    Stealthed UMETA(DisplayName = "Stealthed"),
    Energized UMETA(DisplayName = "Energized"),
    Berserk UMETA(DisplayName = "Berserk"),
    Enraged UMETA(DisplayName = "Enraged"),
    Taunted UMETA(DisplayName = "Taunted"),
    Pacified UMETA(DisplayName = "Pacified"),
    Petrified UMETA(DisplayName = "Petrified"),
    Empowered UMETA(DisplayName = "Empowered"),
    Protected UMETA(DisplayName = "Protected"),
    Crippled UMETA(DisplayName = "Crippled"),
    Fumbling UMETA(DisplayName = "Fumbling"),
    Intoxicated UMETA(DisplayName = "Intoxicated"),
    Hypnotized UMETA(DisplayName = "Hypnotized"),
    Polymorphed UMETA(DisplayName = "Polymorphed"),
    Exhausted UMETA(DisplayName = "Exhausted"),
    Invisible UMETA(DisplayName = "Invisible"),
    Vulnerable UMETA(DisplayName = "Vulnerable"),
    Reflective UMETA(DisplayName = "Reflective"),
    Diseased UMETA(DisplayName = "Diseased"),
    Frenzied UMETA(DisplayName = "Frenzied"),
    Entangled UMETA(DisplayName = "Entangled")
};
