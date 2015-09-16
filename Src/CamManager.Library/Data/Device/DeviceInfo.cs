//
//  DeviceInfo.cs
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
using System.Globalization;

namespace xCom.CamManager
{
	public sealed class DeviceInfo : IDeviceInfo
	{
		internal static IDeviceInfo Parse(Dictionary<string, string> values)
		{
			var info = new DeviceInfo();

			info.ProductName = values.ReadValue<string>("productName");
			info.SerialNumber = values.ReadValue<string>("serialNo");
			info.DeviceName = values.ReadValue<string>("devName");
			info.Mac = values.ReadValue<string>("mac");
			info.TimeZone = values.ReadIntValue("timeZone");
			info.FirmwareVersion = values.ReadValue<Version>("firmwareVer");
			info.HardwareVersion = values.ReadValue<Version>("hardwareVer");

			string year = values.ReadValue<string>("year");
			string month = values.ReadValue<string>("mon");
			string day = values.ReadValue<string>("day");
			string hour = values.ReadValue<string>("hour");
			string minute = values.ReadValue<string>("min");
			string second = values.ReadValue<string>("sec");

			info.CurrentDate = DateTime.Parse(string.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2} {3}:{4}:{5}", day, month, year, hour, minute, second));
			return info;
		}

		#region IDeviceInfo implementation

		public string ProductName { get ; private set; }

		public string SerialNumber{ get ; private set; }

		public string DeviceName { get ; private set; }

		public string Mac { get ; private set; }

		public DateTime CurrentDate { get ; private set; }

		public int TimeZone { get ; private set; }

		public Version FirmwareVersion { get ; private set; }

		public Version HardwareVersion { get ; private set; }

		#endregion
	}
}






























