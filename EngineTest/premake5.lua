project "EngineTest"
	kind "ConsoleApp"
	staticruntime "on"
	language "C++"
	cppdialect "C++20"
	exceptionhandling "Off"
	rtti "Off"
	floatingpoint "Fast"
	warnings "Everything"

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
		"src",
		"../Engine/src"
	}

	libdirs
	{
		"%{wks.location}/bin/" .. outputdir .. "/Engine"
	}

	filter "system:windows"
		systemversion "latest"

		defines
		{
			"MKS_WINDOWS"
		}

	filter "configurations:Debug"
		defines "MKS_DEBUG"
		runtime "Debug"
		symbols "on"

	filter "configurations:Release"
		defines "MKS_RELEASE"
		runtime "Release"
		optimize "on"
