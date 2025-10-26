﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumv = enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .FirstOrDefault();
            if (enumv != null)
                return enumv.GetCustomAttribute<DisplayAttribute>()
                             .GetName();

            return enumValue.ToString();
        }
    }
}
