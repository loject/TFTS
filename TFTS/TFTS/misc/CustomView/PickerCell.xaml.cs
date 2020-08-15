using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TFTS.misc.CustomView
{
    [ContentProperty("Items")]
    public partial class PickerCell : ViewCell
    {
        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create(
                nameof(SelectedValue), typeof(string), typeof(PickerCell), null,
                BindingMode.TwoWay,
                propertyChanged: (sender, oldValue, newValue) =>
                {
                    var pickerCell = (PickerCell)sender;
                    var picker = pickerCell.picker;

                    picker.SelectedIndex = GetIndex(newValue, pickerCell.ItemsSource);

                    static int GetIndex(object newValue, IEnumerable<string> source)
                    {
                        if (newValue == null) return -1;

                        var newValueT = (string)newValue;

                        return source.IndexOf(opt => opt.Equals(newValueT));
                    }
                });

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource), typeof(IList<string>), typeof(PickerCell), new List<string>(),
                BindingMode.TwoWay,
                propertyChanged: (sender, oldValue, newValue) =>
                {
                    var pickerCell = (PickerCell)sender;

                    pickerCell.picker.ItemsSource = (IList)newValue;
                });

        public PickerCell()
        {
            InitializeComponent();
        }

        public string Label
        {
            get => label.Text;
            set => label.Text = value;
        }

        public string Title
        {
            get => picker.Title;
            set => picker.Title = value;
        }

        public string SelectedValue
        {
            get => (string)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public IList<string> ItemsSource
        {
            get => (IList<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private void OnPickerSelectedIndexChanged(object sender, EventArgs args)
        {
            if (picker.SelectedIndex == -1)
                SelectedValue = null;
            else 
                SelectedValue = (string)picker.ItemsSource[picker.SelectedIndex];
        }
    }
}