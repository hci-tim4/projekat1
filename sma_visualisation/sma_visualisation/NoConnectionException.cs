using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sma_visualisation
{
    class NoConnectionException : Exception
    {
        public string message;
        public NoConnectionException(string message)
        {
            this.message = message;
        }
    }
}
