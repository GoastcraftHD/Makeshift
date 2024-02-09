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

	constexpr u32 GenerationBits{ 8 };
	constexpr IdType GenerationMask{ (IdType{1} << GenerationBits) - 1 };

	constexpr u32 IndexBits{ sizeof(IdType) * 8 - GenerationBits };
	constexpr IdType IndexMask{ (IdType{1} << IndexBits) - 1 };

	constexpr IdType IdMask{ IdType{u32_invalidId} };

	using GenerationType = std::conditional_t<GenerationBits <= 16, std::conditional_t<GenerationBits <= 8, u8, u16>, u32>;
	static_assert(sizeof(GenerationType) * 8 >= GenerationBits);
	static_assert((sizeof(IdType) - sizeof(GenerationType)) > 0);

	inline bool IsValid(IdType id) { return id != IdMask; }
	inline IdType GetIndex(IdType id) { return id & IndexMask; }
	inline IdType GetGeneration(IdType id) { return (id >> IndexBits) & GenerationMask; }

	inline IdType NewGeneration(IdType id)
	{
		const IdType generation{ GetGeneration(id) + 1 };
		assert(generation < 255);
		return GetIndex(id) | (generation << IndexBits);
	}

#if MST_DEBUG
	namespace internal
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
		struct name final : Id::internal::IdBase		\
		{												\
			constexpr explicit name(Id::IdType id)		\
				: IdBase{ id } {}						\
			constexpr name() : IdBase{ Id::IdMask } {}	\
		};

#else
	#define DEFINE_TYPED_ID(name) using name = Id::IdType;
#endif
}
