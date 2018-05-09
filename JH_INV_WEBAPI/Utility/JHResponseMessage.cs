using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Utility
{
    public class JHResponseMessage
    {
        private string code;
        private string message;
        private Object data;

        public string Code
        {
            get
            {
                return code;
            }

            set
            {
                code = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        public object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        public JHResponseMessage(string code, string message, Object data)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }
    }
}