using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SDSManager
{
	internal class Window
	{
		private string _title;
		public string Title
		{
			get => _title;
			set
			{
				if (value == null || value.Length <= 1)
				{
					throw new ArgumentNullException("value");
				}
				_title = value;
			}
		}

		private int _width;
		public int Width
		{
			get => _width;
			set
			{
				if (value <= 1) throw new ArgumentException("value");
				_width = value;
			}
		}

		private int _height;

		public int Height
		{
			get => _height;
			set
			{
				if (value <= 1) throw new ArgumentException("value");
				_height = value;
			}
		}

		/// <summary>
		/// indentation on the left taking into account the border
		/// </summary>
		private int _leftPadding;
		public int LeftPadding
		{
			get => _leftPadding;
			set
			{
				if (value < 1) throw new ArgumentException("value");
				_leftPadding = value;
			}
		}

		/// <summary>
		/// indentation on the right taking into account the border
		/// </summary>
		private int _topPadding;
		public int TopPadding
		{
			get => _topPadding;
			set
			{
				if (value < 1) throw new ArgumentException("value");
				_topPadding = value;
			}
		}

		public List<DirectoryInfo> PrevDirectories { get; set; }
		public DirectoryInfo? CurrentDirectory { get; set; }
		public FileInfo? SelectedFile { get; set; }
		public DirectoryInfo? SelectedFolder { get; set; }
		public object[] ContentObjects { get; set; }

		private int _selectedObjectIndex = 0;
		public int SelectedObjectIndex
		{
			get
			{
				if (_selectedObjectIndex < 0)
				{
					_selectedObjectIndex = ContentObjects.Length - 1;
					return _selectedObjectIndex;
				}else if (_selectedObjectIndex >= ContentObjects.Length)
				{
					_selectedObjectIndex = 0;
					return _selectedObjectIndex;
				}
				return _selectedObjectIndex;
			}
			set => _selectedObjectIndex = value;
		}

		/// <summary>
		/// The type of content displayed in the window
		/// </summary>
		public WindowContentType ContentType { get; set; }

		/// <summary>
		/// The content displayed in the window, in string[] format
		/// </summary>
		public string[] Content { get; set; }


		public Window(int width, int height, int leftPadding, int topPadding, WindowContentType type)
		{
			Width = width;
			Height = height;
			LeftPadding = leftPadding;
			TopPadding = topPadding;

			Content = new string[Width * Height];

			ContentType = type;

			CurrentDirectory = new DirectoryInfo(Environment.GetLogicalDrives()[0]); // TEMP

			PrevDirectories = new List<DirectoryInfo>();

			var drives = Environment.GetLogicalDrives();

			ContentObjects = new Object[drives.Length];
			for (int i = 0; i < ContentObjects.Length; i++)
			{
				ContentObjects[i] = new DirectoryInfo(drives[i]);
			}
		}
	}
}
