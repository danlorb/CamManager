//
//  CamNavigation.cs
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

namespace xCom.CamManager
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CamNavigation : BaseWidget
	{
		public CamNavigation()
		{
			this.Shown += (sender, e) =>
			{
				CultureManager.RegisterWidgets(this, btnBottom, btnBottomLeft, btnBottomRight, btnLeft, btnMiddle, btnRight, btnTop, btnTopLeft, btnTopRight, chkFlip, chkMirror, chkNightMode);
			};
			this.Build();
		}

		protected override void OnSettingsObjChanged(CamSettings settings)
		{
			if(InitCam())
			{
				var setting = Cam.GetMirrorAndFlipSetting();
				chkFlip.Active = setting.IsFlipMode;
				chkMirror.Active = setting.IsMirrorMode;

				var state = Cam.GetDeviceState();
				chkNightMode.Active = state.InfraLedState;

				var mirrorFlipSetting = Cam.GetMirrorAndFlipSetting();
				chkFlip.Active = mirrorFlipSetting.IsFlipMode;
				chkMirror.Active = mirrorFlipSetting.IsMirrorMode;

				LoadPresets();
			}		
		}

		internal void StopMove()
		{			
			if(Cam != null)
				Cam.StopMove();
			
			DestroyCam();
		}

		protected void OnBtnBottomRightPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveBottomRight();
		}

		protected void OnBtnBottomRightReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		internal void OnBtnBottomPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveDown();
		}

		protected void OnBtnBottomReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		protected void OnBtnBottomLeftPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveBottomLeft();
		}

		protected void OnBtnBottomLeftReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		internal void OnBtnRightPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveRight();
		}

		protected void OnBtnRightReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		internal void OnBtnLeftPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveLeft();
		}

		protected void OnBtnLeftReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		protected void OnBtnTopRightPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveTopRight();
		}

		protected void OnBtnTopRightReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		internal void OnBtnTopPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveUp();
		}

		protected void OnBtnTopReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		protected void OnBtnTopLeftPressed(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveTopLeft();
		}

		protected void OnBtnTopLeftReleased(object sender, EventArgs e)
		{
			StopMove();
		}

		protected void OnBtnMiddleClicked(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.MoveToDefaultPos();
		}

		protected void OnChkNightModeToggled(object sender, EventArgs e)
		{
			if(InitCam())
			{
				var state = Cam.GetDeviceState();

				if(chkNightMode.Active)
				{
					if(!state.InfraLedState)
						Cam.OpenInfraLed();
				}
				else if(state.InfraLedState)
					Cam.CloseInfraLed();
			}
		}

		protected void OnChkFlipToggled(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.SetFlipMode(chkFlip.Active);
		}

		protected void OnChkMirrorToggled(object sender, EventArgs e)
		{
			if(InitCam())
				Cam.SetMirrorMode(chkMirror.Active);
		}

		private void LoadPresets()
		{
			var presets = Cam.GetPresetPointList();
			Gtk.ListStore model = (Gtk.ListStore)cmbPresetSettings.Model;
			model.Clear();

			presets.ToList().ForEach(x => cmbPresetSettings.AppendText(x));
		}

		protected void OnBtnAddPresetClicked(object sender, EventArgs e)
		{
			var dlg = new PresetDialog();
			dlg.Response += (o, args) =>
			{
				if(args.ResponseId == Gtk.ResponseType.Ok)
				{
					if(InitCam())
					{
						Cam.AddPresetPoint(dlg.PresetName);
						LoadPresets();
					}
				}
			};
			dlg.Show();
		}

		protected void OnBtnRemovePresetClicked(object sender, EventArgs e)
		{
			var presetName = cmbPresetSettings.ActiveText;
			if(InitCam())
			{
				Cam.DeletePresetPoint(presetName);
				LoadPresets();
			}
		}

		protected void OnBtnPlayPresetClicked(object sender, EventArgs e)
		{
			var presetName = cmbPresetSettings.ActiveText;
			if(InitCam())
				Cam.GotoPresetPoint(presetName);
		}
	}
}

