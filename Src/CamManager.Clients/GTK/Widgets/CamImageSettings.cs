//
//  CamImageSettings.cs
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
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CamImageSettings : BaseWidget
	{
		private bool _isInit;

		public CamImageSettings()
		{
			this.Shown += (sender, e) =>
			{
				CultureManager.RegisterWidgets(this, lblBrightness, lblHue, lblSaturation, lblSharpness, btnReset);
			};
			this.Build();
		}

		protected override void OnSettingsObjChanged(CamSettings settings)
		{
			_isInit = true;
			if(InitCam())
			{
				var imageSetting = Cam.GetImageSetting();
				hscaleBrightness.Value = imageSetting.Brightness;
				hscaleHue.Value = imageSetting.Hue;
				hscaleSaturation.Value = imageSetting.Saturation;
				hscaleSharpness.Value = imageSetting.Sharpness;
			}
			_isInit = false;
		}

		protected void OnHscaleSharpnessValueChanged(object sender, EventArgs e)
		{
			if(!_isInit && InitCam())
				Cam.SetSharpness((int)hscaleSharpness.Value);	
		}

		protected void OnHscaleSaturationValueChanged(object sender, EventArgs e)
		{
			if(!_isInit && InitCam())
				Cam.SetSaturation((int)hscaleSaturation.Value);	
		}

		protected void OnHscaleHueValueChanged(object sender, EventArgs e)
		{
			if(!_isInit && InitCam())
				Cam.SetHue((int)hscaleHue.Value);	
		}

		protected void OnHscaleBrightnessValueChanged(object sender, EventArgs e)
		{
			if(!_isInit && InitCam())
				Cam.SetBrightness((int)hscaleBrightness.Value);
		}

		protected void OnBtnResetClicked(object sender, EventArgs e)
		{
			if(InitCam())
			{
				Cam.ResetImageSetting();
				OnSettingsObjChanged(SettingsObj);
			}
		}
	}
}

