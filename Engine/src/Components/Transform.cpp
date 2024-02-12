#include "Transform.h"
#include "Entity.h"

namespace Makeshift::Transform
{
	namespace
	{
		Util::vector<Math::V3> Positions;
		Util::vector<Math::V4> Rotations;
		Util::vector<Math::V3> Scales;
	}

	Component CreateTransform(const InitInfo& info, GameEntity::Entity entity)
	{
		assert(entity.IsValid());
		const Id::IdType entityIndex = Id::GetIndex(entity.GetId());

		if (Positions.size() > entityIndex)
		{
			Positions[entityIndex] = Math::V3(info.Position);
			Rotations[entityIndex] = Math::V4(info.Rotation);
			Scales[entityIndex] = Math::V3(info.Scale);
		}
		else
		{
			assert(Positions.size() == entityIndex);
			Positions.emplace_back(info.Position);
			Rotations.emplace_back(info.Rotation);
			Scales.emplace_back(info.Scale);
		}

		return Component(TransformId{ (Id::IdType)Positions.size() - 1 });
	}

	void RemoveTransform(Component component)
	{
		assert(component.IsValid());
	}

	Math::V3 Component::Position() const
	{
		assert(IsValid());
		return Positions[Id::GetIndex(m_Id)];
	}

	Math::V4 Component::Rotation() const
	{
		assert(IsValid());
		return Rotations[Id::GetIndex(m_Id)];
	}

	Math::V3 Component::Scale() const
	{
		assert(IsValid());
		return Scales[Id::GetIndex(m_Id)];
	}
}
