﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Editor.DLLWrappers;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
	class GameEntity : ViewModelBase
	{
		private int _entityId = Id.INVALID_ID;
		public int EntityId
		{
			get => _entityId;
			set
			{
				if (_entityId != value)
				{
					_entityId = value;
					OnPropertyChanged(nameof(EntityId));
				}
			}
		}

		private bool _isActive;
		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;

					if (_isActive)
					{
						EntityId = EngineAPI.CreateGameEntity(this);
						Debug.Assert(Id.IsValid(_entityId));
					}
					else if (Id.IsValid(_entityId))
					{
						EngineAPI.RemoveGameEntity(this);
						EntityId = Id.INVALID_ID;
					}

					OnPropertyChanged(nameof(IsActive));
				}
			}
		}

		private string _name;
		[DataMember]
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		private bool _isEnabled = true;
		[DataMember]
		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
					OnPropertyChanged(nameof(IsEnabled));
				}
			}
		}

		[DataMember]
		public Scene ParentScene { get; private set; }

		[DataMember(Name = nameof(Components))]
		private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
		public ReadOnlyObservableCollection<Component> Components { get; private set; }

		public GameEntity(Scene parentScene)
		{
			Debug.Assert(parentScene != null);
			ParentScene = parentScene;
			_components.Add(new Transform(this));
			OnDeserialized(new StreamingContext());
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			if (_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}
		}

		public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
		public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;
	}

	abstract class MSEntity : ViewModelBase
	{
		private bool _enableUpdates = true;

		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		private bool? _isEnabled = true;
		public bool? IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
					OnPropertyChanged(nameof(IsEnabled));
				}
			}
		}

		private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
		public ReadOnlyObservableCollection<IMSComponent> Components { get; }

		public List<GameEntity> SelectedEntities { get; }

		public MSEntity(List<GameEntity> selectedEntities)
		{
			Debug.Assert(selectedEntities?.Any() == true);

			Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
			SelectedEntities = selectedEntities;

			PropertyChanged += (s, e) =>
			{
				if (_enableUpdates)
				{
					UpdateGameEntities(e.PropertyName);
				}
			};
		}

		protected virtual bool UpdateGameEntities(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Name):
					SelectedEntities.ForEach(x => x.Name = Name);
					return true;

				case nameof(IsEnabled):
					SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value);
					return true;
			}

			return false;
		}

		protected virtual bool UpdateMSGameEntities()
		{
			Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));
			IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));

			return true;
		}

		public T GetMSComponent<T>() where T : IMSComponent
		{
			return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
		}

		public static float? GetMixedValue<T>(List<T> objects, Func<T, float> getProperty)
		{
			float value = getProperty(objects.First());
			//return objects.Skip(1).Any(x => !getProperty(x).IsTheSameAs(value)) ? null : value;

			foreach (T obj in objects.Skip(1))
			{
				if (!value.IsTheSameAs(getProperty(obj)))
				{
					return null;
				}

			}
			return value;
		}

		public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool> getProperty)
		{
			bool value = getProperty(objects.First());
			return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
		}

		public static string GetMixedValue<T>(List<T> objects, Func<T, string> getProperty)
		{
			string value = getProperty(objects.First());
			return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
		}

		public void Refresh()
		{
			_enableUpdates = false;
			UpdateMSGameEntities();
			MakeComponentList();
			_enableUpdates = true;
		}

		private void MakeComponentList()
		{
			_components.Clear();
			GameEntity firstEntity = SelectedEntities.FirstOrDefault();

			if (firstEntity == null)
			{
				return;
			}

			foreach (Component component in firstEntity.Components)
			{
				var type = component.GetType();

				if (!SelectedEntities.Skip(1).Any(entity => entity.GetComponent(type) == null))
				{
					Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
					_components.Add(component.GetMultiSelectionComponent(this));
				}
			}
		}
	}

	class MSGameEntitiy : MSEntity
	{
		public MSGameEntitiy(List<GameEntity> selectedEntities) : base(selectedEntities)
		{
			Refresh();
		}
	}
}
