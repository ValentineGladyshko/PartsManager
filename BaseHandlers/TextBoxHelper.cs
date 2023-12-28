using PartsManager.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    }
}
