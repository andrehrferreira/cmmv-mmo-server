
#include "Engine/Engine.h"
#include "GenericPlatform/GenericPlatformMisc.h"

ServerSubsystem::ServerSubsystem() :
	bIsManuallyLaunched(false),
	bAutoClientTravel(true),
	bAutoServerClose(true)
{

}

void ServerSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	
}

void ServerSubsystem::Deinitialize()
{

}