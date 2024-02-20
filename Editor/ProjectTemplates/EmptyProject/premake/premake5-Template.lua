workspace "{0}"
	location "../../"
	architecture "x86_64"
    startproject "{0}"

	configurations
	{
		"Debug",
		"Release",
		"Debug-Editor",
		"Release-Editor"
	}

	flags
	{
		"MultiProcessorCompile"
	}

	outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"

project "{0}"
	kind "ConsoleApp"
	location "../../Scripts"
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
		"../../{0}/**.h",
		"../../{0}/**.cpp"
	}

	defines
	{
		"_CRT_SECURE_NO_WARNINGS"
	}

	includedirs
	{
		"{1}"
	}

	forceincludes
	{
		"GameEntity.h"
	}

	libdirs
	{
		"C:/dev/VisualStudio/Makeshift/bin/Debug-%{cfg.system}-%{cfg.architecture}/Engine" --Temporary
	}

	links
	{
		"Engine.lib"
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

	filter "configurations:Debug-Editor"
		kind "SharedLib"
		defines
		{
			"MKS_DEBUG",
			"MKS_EDITOR"
		}
		runtime "Debug"
		symbols "on"

	filter "configurations:Release-Editor"
		kind "SharedLib"
		defines
		{
			"MKS_RELEASE",
			"MKS_EDITOR"
		}
		runtime "Release"
		optimize "on"
