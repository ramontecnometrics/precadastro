﻿using System;

namespace framework.Extensions
{
    public static class TimeSpanExtensions

    {
        public static string ToHourText(this TimeSpan? value)
        {
            var result = value != null ? value.Value.ToString("hh':'mm") : null;
            return result;
        }

        public static string GetDescription(this TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return GetDescription(value.Value);
        }

        public static string GetDescription(this TimeSpan value)
        {
            var ts = value;

            var result = "";

            if (ts.Days >= 1)
            {
                result = $"{(int)ts.Days}d ";
            }

            if (ts.Hours >= 1)
            {
                result = $"{result}{(int)ts.Hours}h ";
            }

            if (ts.Minutes >= 1)
            {
                result = $"{result}{(int)ts.Minutes}m ";
            }

            if (ts.Seconds >= 1)
            {
                result = $"{result}{(int)ts.Seconds}s ";
            }

            if (ts.Seconds == 0)
            {
                result = $"{result}{(int)ts.Seconds}s ";
            }

            return result;
        }
    }
}