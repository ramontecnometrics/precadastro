using System;

namespace framework
{
    public static class DateTimeSync
    {
        private static double? _timeDiference;

        public static DateTime Now
        {
            get
            {
                if (!_timeDiference.HasValue)
                {
                    throw new Exception("Data do sistema não sincronizada.");
                }
                return DateTime.Now.AddMilliseconds(_timeDiference.Value);
            }
        }

        public static void SetNow(DateTime now)
        {
            _timeDiference = (DateTime.Now - now).TotalMilliseconds;
        }
    }
}
