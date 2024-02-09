#pragma once

#include "ComponentsCommon.h"

namespace Makeshift::Transform
{
	DEFINE_TYPED_ID(TransformId);

	struct InitInfo
	{
		f32 Position[3]{};
		f32 Rotation[4]{};
		f32 Scale[3]{ 1.0f, 1.0f, 1.0f };
	};

	TransformId CreateTransform(const InitInfo&, GameEntity::EntityId entityId);
	void RemoveTransform(TransformId id);
}
