﻿using PartsManager.Model.Entities;
using PartsManager.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PartsManager.BaseHandlers
{
    public static class TextBoxHelper
    {
        public static string ActionText(ActionType action)
        {
            if (action == ActionType.Create)
                return "створeння";
            else if (action == ActionType.Edit)
                return "редагування";
            return String.Empty;
        }

        public static string GetStringFromPrice(int price)
        {
            return price / 100 + "." + price % 100;
        }

        public static int GetPriceFromString(string str) 
        {
            var regex = @"([0-9])+\.([0-9])+";
            if(Regex.IsMatch(str, regex))
            {
                var strings = str.Split('.');
                if (strings.Length > 1)
                {
                    var integralPart = strings[0];
                    var fractionalPart = strings[1].Substring(0, 2);
                    return Convert.ToInt32(integralPart + fractionalPart);
                }    
            }          

            return 0;
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: return input;
                case "": return input;
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }

        public static void SetTextChangedFirstCharToUpper(this TextBox textBox)
        {
            textBox.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                var cursorPosition = textBox.SelectionStart;
                var selectionLength = textBox.SelectionLength;
                textBox.Text = textBox.Text.FirstCharToUpper();
                textBox.Select(cursorPosition, selectionLength);
            };
        }

        public static void SetTextChangedToUpper(this TextBox textBox)
        {
            textBox.TextChanged += (object sender, TextChangedEventArgs e) =>
            {
                var cursorPosition = textBox.SelectionStart;
                var selectionLength = textBox.SelectionLength;
                if (textBox.Text.Length > 0)
                    textBox.Text = textBox.Text.ToUpper();
                textBox.Select(cursorPosition, selectionLength);
            };
        }

        public static ICollection<string> GetTextCollection(this List<IItem> items)
        {
            var result = new List<string>();
            if (items == null || !items.Any())
                return result;
            else
            {
                for (int i = 0; i < items.Count - 1; i++)
                {
                    result.Add($"{items[i].Name},");
                }
                result.Add(items[items.Count - 1].Name);
                return result;
            }
        }

        public static string Screen(this string s)
        {
            string tmp = s;
            if (s != null)
            {
                if (s.Contains("'"))
                {
                    if (!s.Contains("''"))//Already been fixed previously so skip here
                        tmp = s.Replace("'", "''");

                    s = tmp;
                }
            }
            return tmp;
        }
    }
}
