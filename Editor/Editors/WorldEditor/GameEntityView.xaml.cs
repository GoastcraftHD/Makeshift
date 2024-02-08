using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Editors
{
	/// <summary>
	/// Interaction logic for GameEntityView.xaml
	/// </summary>
	public partial class GameEntityView : UserControl
	{
		private Action _undoAction;
		private string _propertyName;

		public static GameEntityView Instance { get; private set; }

		public GameEntityView()
		{
			InitializeComponent();
			DataContext = null;
			Instance = this;
			DataContextChanged += (_, __) =>
			{
				if (DataContext != null)
				{
					(DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
				}
			};
		}

		private Action GetRenameAction()
		{
			MSEntity vm = DataContext as MSEntity;
			var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
			return new Action(() =>
			{
				selection.ForEach(item => item.entity.Name = item.Name);
				(DataContext as MSEntity).Refresh();
			});
		}

		private Action GetIsEnabledAction()
		{
			MSEntity vm = DataContext as MSEntity;
			var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
			return new Action(() =>
			{
				selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
				(DataContext as MSEntity).Refresh();
			});
		}

		private void OnNameTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			_undoAction = GetRenameAction();
		}

		private void OnNameTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
			{
				Action redoAction = GetRenameAction();
				Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
				_propertyName = null;
			}

			_undoAction = null;
		}

		private void OnIsEnabledCheckboxClick(object sender, RoutedEventArgs e)
		{
			Action undoAction = GetIsEnabledAction();
			MSEntity vm = DataContext as MSEntity;
			vm.IsEnabled = (sender as CheckBox).IsChecked == true;
			Action redoAction = GetIsEnabledAction();

			Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
		}
	}
}
