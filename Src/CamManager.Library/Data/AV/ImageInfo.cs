//
//  ImageInfo.cs
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
	public sealed class ImageInfo : IImageInfo
	{
		internal static IImageInfo Parse(Dictionary<string, string> values)
		{
			var info = new ImageInfo();

			info.Brightness = values.ReadIntValue("brightness");
			info.Contrast = values.ReadIntValue("contrast");
			info.Hue = values.ReadIntValue("hue");
			info.Saturation = values.ReadIntValue("saturation");
			info.Sharpness = values.ReadIntValue("sharpness");
			info.DenoiseLevel = values.ReadIntValue("denoiseLevel");

			return info;
		}

		#region IImageInfo implementation

		public int Brightness { get; private set; }

		public int Contrast { get; private set; }

		public int Hue  { get; private set; }

		public int Saturation  { get; private set; }

		public int Sharpness  { get; private set; }

		public int DenoiseLevel  { get; private set; }

		#endregion
	}
}






























