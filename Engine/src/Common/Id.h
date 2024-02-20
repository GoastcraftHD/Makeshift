#pragma once

#include "CommonHeaders.h"

/* Ids are split into a generation part and a index part.
 * The generation part is for differentiating, when an index gets reused.
 *
 * e.g. 0000 0000 0000 0000 0000 0000 0000 0000 (32 bit id)
 *     |---------|-----------------------------|
 *	    Generation             Index
 *
 * The generation bits can be increased when the risk of ids getting reused too often is too high.
 */

namespace Makeshift::Id
{
	using IdType = u32;

	namespace Detail
	{
		constexpr u32 GenerationBits = 8;
		constexpr IdType GenerationMask = (IdType{1} << GenerationBits) - 1;

		constexpr u32 IndexBits = sizeof(IdType) * 8 - GenerationBits;
		constexpr IdType IndexMask = (IdType{1} << IndexBits) - 1;
	}

	constexpr IdType InvalidId = IdType(-1);
	constexpr u32 MinDeletedElements = 1024;

	using GenerationType = std::conditional_t<Detail::GenerationBits <= 16, std::conditional_t<Detail::GenerationBits <= 8, u8, u16>, u32>;
	static_assert(sizeof(GenerationType) * 8 >= Detail::GenerationBits);
	static_assert((sizeof(IdType) - sizeof(GenerationType)) > 0);

	constexpr bool IsValid(IdType id) { return id != InvalidId; }
	constexpr IdType GetIndex(IdType id)
	{
		IdType index = id & Detail::IndexMask;
		assert(index != Detail::IndexMask);
		return index;
	}
	constexpr IdType GetGeneration(IdType id) { return (id >> Detail::IndexBits) & Detail::GenerationMask; }

	constexpr IdType NewGeneration(IdType id)
	{
		const IdType generation = GetGeneration(id) + 1;
		assert(generation < (((u64)1 << Detail::GenerationBits) - 1));
		return GetIndex(id) | (generation << Detail::IndexBits);
	}

#if MKS_DEBUG
	namespace Detail
	{
		struct IdBase
		{
			constexpr explicit IdBase(IdType id) : m_Id{ id } {}
			constexpr operator IdType() const { return m_Id; }

		private:
			IdType m_Id;
		};

	}

	#define DEFINE_TYPED_ID(name)						\
		struct name final : Id::Detail::IdBase		\
		{												\
			constexpr explicit name(Id::IdType id)		\
				: IdBase{ id } {}						\
			constexpr name() : IdBase{ 0 } {}	\
		};

#else
	#define DEFINE_TYPED_ID(name) using name = Id::IdType;
#endif
}
