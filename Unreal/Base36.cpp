#include "Base36.h"

int32 UBase36::Base36ToInt(const FString& Value)
{
    return FCString::Strtoi(*Value, nullptr, 36);
}

FString UBase36::IntToBase36(int32 Value)
{
    FString chars = TEXT("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
    FString result = TEXT("");

    if (Value == 0)
        return TEXT("0");

    while (Value > 0) {
        int remainder = Value % 36;
        Value /= 36;
        result += chars[remainder];
    }

    result = result.Reverse();

    return result;
}
