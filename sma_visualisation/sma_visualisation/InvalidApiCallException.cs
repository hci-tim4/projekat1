using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sma_visualisation
{
    class InvalidApiCallException : Exception
    {
        public string message;
        public InvalidApiCallException(string message) {
            this.message = message;
        }
    }
}
