project "Engine"
	kind "StaticLib"
	language "C++"
	cppdialect "C++20"
	staticruntime "on"

	targetdir ("%{wks.location}/bin/" .. outputdir .. "/%{prj.name}")
	objdir ("%{wks.location}/bin-int/" .. outputdir .. "/%{prj.name}")

	files
	{
		"src/**.h",
		"src/**.cpp"
	}

	defines
	{
		"_CRT_SECURE_NO_WARNINGS"
	}

	includedirs
	{
		"src"
	}

	filter "system:windows"
		systemversion "latest"

		defines
		{
		}

	filter "configurations:Debug"
		defines "MST_DEBUG"
		runtime "Debug"
		symbols "on"

	filter "configurations:Release"
		defines "MST_RELEASE"
		runtime "Release"
		optimize "on"

	filter "configurations:Dist"
		defines "MST_DIST"
		runtime "Release"
		optimize "on"