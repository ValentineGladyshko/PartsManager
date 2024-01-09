using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PartsManager.BaseHandlers
{
    public static class ComboBoxHelper
    {
        public static void SetDropDownOpened(this ComboBox comboBox, IQueryable<IItem> items)
        {
            comboBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = items
                    .Where(item => item.Name.Contains(comboBox.Text))
                    .Select(item => item.Name).ToList();
                list.Sort();
                comboBox.ItemsSource = list;
            };
        }

        public static void SetTextChangedFirstCharToUpper(this ComboBox comboBox)
        {
            comboBox.Loaded += (object sender, RoutedEventArgs e) => 
            {
                var editTextBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;           
                if (editTextBox != null)
                {
                    editTextBox.SetTextChangedFirstCharToUpper();
                }
            };
        }
    }
}
