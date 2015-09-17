//
//  MessageBoxHelper.cs
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
using Gtk;

namespace xCom.CamManager
{
	internal static class MessageBoxHelper
	{
		internal static void ShowError(Exception ex, params object[] args)
		{
			var message = ex.Message;
			if(ex.InnerException != null)
				message = string.Format("{1} {0}{0}Details:{0}{2}", Environment.NewLine, message, ex.InnerException.Message);
			
			ShowError(message, args);
		}

		internal static void ShowError(string message, params object[] args)
		{
			ShowGeneric(message, args, MessageType.Error, ButtonsType.Ok);
		}

		internal static void ShowInfo(string message, params object[] args)
		{
			ShowGeneric(message, args, MessageType.Info, ButtonsType.Ok);
		}

		internal static void ShowWarning(string message, params object[] args)
		{
			ShowGeneric(message, args, MessageType.Warning, ButtonsType.Ok);
		}

		internal static ResponseType ShowQuestion(string message, params object[] args)
		{
			ResponseType result = ResponseType.No;

			var dlg = CreateDialog(message, args, MessageType.Question, ButtonsType.YesNo);
			dlg.Response += (sender, e) =>
			{
				result = e.ResponseId;
				dlg.Destroy();
			};
			dlg.ShowAll();	

			return result;
		}

		private static void ShowGeneric(string message, object[] args, MessageType messageType, ButtonsType buttonsType)
		{
			var dlg = CreateDialog(message, args, messageType, buttonsType);
			dlg.Response += (sender, e) =>
			{
				dlg.Destroy();
			};
			dlg.ShowAll();	
		}

		private static Gtk.MessageDialog CreateDialog(string message, object[] args, MessageType messageType, ButtonsType buttonsType)
		{
			var dlg = new Gtk.MessageDialog(null, DialogFlags.Modal, messageType, buttonsType, message, args);
			dlg.Title = "xCom - CamManager needs your attention";
			return dlg;
		}
	}
}

