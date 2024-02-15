include "./vendor/premake/premake_customization/solution_items.lua"

workspace "Makeshift"
	architecture "x86_64"
    startproject "Editor"

	configurations
	{
		"Debug",
		"Release",
		"Dist"
	}

	solution_items
	{
		".editorconfig"
	}

	flags
	{
		"MultiProcessorCompile"
	}
outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"


-- Include directories relative to root folder (solution directory)
IncludeDir = {}


group "Dependencies"
	include "vendor/premake"
group ""

include "Engine"
include "Editor"
include "EngineTest"
