#pragma once

#include <string>

#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"
#include "ScriptComponent.h"

namespace Makeshift
{
	namespace GameEntity
	{
		DEFINE_TYPED_ID(EntityId);

		class Entity
		{
		public:
			constexpr explicit Entity(EntityId id) : m_Id{ id } {}
			constexpr Entity() : m_Id{ Id::InvalidId } {}
			constexpr EntityId GetId() const { return m_Id; }
			constexpr bool IsValid() const { return Id::IsValid(m_Id); }

			Transform::Component Transform() const;
			Script::Component Script() const;

		private:
			EntityId m_Id;
		};
	}

	namespace Script
	{
		class EntityScript : public GameEntity::Entity
		{
		public:
			virtual ~EntityScript() = default;

			virtual void Start() {}
			virtual void Update(float deltaTime) {}

		protected:
			constexpr explicit EntityScript(GameEntity::Entity entity)
				: GameEntity::Entity(entity.GetId())  {}
		};

		namespace Detail
		{
			using ScriptPtr = std::unique_ptr<EntityScript>;
			using ScriptCreator = ScriptPtr(*)(GameEntity::Entity entity);
			using StringHash = std::hash<std::string>;

			u8 RegisterScript(size_t hashedScripName, ScriptCreator scriptCreator);

			template<class ScriptClass>
			ScriptPtr CreateScript(GameEntity::Entity entity)
			{
				assert(entity.IsValid());
				return std::make_unique<ScriptClass>(entity);
			}

		#define REGISTER_SCRIPT(TYPE)																														\
		class TYPE;																																			\
		namespace																																			\
		{																																					\
			u8 _reg_##TYPE																																	\
			{																																				\
				Makeshift::Script::Detail::RegisterScript(Makeshift::Script::Detail::StringHash()(#TYPE), &Makeshift::Script::Detail::CreateScript<TYPE>)	\
			};																																				\
		}

		}
	}
}
