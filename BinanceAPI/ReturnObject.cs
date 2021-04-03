using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APICall
{
    public class ReturnObject<T>
    {
        public HttpStatusCode Code { get; set; }

        public T Value { get; set; }

        public int RetryAfter { get; set;}

        public string Error { get; set; }

        public ReturnObject()
        {

        }

        public ReturnObject (HttpStatusCode code, T value)
        {
            Code = code;
            Value = value;
        }

        public ReturnObject(HttpStatusCode code, T value, int retryAfter)
            :this(code, value)
        {
            retryAfter = RetryAfter;
        }
    }
}
