#pragma once

#include "ComponentsCommon.h"
#include "EngineAPI/TransformComponent.h"

namespace Makeshift
{
#define INIT_INFO(component) namespace component { struct InitInfo; }

	INIT_INFO(Transform);
	INIT_INFO(Script)

#undef INIT_INFO

	namespace GameEntity
	{
		struct EntityInfo
		{
			Transform::InitInfo* Transform{ nullptr };
			Script::InitInfo* Script{ nullptr };
		};

		Entity Create(const EntityInfo& info);
		void Remove(EntityId id);
		bool IsAlive(EntityId id);
	}
}
