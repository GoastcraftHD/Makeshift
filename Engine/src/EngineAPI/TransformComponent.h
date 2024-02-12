#pragma once

#include "Components/ComponentsCommon.h"

namespace Makeshift::Transform
{
	DEFINE_TYPED_ID(TransformId);

	class Component final
	{
	public:
		constexpr explicit Component(TransformId id) : m_Id{ id } {}
		constexpr Component() : m_Id{ Id::InvalidId } {}
		constexpr TransformId GetId() const { return m_Id; }
		constexpr bool IsValid() const { return Id::IsValid(m_Id); }

		Math::V3 Position() const;
		Math::V4 Rotation() const;
		Math::V3 Scale() const;

	private:
		TransformId m_Id;
	};
}
