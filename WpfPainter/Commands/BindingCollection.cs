﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace WpfPainter.Commands
{
	/// <summary>
	/// 	BindingCollection.
	/// </summary>
	public class BindingCollection : Collection<BindingBase>
	{
		protected override void ClearItems()
		{
			base.ClearItems();
			OnBindingCollectionChanged();
		}

		protected override void InsertItem(int index, BindingBase item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			ValidateItem(item);
			base.InsertItem(index, item);
			OnBindingCollectionChanged();
		}

		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			OnBindingCollectionChanged();
		}

		protected override void SetItem(int index, BindingBase item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			ValidateItem(item);
			base.SetItem(index, item);
			OnBindingCollectionChanged();
		}

		private static void ValidateItem(BindingBase binding)
		{
			if (!(binding is Binding))
			{
				throw new NotSupportedException("BindingCollectionContainsNonBinding");
			}
		}

		private void OnBindingCollectionChanged()
		{
			if (_collectionChangedEventHandler != null)
			{
				_collectionChangedEventHandler();
			}
		}

		private readonly BindingCollectionChangedEventHandler _collectionChangedEventHandler = delegate { };
	}
}