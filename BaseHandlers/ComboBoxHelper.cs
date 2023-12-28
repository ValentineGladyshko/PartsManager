using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PartsManager.BaseHandlers
{
    public static class ComboBoxHelper
    {
        public static void SetDropDownOpened(ComboBox comboBox, IQueryable<IItem> items)
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
    }
}
