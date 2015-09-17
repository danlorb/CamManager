//
//  MPlayerHelper.cs
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
using System.Diagnostics;
using System.Threading;
using Gtk;

namespace xCom.CamManager
{
	internal sealed class MPlayerHelper
	{
		private CamSettings _settings;
		private Process _mplayerProcess;

		public event EventHandler Stopped;

		//		private Thread _outputThread;
		private Thread _errorThread;

		//		private Socket _socket;
		//		private int _sockedId;
		//
		//		private int _videoHeight;
		//		private int _videoWidth;

		public MPlayerHelper(CamSettings settings)
		{
			_settings = settings;
		}

		internal void Play()
		{
			if(_mplayerProcess != null)
				Stop();

			var mplayerLocation = SettingsManager.LoadSetting("mplayer");
			var url = CreateMPlayerUrl();

			var psi = new ProcessStartInfo(mplayerLocation, url);
			psi.RedirectStandardInput = true; 
			psi.RedirectStandardOutput = false;
			psi.RedirectStandardError = true;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.WindowStyle = ProcessWindowStyle.Hidden;

			_mplayerProcess = new Process();
			_mplayerProcess.StartInfo = psi;
			_mplayerProcess.Exited += new EventHandler(processExited);
			_mplayerProcess.Start();

//			_outputThread = new Thread(readOutputProc);
//			_outputThread.Start();

			_errorThread = new Thread(readErrorProc);
			_errorThread.Start();
		}

		internal void Stop()
		{
			Stop(true);
		}

		private void Stop(bool raiseStopEvent)
		{
			try
			{
				if(_mplayerProcess != null)
					_mplayerProcess.Kill();

				try
				{
					if(_mplayerProcess != null && !_mplayerProcess.HasExited)
						Mono.Unix.Native.Syscall.kill(_mplayerProcess.Id, Mono.Unix.Native.Signum.SIGABRT);
				}
				catch
				{
				}
			}
			catch
			{
				//MessageBoxHelper.ShowError(ex);
			}
			finally
			{
//				if(_outputThread != null)
//				{
//					this._outputThread.Abort();
//					this._outputThread = null;
//				}

				if(_errorThread != null)
				{
					this._errorThread.Abort();
					this._errorThread = null;
				}

				_mplayerProcess = null;

				if(raiseStopEvent)
					OnStopped(this, EventArgs.Empty);
			}
		}

		internal void ReStart(CamSettings settings)
		{
			_settings = settings;
			Stop(false);
			Play();
		}

		private void processExited(object sender, EventArgs args)
		{
			Stop();
			_mplayerProcess = null;
		}

