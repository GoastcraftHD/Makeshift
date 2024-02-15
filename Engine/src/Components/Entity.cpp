#include "Entity.h"
#include "Transform.h"

namespace Makeshift::GameEntity
{
	namespace
	{
		Util::vector<Transform::Component> Transforms;

		Util::vector<Id::GenerationType> Generations;
		Util::deque<EntityId> FreeIds;
	}

	Entity CreateGameEntity(const EntityInfo& info)
	{
		assert(info.Transform);
		if (!info.Transform)
		{
			return Entity{};
		}

		EntityId id;

		if (FreeIds.size() > Id::MinDeletedElements)
		{
			id = FreeIds.front();
			assert(!IsAlive(Entity{ id }));

			FreeIds.pop_front();
			id = EntityId{ Id::NewGeneration(id) };
			++Generations[Id::GetIndex(id)];
		}
		else
		{
			id = EntityId{ (Id::IdType)Generations.size() };
			Generations.push_back(0);

			Transforms.emplace_back();
		}

		const Entity newEntity{ id };
		const Id::IdType index = Id::GetIndex(id);

		assert(!Transforms[index].IsValid());
		Transforms[index] = Transform::CreateTransform(*info.Transform, newEntity);

		if (!Transforms[index].IsValid())
		{
			return {};
		}

		return newEntity;
	}

	void RemoveGameEntity(Entity entity)
	{
		const EntityId id = entity.GetId();
		const Id::IdType index = Id::GetIndex(id);

		assert(IsAlive(entity));

		if (IsAlive(entity))
		{
			Transform::RemoveTransform(Transforms[index]);
			Transforms[index] = Transform::Component{};
			FreeIds.push_back(id);
		}
	}

	bool IsAlive(Entity entity)
	{
		assert(entity.IsValid());

		const EntityId id = entity.GetId();
		const Id::IdType index = Id::GetIndex(id);

		assert(index < Generations.size());
		assert(Generations[index] == Id::GetGeneration(id));

		return Generations[index] == Id::GetGeneration(id) && Transforms[index].IsValid();
	}

	Transform::Component Entity::Transform() const
	{
		assert(IsAlive(*this));
		const Id::IdType index = Id::GetIndex(m_Id);
		return Transforms[index];
	}
}
