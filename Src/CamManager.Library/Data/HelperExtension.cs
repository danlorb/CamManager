//
//  HelperExtension.cs
//
//  Author:
//       Roland Breitschaft <roland.breitschaft@x-company.de>
//
//  Copyright (c) 2015 IT Solutions Roland Breitschaft
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//

using System;
using System.Collections.Generic;

namespace xCom.CamManager
{
	internal static class HelperExtension
	{
		internal static bool ReadBoolValue(this IDictionary<string, string> values, string key, bool defaultValue = false)
		{
			var value = ReadIntValue(values, key);

			if(value != -1)
			{
				if(value == 0)
					return false;
				else
					return true;
			}
			else
				return defaultValue;
		}

		internal static int ReadIntValue(this IDictionary<string, string> values, string key, int defaultValue = -1)
		{
			var value = ReadValue<object>(values, key);
			return value != null ? ChangeType<int>(value) : defaultValue;			
		}

		internal static TReturnType ReadValue<TReturnType>(this IDictionary<string, string> values, string key, TReturnType defaultValue = default(TReturnType)) where TReturnType : class
		{
			string tmp = null;

			if(values.TryGetValue(key, out tmp))
			{
				if(typeof(TReturnType) == typeof(string))
				{
					if(string.IsNullOrEmpty(tmp) && defaultValue != null)
						return defaultValue;
					else
						return (TReturnType)((object)tmp);
				}
				else if(typeof(TReturnType) == typeof(Version))
				{
					Version tmpVersion = null;
					if(Version.TryParse(tmp, out tmpVersion))
						return ChangeType<TReturnType>(tmpVersion);
					else if(defaultValue != null)
						return defaultValue;						
				}
				else
					return ChangeType<TReturnType>(tmp);
			}
			return default(TReturnType);
		}

		private static TReturnObject ChangeType<TReturnObject>(object value)
		{
			var tmp = Convert.ChangeType(value, typeof(TReturnObject));
			return (TReturnObject)tmp;
		}
	}
}






























