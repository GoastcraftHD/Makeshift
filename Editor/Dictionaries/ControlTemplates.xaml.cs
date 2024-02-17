using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Editor.Dictionaries
{
	public partial class ControlTemplates : ResourceDictionary
	{
		private void OnTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			BindingExpression exp = textBox.GetBindingExpression(TextBox.TextProperty);

			if (exp == null)
			{
				return;
			}

			if (e.Key == Key.Enter)
			{
				if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
				{
					command.Execute(textBox.Text);
				}
				else
				{
					exp.UpdateSource();
				}

				Keyboard.ClearFocus();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				exp.UpdateTarget();
				Keyboard.ClearFocus();
			}
		}

		private void OnCloseBtnClick(object sender, RoutedEventArgs e)
		{
			Window window = (sender as FrameworkElement)?.TemplatedParent as Window;
			window?.Close();
        }

		private void OnMaximizeBtnClick(object sender, RoutedEventArgs e)
		{
			Window window = (sender as FrameworkElement)?.TemplatedParent as Window;

			if (window?.WindowState == WindowState.Normal)
			{
				window.WindowState = WindowState.Maximized;
			}
			else
			{
				window.WindowState = WindowState.Normal;
			}
		}

		private void OnMinimizeBtnClick(object sender, RoutedEventArgs e)
		{
			Window window = (sender as FrameworkElement)?.TemplatedParent as Window;
			window.WindowState = WindowState.Minimized;
		}
	}
}
