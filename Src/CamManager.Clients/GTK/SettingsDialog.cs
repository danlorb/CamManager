//
//  SettingsDialog.cs
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
	public partial class SettingsDialog : Gtk.Dialog
	{
		public SettingsDialog()
		{
			this.Shown += (sender, e) =>
			{
				CultureManager.RegisterWidgets(this, lblDefault, lblLanguage, lblMPlayer, btnCancel, btnOk);

				var lang = SettingsManager.LoadSetting("lang");
				if(string.Compare(lang.ToUpper(), "EN-US") == 0)
					cmbLanguage.Active = 0;
				else if(string.Compare(lang.ToUpper(), "DE-DE") == 0)
					cmbLanguage.Active = 1;

				txtMPlayer.Text = SettingsManager.LoadSetting("mplayer");
				txtMPlayerParams.Text = SettingsManager.LoadSetting("mplayer_params");

				CultureManager.UpdateCulture(this);
			};
			this.Build();
		}

		protected void OnCmbLanguageChanged(object sender, EventArgs e)
		{
			if(this.IsRealized)
			{
				if(cmbLanguage.Active == 0)
					SettingsManager.SaveSetting("lang", "en-US");
				else if(cmbLanguage.Active == 1)
					SettingsManager.SaveSetting("lang", "de-DE");

				CultureManager.ChangeCulture();
			}
		}

		protected void OnResponse(object sender, Gtk.ResponseArgs args)
		{
			if(args.ResponseId == Gtk.ResponseType.Ok)
			{
				SettingsManager.SaveSetting("mplayer", txtMPlayer.Text);
				SettingsManager.SaveSetting("mplayer_params", txtMPlayerParams.Text);
				SettingsManager.SaveSettingsFile();
			}
		}
	}
}

