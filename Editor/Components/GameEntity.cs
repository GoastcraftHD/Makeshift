using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
	class GameEntity : ViewModelBase
	{
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

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			if (_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}
		}

		public GameEntity(Scene parentScene)
		{
			Debug.Assert(parentScene != null);
			ParentScene = parentScene;
			_components.Add(new Transform(this));
			OnDeserialized(new StreamingContext());
		}
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

		public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
		{
			float value = getProperty(entities.First());

			foreach (GameEntity entity in entities.Skip(1))
			{
				if (!value.IsTheSameAs(getProperty(entity)))
				{
					return null;
				}
			}

			return value;
		}

		public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
		{
			bool value = getProperty(entities.First());

			foreach (GameEntity entity in entities.Skip(1))
			{
				if (value != getProperty(entity))
				{
					return null;
				}
			}

			return value;
		}

		public static string GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
		{
			string value = getProperty(entities.First());

			foreach (GameEntity entity in entities.Skip(1))
			{
				if (value != getProperty(entity))
				{
					return null;
				}
			}

			return value;
		}

		public void Refresh()
		{
			_enableUpdates = false;
			UpdateMSGameEntities();
			_enableUpdates = true;
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
