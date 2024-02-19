using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Editor.Utilities.Controls
{
	public enum VectorType
	{
		Vector2,
		Vector3,
		Vector4
	}

	class VectorBox : Control
	{
		public Orientation Orientation
		{
			get => (Orientation)GetValue(MultiplierProperty);
			set => SetValue(MultiplierProperty, value);
		}

		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VectorBox), new PropertyMetadata(Orientation.Horizontal));

		public VectorType VectorType
		{
			get => (VectorType)GetValue(VectorTypeProperty);
			set => SetValue(VectorTypeProperty, value);
		}

		public static readonly DependencyProperty VectorTypeProperty = DependencyProperty.Register(nameof(VectorType), typeof(VectorType), typeof(VectorBox), new PropertyMetadata(VectorType.Vector3));

		public double Multiplier
		{
			get => (double)GetValue(MultiplierProperty);
			set => SetValue(MultiplierProperty, value);
		}

		public static readonly DependencyProperty MultiplierProperty = DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(VectorBox), new PropertyMetadata(1.0));

		public string X
		{
			get => GetValue(XProperty) as string;
			set => SetValue(XProperty, value);
		}

		public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(string), typeof(VectorBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Y
		{
			get => GetValue(YProperty) as string;
			set => SetValue(YProperty, value);
		}

		public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(string), typeof(VectorBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		
		public string Z
		{
			get => GetValue(ZProperty) as string;
			set => SetValue(ZProperty, value);
		}
		
		public static readonly DependencyProperty ZProperty = DependencyProperty.Register(nameof(Z), typeof(string), typeof(VectorBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string W
		{
			get => GetValue(WProperty) as string;
			set => SetValue(WProperty, value);
		}

		public static readonly DependencyProperty WProperty = DependencyProperty.Register(nameof(W), typeof(string), typeof(VectorBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		static VectorBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorBox), new FrameworkPropertyMetadata(typeof(VectorBox)));
		}
	}
}
