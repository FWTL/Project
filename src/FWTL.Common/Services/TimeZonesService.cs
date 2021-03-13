using System.Collections.Generic;
using System.Linq;
using FWTL.Core.Services;
using NodaTime.TimeZones;
using TimeZoneNames;

namespace FWTL.Common.Services
{
    public class TimeZonesService : ITimeZonesService
    {
        public IDictionary<string, string> GetAll()
        {
            return TZNames.GetDisplayNames("en-US", true);
        }

        public bool Exist(string zoneId)
        {
            return TzdbDateTimeZoneSource.Default.ZoneLocations.Any(zone => zone.ZoneId == zoneId);
        }
    }
}