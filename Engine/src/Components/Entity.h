#pragma once

#include "ComponentsCommon.h"

namespace Makeshift
{
#define INIT_INFO(component) namespace component { struct InitInfo; }

	INIT_INFO(Transform);

#undef INIT_INFO

	namespace GameEntity
	{
		struct EntityInfo
		{

		};

		EntityId CreateGameEntity(const EntityInfo& info);
		void RemoveGameEntity(EntityId id);
		bool IsAlive(EntityId id);
	}
}
