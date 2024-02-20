project "Editor"
    language "C#"
    flags { "WPF" }
    kind "WindowedApp"
    dotnetframework "4.8"
    csversion "12"

    targetdir ("%{wks.location}/bin/" .. outputdir .. "/%{prj.name}")
	objdir ("%{wks.location}/bin-int/" .. outputdir .. "/%{prj.name}")

    files
    {
        "**.cs",
        "**.xaml",
        "**.config",
        "**.settings",
        "**.resx"
    }

    links
    {
        "Microsoft.Csharp",
        "PresentationCore",
        "PresentationFramework",
        "System",
        "System.Core",
        "System.Data",
        "System.Data.DataSetExtensions",
        "System.Xaml",
        "System.Xml",
        "System.Xml.Linq",
        "System.Runtime.Serialization",
        "System.Numerics",
        "WindowsBase.dll",
        "%{wks.location}/vendor/microsoft/Interop.EnvDTE.dll",
        "%{wks.location}/vendor/microsoft/Interop.EnvDTE80.dll",
        "EngineDLL"
    }



    filter "configurations:Debug"
		defines
        {
            "MKS_DEBUG",
            "DEBUG"
        }
		runtime "Debug"
		symbols "on"

	filter "configurations:Release"
		defines "MKS_RELEASE"
		runtime "Release"
		optimize "on"
