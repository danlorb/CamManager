//
//  ICam.cs
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
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace xCom.CamManager
{
	public interface ICam
	{
		#region PTZ Handling

		void MoveUp();

		void MoveDown();

		void MoveLeft();

		void MoveRight();

		void MoveTopLeft();

		void MoveTopRight();

		void MoveBottomLeft();

		void MoveBottomRight();

		void StopMove();

		void MoveToDefaultPos();

		Speed GetSpeed();

		void SetSpeed(Speed speed);

		IEnumerable<string> GetPresetPointList();

		void AddPresetPoint(string name);

		void DeletePresetPoint(string name);

		void GotoPresetPoint(string name);

		IEnumerable<string> GetCruiseMapList();

		bool TryGetCruiseMapInfo(string name, out IEnumerable<string> presetPoints);

		void SetCruiseMap(string name, IEnumerable<string> presetPoints);

		void DeleteCruiseMap(string name);

		void StartCruiseMap(string name);

		void StopCruiseMap(string name);

		/* Currently Not used
		 
		void ZoomIn ();

		void ZoomOut ();

		void ZoomStop ();

		ZoomSpeed GetZoomSpeed ();

		void SetZoomSpeed (ZoomSpeed speed);

		void SetSelfTest (SelfTestMode mode);

		SelfTestMode GetSelfTest ();

		void SetPrePointSelfTest (string name);

		string GetPrePointSelfTest ();

		void SetRS485Info (IRS485Info info);

		IRS485Info GetRS485Info ();
		
		*/

		#endregion

		#region AV Function

		IImageInfo GetImageSetting();

		void SetBrightness(int brightness);

		void SetContrast(int contrast);

		void SetHue(int hue);

		void SetSaturation(int saturation);

		void SetSharpness(int sharpness);

		void ResetImageSetting();

		IMirrorFlipInfo GetMirrorAndFlipSetting();

		void SetMirrorMode(bool isMirrored);

		void SetFlipMode(bool isFlipped);

		void SetPowerFrequence(PowerFrequence frequence);

		IDictionary<StreamType, IVideoStreamInfo> GetMainVideoStreamInfos();

		IDictionary<StreamType, IVideoStreamInfo> GetSubVideoStreamInfos();

		void SetMainVideoStreamInfo(StreamType streamType, IVideoStreamInfo streamInfo);

		void SetSubVideoStreamInfo(StreamType streamType, IVideoStreamInfo streamInfo);

		StreamType GetMainVideoStreamType();

		StreamType GetSubVideoStreamType();

		void SetMainVideoStreamType(StreamType streamType);

		void SetSubVideoStreamType(StreamType streamType);

		Stream GetMJStream();

		/* Currently not used
		 
		IOSDSetting GetOSDSetting ();

		void SetOSDSetting (IOSDSetting setting);

		IEnumerable<IOSDMaskArea> GetOSDMaskArea ();

		void SetOSDMaskArea (IEnumerable<IOSDMaskArea> masks);

		IMotionDetectConfig GetMotionDetectConfig ();

		void SetMotionDetectConfig (IMotionDetectConfig config);

		ISnapConfig GetSnapConfig ();

		void SetSnapConfig (ISnapConfig config);

		string SnapPictureAsHtml ();

		string SnapPictureAsRaw();

		void GetRecordList(string recordPath, DateTime startTime, DateTime endTime, int recordType, int startNumber);

		IAlarmRecord GetAlarmRecordConfig ();

		bool SetAlarmRecordConfig(IAlarmRecord record);

		IIOAlarmRecord GetIOAlarmConfig();

		bool SetIOAlarmConfig (IIOAlarmRecord record);

		void ClearIOAlarmOutput();

		IEnumerable<string> GetMultiDevList();

		IMultiDevInfo GetMultiDeviceInfo (int channel);

		bool AddMultiDevice(int channel, IMultiDevInfo info);

		bool DeleteMultiDevice(int channel);
		*/

		#endregion

		#region User Account

		/* Currently not used
		 
		void AddUser (string userName, string password, Privileg privileg);

		void DeleteUser(string userName);

		void ChangePassword(string userName, string oldPassword, string newPassword);

		void ChangeUserName(string userName, string newUserName);

		ILoginState Login(string userName, IPAddress address, int groupId);

		void Logout (string userName, IPAddress address, int groupId);

		IEnumerable<string> GetSessionList ();

		IEnumerable<string> GetUserList();

		bool UserBeetHeart(string userName, IPAddress remoteIp, int groupId);

		*/

		#endregion

		#region Network

		/* Currently not used
		 
		IIpInfo GetIpInfo ();

		void SetIpInfo (IIpInfo info);

		void RefreshWifiList ();

		IEnumerable<IAccessPoint> GetWifiList (int startNumber);

		void SetWifiSetting (IAccessPoint accesspoint);

		IAccessPoint GetWifiSetting ();

		IPortInfo GetPortInfo ();

		void SetPortInfo (IPortInfo info);

		bool IsUPnPEnabled();

		void SetUPnP (bool isEnabled);

		IDDNSConfig GetDDNSConfig ();

		void SetDDNSConfig(IDDNSConfig config);

		void SetFtpConfig(IFtpConfig config);

		IFtpConfig GetFtpConfig();

		bool TestFtpServer(IFtpConfig config);

		ISmtpConfig GetSmtpConfig();

		void SetSmtpConfig(ISmtpConfig config);

		bool TestSmtpConfig(ISmtpConfig config);

		*/

		#endregion

		#region Device Management

		/* Currently not used
		 
		void SetSystemTime(IDateConfig config);

		IDateConfig GetSystemTime();

		*/

		bool OpenInfraLed();

		bool CloseInfraLed();

		bool IsInfraLedInAutoMode();

		void SetInfraLedMode(bool isAutoMode);

		IDeviceState GetDeviceState();

		string GetDeviceName();

		void SetDeviceName(string deviceName);

		IDeviceInfo GetDevInfo();

		#endregion

		#region System

		void Reboot();

		/* Currently not used
		 
		void RestoreToFactorySettings();

		string ExportConfig();

		bool FirmwareUpgrade();

		*/

		#endregion

		#region Misc

		/* Currently not used
		 
		IFirewallConfig GetFirewallConfig ();

		void SetFirewallConfig (IFirewallConfig config);

		IEnumerable<ILogEntry> GetLog (int offset, int count);

		*/

		#endregion
	}
}






























