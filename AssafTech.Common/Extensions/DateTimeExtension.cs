using System;
using System.Collections.Generic;
namespace AssafTech.Common.Extensions;

public static class DateTimeExtension
{
    public static DateTime ToLebanonDateTime(this DateTime utcDatetime)
    {
        // Specify the time zone for Lebanon
        TimeZoneInfo lebanonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Middle East Standard Time");

        // Convert UTC time to Lebanon time
        DateTime lebanonTime = TimeZoneInfo.ConvertTimeFromUtc(utcDatetime, lebanonTimeZone);

        return lebanonTime;
    }


    public static string ToStringDateTime(this DateTime datetime)
    {
        return datetime.ToString("dd-MM-yyyy HH:mm tt");
    }

    public static string ToStringDate(this DateTime datetime)
    {
        return datetime.ToString("dd-MM-yyyy");
    }
}
