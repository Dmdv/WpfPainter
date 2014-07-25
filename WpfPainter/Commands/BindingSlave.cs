﻿using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace WpfPainter.Commands
{
	/// <summary>
	/// BindingSlave.
	/// </summary>
	public class BindingSlave : FrameworkElement, INotifyPropertyChanged
	{
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value",
				typeof (object),
				typeof (BindingSlave),
				new PropertyMetadata(null, OnValueChanged));

		public event PropertyChangedEventHandler PropertyChanged;

		public object Value
		{
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private static void OnValueChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			var slave = depObj as BindingSlave;
			Debug.Assert(slave != null);
			slave.OnPropertyChanged("Value");
		}

		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}