﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TFTS.misc
{
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public void Sort(Comparison<T> comparison)
        {
            var items = this.Items as List<T>;
            items.Sort(comparison);
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
}
