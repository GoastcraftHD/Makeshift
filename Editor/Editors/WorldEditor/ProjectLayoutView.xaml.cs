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
	/// Interaction logic for ProjectLayoutView.xaml
	/// </summary>
	public partial class ProjectLayoutView : UserControl
	{
		public ProjectLayoutView()
		{
			InitializeComponent();
		}

		private void OnAddGameEntityBtnClick(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			Scene vm = btn.DataContext as Scene;

			vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity"});
        }

		private void OnGameEntitiesListboxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GameEntityView.Instance.DataContext = null;

			if (e.AddedItems.Count > 0)
			{
				GameEntityView.Instance.DataContext =  (sender as ListBox).SelectedItems[0];
			}

			ListBox listBox = sender as ListBox;
			List<GameEntity> newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
			List<GameEntity> previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

			Project.UndoRedo.Add(new UndoRedoAction(() =>
			{
				listBox.UnselectAll();
				previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
			}, () =>
			{
				listBox.UnselectAll();
				newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
			}, "Selection changed"));
		}
	}
}
