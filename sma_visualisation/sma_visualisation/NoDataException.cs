using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sma_visualisation
{
    class NoDataException : Exception
    {
        public string message;
        public NoDataException(string message) {
            this.message = message;
        }
    }
}
