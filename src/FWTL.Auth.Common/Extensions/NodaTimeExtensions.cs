using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace FWTL.Common.Extensions
{
    //Temporary solution until NodaTime.Serialization.JsonNet new version 
    public static class NodaTimeExtensions
    {
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings,
            IDateTimeZoneProvider provider)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            // Add our converters
            AddDefaultConverters(settings.Converters, provider);

            // Disable automatic conversion of anything that looks like a date and time to BCL types.
            settings.DateParseHandling = DateParseHandling.None;

            // return to allow fluent chaining if desired
            return settings;
        }

        private static void AddDefaultConverters(IList<JsonConverter> converters, IDateTimeZoneProvider provider)
        {
            converters.Add(NodaConverters.InstantConverter);
            converters.Add(NodaConverters.IntervalConverter);
            converters.Add(NodaConverters.LocalDateConverter);
            converters.Add(NodaConverters.LocalDateTimeConverter);
            converters.Add(NodaConverters.LocalTimeConverter);
            //converters.Add(NodaConverters.AnnualDateConverter);
            converters.Add(NodaConverters.DateIntervalConverter);
            converters.Add(NodaConverters.OffsetConverter);
            converters.Add(NodaConverters.CreateDateTimeZoneConverter(provider));
            converters.Add(NodaConverters.DurationConverter);
            converters.Add(NodaConverters.RoundtripPeriodConverter);
            converters.Add(NodaConverters.OffsetDateTimeConverter);
            converters.Add(NodaConverters.OffsetDateConverter);
            converters.Add(NodaConverters.OffsetTimeConverter);
            converters.Add(NodaConverters.CreateZonedDateTimeConverter(provider));
        }
    }
}