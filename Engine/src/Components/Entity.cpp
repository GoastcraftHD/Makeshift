#include "Entity.h"
#include "Transform.h"
#include "Script.h"

namespace Makeshift::GameEntity
{
	namespace
	{
		Util::vector<Transform::Component> Transforms;
		Util::vector<Script::Component> Scripts;

		Util::vector<Id::GenerationType> Generations;
		Util::deque<EntityId> FreeIds;
	}

	Entity Create(const EntityInfo& info)
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
			assert(!IsAlive(id ));

			FreeIds.pop_front();
			id = EntityId{ Id::NewGeneration(id) };
			++Generations[Id::GetIndex(id)];
		}
		else
		{
			id = EntityId{ (Id::IdType)Generations.size() };
			Generations.push_back(0);

			Transforms.emplace_back();
			Scripts.emplace_back();
		}

		const Entity newEntity{ id };
		const Id::IdType index = Id::GetIndex(id);

		assert(!Transforms[index].IsValid());
		Transforms[index] = Transform::Create(*info.Transform, newEntity);

		if (!Transforms[index].IsValid())
		{
			return {};
		}

		if (info.Script && info.Script->ScripCreator)
		{
			assert(!Scripts[index].IsValid());
			Scripts[index] = Script::Create(*info.Script, newEntity);
			assert(Scripts[index].IsValid());
		}

		return newEntity;
	}

	void Remove(EntityId id)
	{
		const Id::IdType index = Id::GetIndex(id);

		assert(IsAlive(id));

		if (Scripts[index].IsValid())
		{
			Script::Remove(Scripts[index]);
			Scripts[index] = {};
		}

		Transform::Remove(Transforms[index]);
		Transforms[index] = {};
		FreeIds.push_back(id);
	}

	bool IsAlive(EntityId id)
	{
		assert(Id::IsValid(id));

		const Id::IdType index = Id::GetIndex(id);

		assert(index < Generations.size());
		assert(Generations[index] == Id::GetGeneration(id));

		return Generations[index] == Id::GetGeneration(id) && Transforms[index].IsValid();
	}

	Transform::Component Entity::Transform() const
	{
		assert(IsAlive(m_Id));
		const Id::IdType index = Id::GetIndex(m_Id);
		return Transforms[index];
	}

	Script::Component Entity::Script() const
	{
		assert(IsAlive(m_Id));
		const Id::IdType index = Id::GetIndex(m_Id);
		return Scripts[index];
	}
}
