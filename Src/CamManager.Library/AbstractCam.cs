//
//  AbstractCam.cs
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
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace xCom.CamManager
{
	public abstract class AbstractCam : ICam
	{
		private Action<Exception> _handler;

		protected IPAddress Address { get; set; }

		protected int Port { get; set; }

		protected string UserName { get; set; }

		protected string Password { get; set; }

		protected abstract ResultSet Execute(string command, params Tuple<string, string>[] parameters);

		protected ResultSet ExecuteCommand(string url)
		{
			var result = new ResultSet();
			var client = new WebClient();

			try
			{
				using (var stream = new StreamReader(client.OpenRead(url)))
				{
					var tmp = stream.ReadToEnd();
					result = ParseResult(tmp);
				}
			}
			catch(Exception ex)
			{
				throw new CamException("An unspecified Error while processing is occured.", ex);
			}

			if(result.State == ResultState.Success)
				return result;
			else if(result.State == ResultState.AccessDeny)
				throw new CamException("Access denied");
			else if(result.State == ResultState.CGIExecuteFail)
				throw new CamException("CGI Execution failed");
			else if(result.State == ResultState.RequestFormatError)
				throw new CamException("Parameters Format Error");
			else if(result.State == ResultState.Timeout)
				throw new CamException("An Timeout is occured");
			else if(result.State == ResultState.UserNameOrPasswordWrong)
				throw new CamException("User or Password is wrong.");
			else if(result.State == ResultState.UnknownError)
				throw new CamException("An unknown Error is occured.");
			else
				throw new CamException("An unspecified Error is occured");
		}

		protected Stream ExecuteRawCommand(string url)
		{
			System.IO.Stream stream = null;
			try
			{
				var client = new WebClient();
				stream = client.OpenRead(url);
			}
			catch(Exception ex)
			{
				throw new CamException("An unspecified Error while opening Stream is occured.", ex);
			}

			return stream;
		}

		protected ResultSet ParseResult(string cgiResultString)
		{
			var result = new ResultSet();

			if(string.IsNullOrEmpty(cgiResultString))
			{
				result.State = ResultState.UnknownError;
				return result;
			}

			var values = new Dictionary<string, string>();
			var xDoc = XDocument.Parse(cgiResultString);
			xDoc.Element("CGI_Result").Elements().ToList().ForEach(x =>
			{
				var key = x.Name.ToString();
				var value = x.Value;
				values.Add(key, value);
			});

			var stateString = string.Empty;
			if(values.TryGetValue("result", out stateString))
			{
				var state = ResultState.None;
				if(Enum.TryParse<ResultState>(stateString, out state))
				{
					result.State = state;
					values.Remove("result");
				}
			}

			result.Values = values;
			return result;
		}

		protected void ThrowError(Exception ex)
		{
			if(_handler != null)
				_handler(ex);
		}

		internal void SetErrorHandler(Action<Exception> handler)
		{
			_handler = handler;
		}

		#region PTZ Handling

		public virtual void MoveUp()
		{
		}

		public virtual void MoveDown()
		{
		}

		public virtual void MoveLeft()
		{

		}

		public virtual void MoveRight()
		{

		}

		public virtual void MoveTopLeft()
		{

		}

		public virtual void MoveTopRight()
		{

		}

		public virtual void MoveBottomLeft()
		{

		}

		public virtual void MoveBottomRight()
		{

		}

		public virtual void StopMove()
		{
		}

		public virtual void MoveToDefaultPos()
		{

		}

		public virtual Speed GetSpeed()
		{
			return Speed.None;
		}

		public virtual void SetSpeed(Speed speed)
		{

		}

		public virtual IEnumerable<string> GetPresetPointList()
		{
			return new List<string>();
		}

		public virtual  void AddPresetPoint(string name)
		{

		}

		public virtual void DeletePresetPoint(string name)
		{

		}

		public virtual void GotoPresetPoint(string name)
		{

		}

		public virtual IEnumerable<string> GetCruiseMapList()
		{
			return new List<string>();
		}

		public virtual  bool TryGetCruiseMapInfo(string name, out IEnumerable<string> presetPoints)
		{
			presetPoints = new List<string>();

			return false;
		}

		public virtual void SetCruiseMap(string name, IEnumerable<string> presetPoints)
		{

		}

		public virtual void DeleteCruiseMap(string name)
		{

		}

		public virtual  void StartCruiseMap(string name)
		{

		}

		public virtual void StopCruiseMap(string name)
		{

		}

		#endregion

		#region AV Functions

		public virtual  IImageInfo GetImageSetting()
		{
			return new ImageInfo();
		}

		public virtual void SetBrightness(int brightness)
		{

		}

		public virtual void SetContrast(int contrast)
		{

		}

		public virtual void SetHue(int hue)
		{

		}

		public virtual void SetSaturation(int saturation)
		{

		}

		public virtual void SetSharpness(int sharpness)
		{

		}

		public virtual void ResetImageSetting()
		{

		}

		public virtual IMirrorFlipInfo GetMirrorAndFlipSetting()
		{
			return new MirrorFlipInfo();
		}

		public virtual void SetMirrorMode(bool isMirrored)
		{

		}

		public virtual void SetFlipMode(bool isFlipped)
		{

		}

		public virtual void SetPowerFrequence(PowerFrequence frequence)
		{

		}

		public virtual IDictionary<StreamType, IVideoStreamInfo> GetMainVideoStreamInfos()
		{
			return new Dictionary<StreamType, IVideoStreamInfo>();
		}

		public virtual IDictionary<StreamType, IVideoStreamInfo> GetSubVideoStreamInfos()
		{
			return new Dictionary<StreamType, IVideoStreamInfo>();
		}

		public virtual void SetMainVideoStreamInfo(StreamType streamType, IVideoStreamInfo streamInfo)
		{

		}


		public virtual void SetSubVideoStreamInfo(StreamType streamType, IVideoStreamInfo streamInfo)
		{

		}

		public  virtual StreamType GetMainVideoStreamType()
		{
			return StreamType.None;
		}

		public virtual  StreamType GetSubVideoStreamType()
		{
			return StreamType.None;
		}

		public virtual void SetMainVideoStreamType(StreamType streamType)
		{

		}

		public virtual void SetSubVideoStreamType(StreamType streamType)
		{

		}

		public virtual void SetSubStreamFormat(StreamFormat streamFormat)
		{

		}

		public virtual  Stream GetMJStream()
		{
			return null;
		}


		#endregion

		#region Device Management

		public virtual  bool OpenInfraLed()
		{
			return false;
		}

		public virtual  bool CloseInfraLed()
		{
			return false;
		}

		public virtual  bool IsInfraLedInAutoMode()
		{
			return false;
		}

		public virtual void SetInfraLedMode(bool isAutoMode)
		{

		}

		public virtual  IDeviceState GetDeviceState()
		{
			return new DeviceState();
		}

		public  virtual string GetDeviceName()
		{
			return string.Empty;
		}

		public virtual void SetDeviceName(string deviceName)
		{

		}

		public virtual IDeviceInfo GetDevInfo()
		{
			return new DeviceInfo();
		}

		#endregion

		#region System

		public virtual void Reboot()
		{

		}

		#endregion
	}
}






























