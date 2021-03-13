using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FWTL.Common.Extensions;
using FWTL.Core.Commands;

namespace FWTL.Common.Setup.Profiles
{
    public class RequestToCommandProfile : Profile
    {
        public RequestToCommandProfile()
        {
        }

        public RequestToCommandProfile(Type type)
        {
            List<Type> classes = type.Assembly.GetTypes().Where(x => !x.IsNested).ToList();
            foreach (Type @class in classes)
            {
                Type[] nestedClasses = @class.GetNestedTypes();
                Type command = nestedClasses.FirstOrDefault(t => typeof(ICommand).IsAssignableFrom(t));
                Type request = nestedClasses.FirstOrDefault(t => command?.IsSubclassOf(t) ?? false);
                if (command.IsNotNull() && request.IsNotNull())
                {
                    CreateMap(request, command).ConstructUsingServiceLocator();
                }
            }
        }
    }
}