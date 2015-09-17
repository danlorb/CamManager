using System;

namespace xCom.CamManager
{
	public class StreamKindChangedEventArgs : EventArgs
	{
		public bool IsMainStream {
			get;
			private set;
		}

		public StreamKindChangedEventArgs(bool isMainStream)
		{
			IsMainStream = isMainStream;
		}
	}
}

