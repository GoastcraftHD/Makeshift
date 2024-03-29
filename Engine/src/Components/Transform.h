#pragma once

#include "ComponentsCommon.h"

namespace Makeshift::Transform
{
	struct InitInfo
	{
		f32 Position[3]{};
		f32 Rotation[4]{};
		f32 Scale[3]{ 1.0f, 1.0f, 1.0f };
	};

	Component Create(InitInfo info, GameEntity::Entity entity);
	void Remove(Component component);
}
