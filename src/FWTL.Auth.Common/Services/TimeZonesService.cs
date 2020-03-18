using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FWTL.Core.Services;
using NodaTime.TimeZones;

namespace FWTL.Common.Services
{
    public class TimeZonesService : ITimeZonesService
    {
        public IEnumerable<string> GetAll()
        {
            return TzdbDateTimeZoneSource.Default.ZoneLocations.Select(zone => zone.ZoneId).ToList();
        }

        public bool Exist(string zoneId)
        {
            return TzdbDateTimeZoneSource.Default.ZoneLocations.Any(zone => zone.ZoneId == zoneId);
        }
    }
}
