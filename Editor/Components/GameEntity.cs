﻿using System;
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
	public class GameEntity : ViewModelBase
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

		public ICommand RenameCommand { get; private set; }
		public ICommand IsEnabledCommand { get; private set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			if (_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}

			RenameCommand = new RelayCommand<string>(x =>
			{
				string oldName = _name;
				Name = x;

				Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Rename entity '{oldName}' to '{x}'"));
			}, x => x != _name);

			IsEnabledCommand = new RelayCommand<bool>(x =>
			{
				bool oldValue = _isEnabled;
				IsEnabled = x;

				Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this, oldValue, x, x ? $"Enable {Name}" : $"Disable {Name}"));
			});
		}

		public GameEntity(Scene parentScene)
		{
			Debug.Assert(parentScene != null);
			ParentScene = parentScene;
			_components.Add(new Transform(this));
			OnDeserialized(new StreamingContext());
		}
	}
}