using System;
using MassTransit.Courier;

namespace FWTL.Domain._Extensions
{
    internal static class RoutingSlipBuilderExtensions
    {
        public static void AddActivity<TCommand>(this RoutingSlipBuilder builder, TCommand command)
        {
            string endpointName = typeof(TCommand).FullName.Replace(".", "").Replace("+", "");
            builder.AddActivity(endpointName, new Uri($"queue:{endpointName}"), command);
        }
    }
}