		private void readErrorProc()
		{
			try
			{
				while(_mplayerProcess != null && !_mplayerProcess.HasExited)
				{
					string line = null;
					try
					{
						line = _mplayerProcess.StandardError.ReadLine();
//						Gtk.Application.Invoke(delegate
//						{
//							txtErrorOutput.Buffer.Text += string.Format("{1} {0}", Environment.NewLine, line);	
//						});
					}
					catch(Exception)
					{
						break;
					}

					if(line == null)
						break;
					else if(line.StartsWith("X11 error: BadDrawable"))
					{
						Stop();                 
						break;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBoxHelper.ShowError(ex);
			}
		}

		//		private void readOutputProc()
		//		{
		//			try
		//			{
		//				while(_mplayerProcess != null && !_mplayerProcess.HasExited)
		//				{
		//					string line = null;
		//					try
		//					{
		//						line = _mplayerProcess.StandardOutput.ReadLine();
		////						Gtk.Application.Invoke(delegate
		////						{
		////							txtDefaultOutput.Buffer.Text += string.Format("{1} {0}", Environment.NewLine, line);	
		////						});
		//					}
		//					catch(Exception)
		//					{
		//						break;
		//					}
		//
		//					if(line == null)
		//						break;
		//					else if(line.StartsWith("VO:") && line.Contains("]") && line.Contains("x"))
		//					{
		//						// VO: [direct3d] 1280x720 => 1280x720 Planar YV12  
		//						//VIDEO:  [MP4S]  320x240  24bpp  1000.000 fps    0.0 kbps ( 0.0 kbyte/s)
		//						string data = line.Substring(line.IndexOf("]") + 1).Trim();
		//						int x = data.IndexOf("x");
		//						if(x > 0)
		//						{
		//							string width = data.Substring(0, data.IndexOf("x")).Trim();
		//							int w, h;
		//							if(int.TryParse(width, out w))
		//							{
		//								string height = data.Substring(++x, data.IndexOf(" ", x) - x).Trim();
		//								if(int.TryParse(height, out h))
		//								{
		//									this._videoHeight = h;
		//									this._videoWidth = w;
		////									Application.Invoke(new EventHandler(delegate
		////									{                                                          
		////										this.SetSizeRequest(w, h);
		////									}));
		//								}
		//							}
		//						}
		//					}
		//					//TODO: track progress and other stats by increasing to msglevel 5
		//					//V:  17.4 17395/17395  2%  0%  0.0% 0 0
		//					//V: 120.5 120512/120512  1%  0%  0.0% 0 0 0.91x 
		//				}
		//			}
		//			catch(Exception ex)
		//			{
		//				MessageBoxHelper.ShowError(ex);
		//			}
		//		}

		private void OnStopped(object sender, EventArgs args)
		{
			if(Stopped != null)
				Stopped(sender, args);
		}

		private string CreateMPlayerUrl()
		{
			var mplayerParams = SettingsManager.LoadSetting("mplayer_params");
			var streamType = _settings.IsMainStream ? "videoMain" : "videoSub";

			var rtspUrl = string.Format("rtsp://{0}:{1}@{2}:{3}/{4}", _settings.UserName, _settings.Password, _settings.Address.ToString(), _settings.Port, streamType);
			var paramString = string.Format("{0} {1}", mplayerParams, rtspUrl); 

			return paramString;
		}

		public void WriteToStandardInput(params char[] chars)
		{
			if(CanControl)
			{
				_mplayerProcess.StandardInput.Write(chars);
			}
		}

		private bool CanControl {
			get
			{ 
				return (this._mplayerProcess != null &&
				this._mplayerProcess.StandardInput.BaseStream.CanWrite); 
			}
		}

		public void Mute()
		{
			WriteToStandardInput('m');
		}

		public void IncreaseVolume()
		{
			WriteToStandardInput('0');
		}

		public void DecreaseVolume()
		{
			WriteToStandardInput('9');
		}

		public void SkipForward()
		{
			WriteToStandardInput((char)0x1b, (char)0x5b, (char)0x43);
		}

		public void SkipBackward()
		{
			WriteToStandardInput((char)0x1b, (char)0x5b, (char)0x44);
		}

		public void Pause()
		{
			WriteToStandardInput(' ');
		}

		public void IncreaseSpeed()
		{
			WriteToStandardInput(']');
		}

		public void DecreaseSpeed()
		{
			WriteToStandardInput('[');
		}

		public void FullScreen()
		{
			WriteToStandardInput('f');
		}

		//		private void OnSizeAllocated(object o, SizeAllocatedArgs args)
		//		{
		//			if(this._videoHeight > 0 && this._videoWidth > 0)
		//			{
		//				Gdk.Rectangle rect = MPlayerHelper.CalculateSize(args.Allocation, _videoWidth, _videoHeight);
		//				if(this._socket.HeightRequest != rect.Height &&
		//				   this._socket.WidthRequest != rect.Width)
		//				{
		//					this.cntVideo.Move(_socket, rect.X, rect.Y);
		//					this._socket.SetSizeRequest(rect.Width, rect.Height);
		//				}
		//			}
		//		}
		//
		//		private static Gdk.Rectangle CalculateSize(Gdk.Rectangle maxSize, int width, int height)
		//		{
		//			double aspectRatio = (double)width / (double)height;
		//			double windowRatio = (double)maxSize.Width / (double)maxSize.Height;
		//
		//			if(aspectRatio == windowRatio)
		//			{
		//				//alreadyCorrect!
		//				maxSize.Y = maxSize.X = 0;
		//			}
		//			else if(aspectRatio > windowRatio)
		//			{
		//				height = (int)((double)maxSize.Width / aspectRatio);
		//				maxSize.Y = (maxSize.Height - height) / 2;
		//				maxSize.X = 0;
		//				maxSize.Height = height;
		//
		//			}
		//			else
		//			{
		//				width = (int)((double)maxSize.Height * aspectRatio);
		//				maxSize.X = (maxSize.Width - width) / 2;
		//				maxSize.Y = 0;
		//				maxSize.Width = width;
		//			}
		//			return maxSize;
		//		}

		//		#region GTK Sockets
		//
		//		private void CreateSocket()
		//		{
		//			if(this._socket == null)
		//				this._socket = new Socket();
		//
		//			this._socket.Visible = true;
		//			this._socket.Realized += new EventHandler(OnVideoWidgetRealized);
		//			this.cntVideo.Put(_socket, 0, 0);
		//		}
		//
		//		private void DestroySocket()
		//		{
		//			if(this._socket != null)
		//			{
		//				this.cntVideo.Remove(this._socket);
		//			}
		//			this._socket = null;
		//		}
		//
		//		private void OnVideoWidgetRealized(object sender, EventArgs args)
		//		{
		//			this._sockedId = (int)_socket.Id;
		//		}
		//
		//		#endregion
	}
}

