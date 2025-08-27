using System;
using System.Security;

namespace framework.Extensions
{
    public static class SecureStringExtensions 
    {
        public static string ToPlainText(this SecureString value)  
        {
            string key = null;
            var bstrKey = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(value);
            try
            {
                key = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(bstrKey);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeBSTR(bstrKey);
            }
            return key;
        }
    }
}
