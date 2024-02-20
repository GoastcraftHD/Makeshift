#pragma once

#include "../Common/CommonHeaders.h"

namespace Makeshift::Math
{
	constexpr float Pi = 3.1415926535897932384626433832795f;
	constexpr float Epsilon = 1e-5f;

#if defined(_WIN64)
	using V2 = DirectX::XMFLOAT2;
	using V2A = DirectX::XMFLOAT2A;
	using V3 = DirectX::XMFLOAT3;
	using V3A = DirectX::XMFLOAT3A;
	using V4 = DirectX::XMFLOAT4;
	using V4A = DirectX::XMFLOAT4A;
	using U32V2 = DirectX::XMUINT2;
	using U32V3 = DirectX::XMUINT3;
	using U32V4 = DirectX::XMUINT4;
	using S32V2 = DirectX::XMINT2;
	using S32V3 = DirectX::XMINT3;
	using S32V4 = DirectX::XMINT4;
	using M3x3 = DirectX::XMFLOAT3X3;
	using M4x4 = DirectX::XMFLOAT4X4;
	using M4x4A = DirectX::XMFLOAT4X4A;
#endif
}
