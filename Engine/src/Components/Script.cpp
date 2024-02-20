#include "Script.h"
#include "Entity.h"

namespace Makeshift::Script
{
	namespace 
	{
		Util::vector<Detail::ScriptPtr> EntityScripts;
		Util::vector<Id::IdType> IdMapping;

		Util::vector<Id::GenerationType> Generations;
		Util::deque<ScriptId> FreeIds;

		using ScriptRegistry = std::unordered_map<size_t, Detail::ScriptCreator>;

		ScriptRegistry& Registry()
		{
			// Make sure, that this variable is created before it is used
			static ScriptRegistry registry;
			return registry;
		}

		bool Exists(ScriptId id)
		{
			assert(Id::IsValid(id));
			const Id::IdType index = Id::GetIndex(id);
			assert(index < Generations.size() && IdMapping[index] < EntityScripts.size());
			assert(Generations[index] == Id::GetGeneration(id));
			return Generations[index] == Id::GetGeneration(id) && EntityScripts[IdMapping[index]] && EntityScripts[IdMapping[index]]->IsValid();
		}
	}

	namespace Detail
	{
		u8 RegisterScript(size_t hashedScripName, ScriptCreator scriptCreator)
		{
			bool result = Registry().insert(ScriptRegistry::value_type{ hashedScripName, scriptCreator }).second;
			assert(result);
			return  result;
		}
	}

	Component Create(InitInfo info, GameEntity::Entity entity)
	{
		assert(entity.IsValid());
		assert(info.ScripCreator);

		ScriptId id{};

		if (FreeIds.size() > Id::MinDeletedElements)
		{
			id = FreeIds.front();
			assert(!Exists(id));
			FreeIds.pop_front();
			id = ScriptId{ Id::NewGeneration(id) };
			Generations[Id::GetIndex(id)]++;
		}
		else
		{
			id = ScriptId{ (Id::IdType)IdMapping.size() };
			IdMapping.emplace_back();
			Generations.push_back(0);
		}

		assert(Id::IsValid(id));
		const Id::IdType index = (Id::IdType)EntityScripts.size();
		EntityScripts.emplace_back(info.ScripCreator(entity));
		assert(EntityScripts.back()->GetId() == entity.GetId());
		IdMapping[Id::GetIndex(id)] = index;

		return Component{ id };
	}

	void Remove(Component component)
	{
		assert(component.IsValid() && Exists(component.GetId()));
		const ScriptId id = component.GetId();
		const Id::IdType index = IdMapping[Id::GetIndex(id)];
		const ScriptId lastId = EntityScripts.back()->Script().GetId();

		Util::EraseUnordered(EntityScripts, index);
		IdMapping[Id::GetIndex(lastId)] = index;
		IdMapping[Id::GetIndex(id)] = Id::InvalidId;
	}
}
