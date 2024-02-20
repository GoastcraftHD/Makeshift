#pragma once
#include "Test.h"
#include "Components/Entity.h"
#include "Components/Transform.h"

#include <iostream>
#include <ctime>

using namespace Makeshift;

class EngineTest : public Test
{
public:
	bool Initialize() override
	{
		srand((u32)time(nullptr));
		return true;
	}

	void Run() override
	{
		do
		{
			for (u32 i = 0; i < 10000; i++)
			{
				CreateRandomNumOfEntites();
				RemoveRandomNumOfEntites();

				m_NumEntities = (u32)m_Entities.size();
			}

			PrintResults();

		} while (getchar() != 'q');
	}

	void Shutdown() override
	{

	}

private:
	void CreateRandomNumOfEntites()
	{
		u32 count = rand() % 20;

		if (m_Entities.empty())
		{
			count = 1000;
		}

		Transform::InitInfo transformInfo{};
		GameEntity::EntityInfo entityInfo{ &transformInfo };

		while (count > 0)
		{
			m_Added++;

			GameEntity::Entity entity = GameEntity::Create(entityInfo);
			assert(entity.IsValid());
			m_Entities.push_back(entity);
			assert(GameEntity::IsAlive(entity.GetId()));

			count--;
		}
	}

	void RemoveRandomNumOfEntites()
	{
		u32 count = rand() % 20;

		if (m_Entities.size() < 1000)
		{
			return;
		}

		while (count > 0)
		{
			const u32 index = (u32)rand() % (u32)m_Entities.size();
			const GameEntity::Entity entity = m_Entities[index];
			assert(entity.IsValid());

			if (entity.IsValid())
			{
				GameEntity::Remove(entity.GetId());
				m_Entities.erase(m_Entities.begin() + index);
				assert(!GameEntity::IsAlive(entity.GetId()));
				m_Removed++;
			}

			count--;
		}
	}

	void PrintResults()
	{
		std::cout << "Entities created: " << m_Added << "\n";
		std::cout << "Entities deleted: " << m_Removed << "\n";
	}

	Util::vector<GameEntity::Entity> m_Entities;

	u32 m_Added = 0;
	u32 m_Removed = 0;
	u32 m_NumEntities = 0;
};
