#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>
#include <algorithm>

namespace Makeshift::Util
{
	template<typename T>
	using vector = typename std::vector<T>;

	template<typename T>
	void EraseUnordered(vector<T>& v, size_t index)
	{
		if (v.size() > 1)
		{
			std::iter_swap(v.begin() + index, v.end() - 1);
			v.pop_back();
		}
		else
		{
			v.clear();
		}
	}
}
#endif

#if USE_STL_DEQUE
#include <deque>

namespace Makeshift::Util
{
	template<typename T>
	using deque = typename std::deque<T>;
}
#endif

namespace Makeshift::Util
{
	//TODO: implement custom vector and deque
}
