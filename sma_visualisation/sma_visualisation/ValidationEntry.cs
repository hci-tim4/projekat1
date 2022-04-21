using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace sma_visualisation
{
    public class ValidationEntry
    {
        public static bool validateComboBox(ComboBox cb)
        {
            if (cb.SelectedValue == null)
            {
                return false;
            }
            return true;
        }

        public static bool emptyTextBox(TextBox tb)
        {
            if (tb.Text == "")
            {
                return false;
            }
            return true;
        }
       
    }
}
