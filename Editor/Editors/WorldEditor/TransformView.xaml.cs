using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Editors
{
	/// <summary>
	/// Interaction logic for TransformView.xaml
	/// </summary>
	public partial class TransformView : UserControl
	{
		private Action _undoAction = null;
		private bool _propertyChanged = false;

		public TransformView()
		{
			InitializeComponent();

			Loaded += OnTransformViewLoaded;
		}

		private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnTransformViewLoaded;
			(DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
		}

		private void OnPositionVectorBoxPreviewMouseLBD(object sender, MouseButtonEventArgs e)
		{
			_propertyChanged = false;

			_undoAction = GetPositionAction();
		}

		private void OnPositionVectorBoxPreviewMouseLBU(object sender, MouseButtonEventArgs e)
		{
			RecordAction(GetPositionAction(), "Position changed");
		}

		private void OnPositionVectorBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_propertyChanged && _undoAction != null)
			{
				OnPositionVectorBoxPreviewMouseLBU(sender, null);
			}
		}

		private void OnRotationVectorBoxPreviewMouseLBD(object sender, MouseButtonEventArgs e)
		{
			_propertyChanged = false;

			_undoAction = GetRotationAction();
		}

		private void OnRotationVectorBoxPreviewMouseLBU(object sender, MouseButtonEventArgs e)
		{
			RecordAction(GetRotationAction(), "Rotation changed");
		}

		private void OnRotationVectorBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_propertyChanged && _undoAction != null)
			{
				OnRotationVectorBoxPreviewMouseLBU(sender, null);
			}
		}

		private void OnScaleVectorBoxPreviewMouseLBD(object sender, MouseButtonEventArgs e)
		{
			_propertyChanged = false;

			_undoAction = GetScaleAction();
		}

		private void OnScaleVectorBoxPreviewMouseLBU(object sender, MouseButtonEventArgs e)
		{
			RecordAction(GetScaleAction(), "Scale changed");
		}

		private void OnScaleVectorBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_propertyChanged && _undoAction != null)
			{
				OnScaleVectorBoxPreviewMouseLBU(sender, null);
			}
		}

		private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction)
		{
			if (!(DataContext is MSTransform vm))
			{
				_undoAction = null;
				_propertyChanged = false;
				return null;
			}

			var selection = vm.SelectedComponents.Select(x => selector(x)).ToList();
			return new Action(() =>
			{
				selection.ForEach(x => forEachAction(x));
				(GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
			});
		}

		private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transform.Position = x.Item2);
		private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);
		private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

		private void RecordAction(Action redoAction, string name)
		{
			if (_propertyChanged)
			{
				Debug.Assert(_undoAction != null);

				_propertyChanged = false;

				Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
			}
		}
	}
}
