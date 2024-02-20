#pragma once

#include "ComponentsCommon.h"

namespace Makeshift::Script
{
	struct InitInfo
	{
		Detail::ScriptCreator ScripCreator;
	};

	Component Create(InitInfo info, GameEntity::Entity entity);
	void Remove(Component component);
}
