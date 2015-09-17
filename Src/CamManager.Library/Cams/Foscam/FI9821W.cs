//
//  FI9821W.cs
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
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace xCom.CamManager.Foscam
{
	public class FI9821W : AbstractCam
	{
		public FI9821W(IPAddress address, int port, string username, string password)
		{
			Address = address;
			Port = port;
			UserName = username;
			Password = password;
		}

		private string CreateCommandUrl(string command, Tuple<string, string>[] parameters)
		{
			var url = string.Format("http://{0}:{1}/cgi-bin/CGIProxy.fcgi?usr={2}&pwd={3}&cmd={4}", Address, Port, UserName, Password, command);

			var parameterUrl = string.Empty;
			if(parameters != null && parameters.Count() > 0)
			{
				foreach(var parameter in parameters)
				{
					parameterUrl += string.Format("&{0}={1}", parameter.Item1, parameter.Item2);
				}

				url += parameterUrl;
			}

			return url;
		}

		protected override ResultSet Execute(string command, params Tuple<string, string>[] parameters)
		{
			ResultSet result = null;

			try
			{
				var url = CreateCommandUrl(command, parameters);
				result = ExecuteCommand(url);
			}
			catch(Exception ex)
			{
				ThrowError(ex);
			}
			return result;
		}


		#region PTZ Handling

		public override void MoveUp()
		{
			Execute("ptzMoveUp");
		}

		public override void MoveDown()
		{
			Execute("ptzMoveDown");
		}

		public override void MoveLeft()
		{
			Execute("ptzMoveLeft");
		}

		public override void MoveRight()
		{
			Execute("ptzMoveRight");
		}

		public override void MoveBottomRight()
		{
			Execute("ptzMoveBottomRight");
		}

		public override void MoveBottomLeft()
		{
			Execute("ptzMoveBottomLeft");
		}

		public override void MoveTopLeft()
		{
			Execute("ptzMoveTopLeft");
		}

		public override void MoveTopRight()
		{
			Execute("ptzMoveTopRight");
		}

		public override void StopMove()
		{
			Execute("ptzStopRun");
		}

		public override void MoveToDefaultPos()
		{
			Execute("ptzReset");
		}

		public override IEnumerable<string> GetPresetPointList()
		{
			var result = Execute("getPTZPresetPointList");

			var resultList = new List<string>();
			var count = result.Values.ReadIntValue("cnt");
			for(int i = 0; i < count; i++)
			{
				var tmp = result.Values.ReadValue<string>(string.Format("point{0}", i));
				resultList.Add(tmp);
			}
			return resultList;
		}

		public override void AddPresetPoint(string name)
		{
			var parameter = new Tuple<string, string>("name", name);
			var result = Execute("ptzAddPresetPoint", parameter);
		}

		public override void DeletePresetPoint(string name)
		{
			var parameter = new Tuple<string, string>("name", name);
			var result = Execute("ptzDeletePresetPoint", parameter);
		}

		public override void GotoPresetPoint(string name)
		{
			var parameter = new Tuple<string, string>("name", name);
			var result = Execute("ptzGotoPresetPoint", parameter);
		}

		#endregion

		#region AV Functions

		public override IImageInfo GetImageSetting()
		{
			var result = Execute("getImageSetting");
			return ImageInfo.Parse(result.Values);
		}

		public override void SetBrightness(int brightness)
		{
			if(brightness < 0 || brightness > 100)
				return;
			
			var parameter = new Tuple<string, string>("brightness", brightness.ToString());
			Execute("setBrightness", parameter);
		}

		public override void SetContrast(int contrast)
		{
			if(contrast < 0 || contrast > 100)
				return;
			
			var parameter = new Tuple<string, string>("contrast", contrast.ToString());
			Execute("setContrast", parameter);
		}

		public override void SetHue(int hue)
		{
			if(hue < 0 || hue > 100)
				return;

			var parameter = new Tuple<string, string>("hue", hue.ToString());
			Execute("setHue", parameter);
		}

		public override void SetSaturation(int saturation)
		{
			if(saturation < 0 || saturation > 100)
				return;

			var parameter = new Tuple<string, string>("saturation", saturation.ToString());
			Execute("setSaturation", parameter);
		}

		public override void SetSharpness(int sharpness)
		{
			if(sharpness < 0 || sharpness > 100)
				return;

			var parameter = new Tuple<string, string>("sharpness", sharpness.ToString());
			Execute("setSharpness", parameter);
		}

		public override void ResetImageSetting()
		{
			Execute("resetImageSetting");
		}

		public override IMirrorFlipInfo GetMirrorAndFlipSetting()
		{
			var result = Execute("getMirrorAndFlipSetting");
			return MirrorFlipInfo.Parse(result.Values);
		}

		public override void SetMirrorMode(bool isMirrored)
		{
			var parameter = new Tuple<string, string>("isMirror", isMirrored ? "1" : "0");	
			Execute("mirrorVideo", parameter);
		}

		public override void SetFlipMode(bool isFlipped)
		{
			var parameter = new Tuple<string, string>("isFlip", isFlipped ? "1" : "0");	
			Execute("flipVideo", parameter);
		}

		public override void SetPowerFrequence(PowerFrequence frequence)
		{
			var parameter = new Tuple<string, string>("freq", frequence == PowerFrequence.Hertz60 ? "0" : "1");	
			Execute("setPwrFreq", parameter);
		}

		public override IDictionary<StreamType, IVideoStreamInfo> GetMainVideoStreamInfos()
		{
			return GetVideoStreamInfosInternal("getVideoStreamParam");
		}

		public override IDictionary<StreamType, IVideoStreamInfo> GetSubVideoStreamInfos()
		{
			return GetVideoStreamInfosInternal("getSubVideoStreamParam");
		}

		private IDictionary<StreamType, IVideoStreamInfo> GetVideoStreamInfosInternal(string command)
		{
			var result = Execute(command);

			Dictionary<StreamType, IVideoStreamInfo> infos = new Dictionary<StreamType, IVideoStreamInfo>();
			var values = result.Values;
			for(int i = 0; i < 4; i++)
			{
				var tmp = VideoStreamInfo.Parse(i, values);

				if(i == 0)
					infos.Add(StreamType.HD, tmp);
				else if(i == 1)
					infos.Add(StreamType.Equilibrium, tmp);
				else if(i == 2)
					infos.Add(StreamType.VGA, tmp);
				else if(i == 3)
					infos.Add(StreamType.Custom, tmp);
			}

			return infos;	
		}

		public override void SetMainVideoStreamInfo(StreamType streamType, IVideoStreamInfo streamInfo)
		{
			SetVideoStreamInfoInternal("setVideoStreamParam", streamType, streamInfo);
		}

		public override void SetSubVideoStreamInfo(StreamType streamType, IVideoStreamInfo streamInfo)
		{
			SetVideoStreamInfoInternal("setSubVideoStreamParam", streamType, streamInfo);
		}

		private void SetVideoStreamInfoInternal(string command, StreamType streamType, IVideoStreamInfo streamInfo)
		{			
			var streamTypeParam = new Tuple<string, string>("streamType", ((int)streamType - 1).ToString());
			var resolutionParam = new Tuple<string, string>("resolution", ((int)streamInfo.Resolution).ToString());
			var bitRateParam = new Tuple<string, string>("bitRate", streamInfo.BitRate.ToString());
			var frameRateParam = new Tuple<string, string>("frameRate", streamInfo.FrameRate.ToString());
			var gopParam = new Tuple<string, string>("GOP", streamInfo.GOP.ToString());
			var vbrParam = new Tuple<string, string>("isVBR", streamInfo.IsVBR ? "1" : "0");

			Execute(command, streamTypeParam, resolutionParam, bitRateParam, frameRateParam, gopParam, vbrParam);
		}

		public override StreamType GetMainVideoStreamType()
		{
			return GetVideoStreamTypeInternal("getMainVideoStreamType");
		}

		public override StreamType GetSubVideoStreamType()
		{
			return GetVideoStreamTypeInternal("getSubVideoStreamType");
		}

		private StreamType GetVideoStreamTypeInternal(string command)
		{
			var result = Execute(command);

			var streamType = result.Values.ReadIntValue("streamType");

			if(streamType == 0)
				return StreamType.HD;
			else if(streamType == 1)
				return StreamType.Equilibrium;
			else if(streamType == 2)
				return StreamType.VGA;
			else if(streamType == 3)
				return StreamType.Custom;
			else
				return StreamType.None;
		}

		public override void SetMainVideoStreamType(StreamType streamType)
		{
			SetVideoStreamTypeInternal("setMainVideoStreamType", streamType);
		}

		public override void SetSubVideoStreamType(StreamType streamType)
		{
			SetVideoStreamTypeInternal("setSubVideoStreamType", streamType);
		}

		private void SetVideoStreamTypeInternal(string command, StreamType streamType)
		{
			var parameter = new Tuple<string, string>("streamType", (((int)streamType) - 1).ToString());
			Execute(command, parameter);
		}

		public override void SetSubStreamFormat(StreamFormat streamFormat)
		{
			var parameter = new Tuple<string, string>("format", ((int)streamFormat).ToString());
			var result = Execute("setSubStreamFormat", parameter);
		}

		public override System.IO.Stream GetMJStream()
		{
			SetSubStreamFormat(StreamFormat.MotionJpeg);

			var url = string.Format("http://{0}:{1}/cgi-bin/CGIStream.cgi?usr={2}&pwd={3}&cmd=GetMJStream", Address, Port, UserName, Password);

			return ExecuteRawCommand(url);
		}

		#endregion

		#region Device Management

		public override IDeviceInfo GetDevInfo()
		{
			var result = Execute("getDevInfo");
			return DeviceInfo.Parse(result.Values);
		}

		public override bool OpenInfraLed()
		{
			var result = Execute("openInfraLed");
			return result.Values.ReadBoolValue("ctrlResult");
		}

		public override bool CloseInfraLed()
		{
			var result = Execute("closeInfraLed");
			return result.Values.ReadBoolValue("ctrlResult");
		}

		public override IDeviceState GetDeviceState()
		{
			var result = Execute("getDevState");
			return DeviceState.Parse(result.Values);
		}

        

		#endregion

	}
}





























