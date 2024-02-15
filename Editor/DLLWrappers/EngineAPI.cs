using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Editor.Components;
using Editor.EngineAPIStructs;

namespace Editor.EngineAPIStructs
{
	[StructLayout(LayoutKind.Sequential)]
	class TransformComponent
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;
	}

	[StructLayout(LayoutKind.Sequential)]
	class GameEntityDescriptor
	{
		public TransformComponent Transform = new TransformComponent();
	}
}

namespace Editor.DLLWrappers
{
	static class EngineAPI
	{
		private const string _dllName = "EngineDLL.dll";

		[DllImport(_dllName)]
		private static extern int CreateGameEntity(GameEntityDescriptor descriptor);
		public static int CreateGameEntity(GameEntity entity)
		{
			GameEntityDescriptor desc = new GameEntityDescriptor();

			Transform transform = entity.GetComponent<Transform>();
			desc.Transform.Position = transform.Position;
			desc.Transform.Rotation = transform.Rotation;
			desc.Transform.Scale = transform.Scale;

			return CreateGameEntity(desc);
		}

		[DllImport(_dllName)]
		private static extern void RemoveGameEntity(int id);
		public static void RemoveGameEntity(GameEntity entity)
		{
			RemoveGameEntity(entity.EntityId);
		}
	}
}
