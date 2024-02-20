#pragma once

#include "../Components/ComponentsCommon.h"

namespace Makeshift::Script
{
	DEFINE_TYPED_ID(ScriptId);

	class Component final
	{
	public:
		constexpr explicit Component(ScriptId id) : m_Id{ id } {}
		constexpr Component() : m_Id{ Id::InvalidId } {}
		constexpr ScriptId GetId() const { return m_Id; }
		constexpr bool IsValid() const { return Id::IsValid(m_Id); }

	private:
		ScriptId m_Id;
	};
}
