#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif

#include "Common/CommonHeaders.h"
#include "Common/Id.h"
#include "Components/Entity.h"
#include "Components/Transform.h"

using namespace Makeshift;

namespace
{
	struct TransformComponent
	{
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];

		Transform::InitInfo ToInitInfo()
		{
			using namespace DirectX;
			Transform::InitInfo info{};
			memcpy(&info.Position[0], &position[0], sizeof(f32) * _countof(position));
			memcpy(&info.Scale[0], &scale[0], sizeof(f32) * _countof(scale));

			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quaternion = XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot));
			XMFLOAT4A rotQuaternion{};
			XMStoreFloat4A(&rotQuaternion, quaternion);
			memcpy(&info.Rotation[0], &rotQuaternion.x, sizeof(f32) * _countof(info.Rotation));

			return info;
		}
	};

	struct GameEntityDescriptor
	{
		TransformComponent transform;
	};

	GameEntity::Entity EntityFromId(Id::IdType id)
	{
		return GameEntity::Entity{ GameEntity::EntityId{id} };
	}
}

EDITOR_INTERFACE Id::IdType CreateGameEntity(GameEntityDescriptor* descriptor)
{
	assert(descriptor);
	GameEntityDescriptor& desc = *descriptor;
	Transform::InitInfo transformInfo = desc.transform.ToInitInfo();
	GameEntity::EntityInfo entityInfo{ &transformInfo };

	return GameEntity::Create(entityInfo).GetId();
}

EDITOR_INTERFACE void RemoveGameEntity(Id::IdType id)
{
	assert(Id::IsValid(id));
	GameEntity::Remove(GameEntity::EntityId{ id });
}
