#pragma once

#include "ComponentsCommon.h"
#include "EngineAPI/TransformComponent.h"

namespace Makeshift
{
#define INIT_INFO(component) namespace component { struct InitInfo; }

	INIT_INFO(Transform);

#undef INIT_INFO

	namespace GameEntity
	{
		struct EntityInfo
		{
			Transform::InitInfo* Transform{ nullptr };
		};

		Entity CreateGameEntity(const EntityInfo& info);
		void RemoveGameEntity(Entity entity);
		bool IsAlive(Entity entity);
	}
}
