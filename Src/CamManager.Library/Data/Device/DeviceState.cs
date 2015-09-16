//
//  DeviceState.cs
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

namespace xCom.CamManager
{
	public sealed class DeviceState : IDeviceState
	{
		#region IDeviceState implementation

		public AlarmType IOAlarm { get; private set; }

		public AlarmType MotionDetectAlarm { get; private set; }

		public AlarmType SoundAlarm { get; private set; }

		public bool IsRecording { get; private set; }

		public SDCardState CardState { get; private set; }

		public string CardFreeSpace { get; private set; }

		public string CardTotalSpace { get; private set; }

		public NtpState NtpState { get; private set; }

		public DDNSState DDNSState { get; private set; }

		public Uri DDNSUrl { get; private set; }

		public UPnPState UPnPState { get; private set; }

		public bool IsWifiConnected { get; private set; }

		public string WifiSSID { get; private set; }

		public bool InfraLedState { get; private set; }

		#endregion
		
	}
}






























