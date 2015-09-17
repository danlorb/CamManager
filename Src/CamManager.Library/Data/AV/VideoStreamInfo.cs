//
//  VideoStreamInfo.cs
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

namespace xCom.CamManager
{
	public sealed class VideoStreamInfo : IVideoStreamInfo
	{
		internal static IVideoStreamInfo Parse(int index, Dictionary<string, string> values)
		{
			var info = new VideoStreamInfo();

			info.Resolution = (Resolution)Enum.Parse(typeof(Resolution), values.ReadValue<string>(string.Format("resolution{0}", index)));
			info.BitRate = values.ReadIntValue(string.Format("bitRate{0}", index));
			info.FrameRate = values.ReadIntValue(string.Format("frameRate{0}", index));
			info.GOP = values.ReadIntValue(string.Format("GOP{0}", index));
			info.IsVBR = values.ReadIntValue(string.Format("isVBR{0}", index)) == 1 ? true : false;

			return info;
		}

		#region IVideoStreamInfo implementation

		public Resolution Resolution { get; set; }

		public int BitRate { get; set; }

		public int FrameRate { get; set; }

		public int GOP { get; set; }

		public bool IsVBR { get; set; }

		#endregion
	}
}






























