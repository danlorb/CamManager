//
//  CamVideoSettings.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace xCom.CamManager
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CamVideoSettings : BaseWidget
	{
		private StreamType _mainStreamType;
		private StreamType _subStreamType;

		private IDictionary<StreamType, IVideoStreamInfo> _mainVideoInfos;
		private IDictionary<StreamType, IVideoStreamInfo> _subVideoInfos;

		private Dictionary<string, int> _bitrates;
		private string[] _mainBitrates = new string[] { "4M", "2M", "1M", "512K", "256K", "200K", "128K", "100K" };
		private string[] _subBitrates = new string[] { "512K", "256K", "200K", "128K", "100K", "50K", "20K" };

		private bool _isCurrentMainStream;

		public event EventHandler<StreamKindChangedEventArgs> StreamKindChanged;

		public CamVideoSettings()
		{
			_isCurrentMainStream = true;

			_bitrates = new Dictionary<string, int>();
			_bitrates.Add("4M", 4194304);
			_bitrates.Add("2M", 2097152);
			_bitrates.Add("1M", 1048576);
			_bitrates.Add("512K", 524288);
			_bitrates.Add("256K", 262144);
			_bitrates.Add("200K", 204800);
			_bitrates.Add("128K", 131072);
			_bitrates.Add("100K", 102400);
			_bitrates.Add("50K", 51200);
			_bitrates.Add("20K", 20480);

			this.Shown += (sender, e) =>
			{
				CultureManager.RegisterWidgets(this, lblStreamKind, lblStreamType, lblBitrate, lblFramerate, lblGroupOfPics, lblResolution, lblVariableBitrate, btnSave, btnSetStreamType);
			};
			this.Build();
		}

		protected override void OnSettingsObjChanged(CamSettings settings)
		{
			cmbStreamKind.Active = 0;
		}

		private void RefreshStreamType()
		{
			if(InitCam())
			{
				_mainStreamType = Cam.GetMainVideoStreamType();
				_subStreamType = Cam.GetSubVideoStreamType();
			}
		}

		protected void OnCmbStreamKindChanged(object sender, EventArgs e)
		{
			RefreshStreamType();
			FillControls();

			if(IsMainStream())
				cmbStreamType.Active = ((int)_mainStreamType) - 1;
			else
				cmbStreamType.Active = ((int)_subStreamType) - 1;
		}

		protected void OnCmbStreamTypeChanged(object sender, EventArgs e)
		{
			if(InitCam())
			{
				_mainVideoInfos = Cam.GetMainVideoStreamInfos();
				_subVideoInfos = Cam.GetSubVideoStreamInfos();

				IVideoStreamInfo info = null;

				if(IsMainStream())
				{
					_mainStreamType = (StreamType)Enum.Parse(typeof(StreamType), (cmbStreamType.Active + 1).ToString());
					info = _mainVideoInfos[_mainStreamType];
				}
				else
				{
					_subStreamType = (StreamType)Enum.Parse(typeof(StreamType), (cmbStreamType.Active + 1).ToString());
					info = _subVideoInfos[_subStreamType];
				}					

				LoadValues(info);
			}
		}

		protected void OnBtnSetStreamTypeClicked(object sender, EventArgs e)
		{
			if(InitCam())
			{
				if(IsMainStream())
				{
					_mainStreamType = (StreamType)Enum.Parse(typeof(StreamType), (cmbStreamType.Active + 1).ToString());
					Cam.SetMainVideoStreamType(_mainStreamType);
				}
				else
				{
					_subStreamType = (StreamType)Enum.Parse(typeof(StreamType), (cmbStreamType.Active + 1).ToString());
					Cam.SetSubVideoStreamType(_subStreamType);
				}
			}	

			EventHandler<StreamKindChangedEventArgs> tmp = StreamKindChanged;
			if(tmp != null)
			{
				if(_isCurrentMainStream != IsMainStream())
				{
					_isCurrentMainStream = IsMainStream();
					tmp(this, new StreamKindChangedEventArgs(_isCurrentMainStream));
				}
			}
		}

		protected void OnBtnSaveClicked(object sender, EventArgs e)
		{
			SaveValues();
		}

		private void FillControls()
		{
			// Fülle die GOP von 10 - 100
			Gtk.ListStore model = (Gtk.ListStore)cmbGroupOfPics.Model;
			model.Clear();
			for(int i = 10; i <= 100; i++)
			{
				cmbGroupOfPics.AppendText(i.ToString());
			}

			// Fülle die Framerate von 1-30, wenn im Hauptstream, ansonsten nur 10
			model = (Gtk.ListStore)cmbFramerate.Model;
			model.Clear();
			var framerateMax = IsMainStream() ? 30 : 10;
			for(int i = 1; i <= framerateMax; i++)
			{
				cmbFramerate.AppendText(i.ToString());
			}

			// Fülle die Bitraten
			var rates = IsMainStream() ? _mainBitrates : _subBitrates;
			model = (Gtk.ListStore)cmbBitrate.Model;
			model.Clear();
			for(int i = 0; i < rates.Length; i++)
			{
				cmbBitrate.AppendText(rates[i]);
			}
		}

		private void LoadValues(IVideoStreamInfo info)
		{
			if(info == null)
				return;

			// Resolution
			cmbResolution.Active = ((int)info.Resolution) - 1;
			if(info.Resolution == Resolution.HD_720)
				cmbResolution.Active = 0;
			else if(info.Resolution == Resolution.VGA_640_480)
				cmbResolution.Active = 1;
			else if(info.Resolution == Resolution.VGA_640_360)
				cmbResolution.Active = 2;
			else if(info.Resolution == Resolution.QVGA_320_240)
				cmbResolution.Active = 3;
			else if(info.Resolution == Resolution.QVGA_320_180)
				cmbResolution.Active = 4;			

			// Bitrate
			var kvp = _bitrates.FirstOrDefault(x => x.Value == info.BitRate);
			cmbBitrate.Active = IsMainStream() ? _mainBitrates.ToList().IndexOf(kvp.Key) : _subBitrates.ToList().IndexOf(kvp.Key);

			// Framerate
			cmbFramerate.Active = info.FrameRate - 1;

			// GOP
			cmbGroupOfPics.Active = info.GOP - 10;

			// VBR
			cmbVariableBitrate.Active = info.IsVBR ? 0 : 1;
		}

		private void SaveValues()
		{
			var info = new VideoStreamInfo();

			info.IsVBR = cmbVariableBitrate.Active == 0 ? true : false;
			info.GOP = cmbGroupOfPics.Active + 10;
			info.FrameRate = cmbFramerate.Active + 1;

			var value = IsMainStream() ? _mainBitrates[cmbBitrate.Active] : _subBitrates[cmbBitrate.Active];
			var kvp = _bitrates.FirstOrDefault(x => string.Compare(x.Key.ToUpper(), value.ToUpper()) == 0);
			info.BitRate = kvp.Value;

			if(cmbResolution.Active == 0)
				info.Resolution = Resolution.HD_720;
			else if(cmbResolution.Active == 1)
				info.Resolution = Resolution.VGA_640_480;
			else if(cmbResolution.Active == 2)
				info.Resolution = Resolution.VGA_640_360;
			else if(cmbResolution.Active == 3)
				info.Resolution = Resolution.QVGA_320_240;
			else if(cmbResolution.Active == 4)
				info.Resolution = Resolution.QVGA_320_180;

			if(InitCam())
			{
				if(IsMainStream())
					Cam.SetMainVideoStreamInfo(_mainStreamType, info);
				else
					Cam.SetSubVideoStreamInfo(_subStreamType, info);
			}
		}

		private bool IsMainStream()
		{
			return cmbStreamKind.Active == 0 ? true : false;
		}
	}
}

