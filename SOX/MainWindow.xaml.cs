using Bridge;
using SOX.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Button = System.Windows.Controls.Button;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Path = System.IO.Path;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using TabControl = System.Windows.Controls.TabControl;
using TextBox = System.Windows.Controls.TextBox;
using System.Threading;
using Controls;
using SOX.Properties;
using System.Windows.Threading;
using System.Diagnostics;
using Sentinel;
using Control = System.Windows.Controls.Control;

namespace SOX
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{











		public MainWindow()
		{
		}
		public MainWindow(string cmd)
		{
			Instance = this;
			base.Topmost = Settings.Instance.TopMost;
			InitializeComponent();
			base.ResizeMode = ResizeMode.NoResize;
			tabEditWindow = new TabEditWindow(EditorTabs);
			scriptsContextMenu = new ScriptsContextMenu(this);
			EditorTabs.Loaded += delegate
			{
				EditorTabs.GetTemplateChild<Button>("AddTabButton").Click += delegate
				{
					MakeTab("", "New Tab");
				};
				tabScroller = EditorTabs.GetTemplateChild<ScrollViewer>("TabScrollViewer");
			};
			OutputTab.Loaded += delegate
			{
				OutputTab.GetTemplateChild<Button>("CloseButton").Visibility = Visibility.Collapsed;
			};
			OutputTab.MouseWheel += ScrollTabs;
			OutputTab.Tag = true;
			MakeTab("", "New Tab");
			base.Closing += delegate
			{
				CloseWindows();
			};
			watcher = new FileSystemWatcher("scripts\\");
			watcher.Created += delegate
			{
				base.Dispatcher.Invoke(ReloadScripts);
			};
			watcher.Renamed += delegate
			{
				base.Dispatcher.Invoke(ReloadScripts);
			};
			watcher.Deleted += delegate
			{
				base.Dispatcher.Invoke(ReloadScripts);
			};
			watcher.EnableRaisingEvents = true;
			ReloadScripts();
			int attachedPid = 0;
			this.Output.TextChanged += delegate (object sender, EventArgs e)
			{
				Output.ScrollToEnd();
			}; 

			IPC instance = IPC.Instance;
			instance.Connected += delegate
			{
				base.Dispatcher.Invoke(delegate
				{
					Output.AppendText("Client has connected.\n");
					Bridge.Sentinel.UnlockFPS(Settings.Instance.UnlockFPS);
				});
			};
			instance.Disconnected += delegate
			{
				base.Dispatcher.Invoke(delegate
				{
					attachedPid = 0;
					Output.AppendText("Client has disconnected.\n");
				});
			};
			instance.Error += delegate
			{
				base.Dispatcher.Invoke(delegate
				{
				});
			};
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500.0);
			dispatcherTimer.Tick += delegate
			{
				if (Body2.Visibility != Visibility.Hidden && Settings.Instance.AutoAttach)
				{
					Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
					if (processesByName.Length == 1 && processesByName[0].Id != attachedPid && !(processesByName[0].MainWindowHandle == IntPtr.Zero))
					{
						attachedPid = processesByName[0].Id;
						Bridge.Injector.Inject();
					}
				}
			};
			dispatcherTimer.Start();
		}


		private void ReloadScripts()
		{
			StackPanel stackPanel = new StackPanel();
			using (IEnumerator<string> enumerator = Directory.EnumerateFiles("scripts\\").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string f = enumerator.Current;
					Button button = new Button
					{
						Content = System.IO.Path.GetFileName(f)
					};
					button.MouseRightButtonUp += delegate (object sender, MouseButtonEventArgs e)
					{
						this.scriptsContextMenu.Left = e.GetPosition(null).X - 12.0 + this.Left;
						this.scriptsContextMenu.Top = e.GetPosition(null).Y - 12.0 + this.Top;
						this.scriptsContextMenu.Owner = this;
						this.scriptsContextMenu.Show(f);
					};
					stackPanel.Children.Add(button);
				}
			}
			this.SVContainer.Child = stackPanel;
		}



		private void ScrollTabs(object sender, MouseWheelEventArgs e)
		{
			tabScroller.ScrollToHorizontalOffset(tabScroller.HorizontalOffset + (double)(e.Delta / 10));
		}

		private void MoveTab(object sender, System.Windows.Input.MouseEventArgs e)
		{
			TabItem tabItem = e.Source as TabItem;
			if (tabItem != null && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed && !(VisualTreeHelper.HitTest(tabItem, Mouse.GetPosition(tabItem)).VisualHit is Button))
			{
				DragDrop.DoDragDrop(tabItem, tabItem, System.Windows.DragDropEffects.Move);
			}
		}

		private void DropTab(object sender, System.Windows.DragEventArgs e)
		{
			TabItem tabItem = e.Source as TabItem;
			if (tabItem != null)
			{
				TabItem tabItem2 = e.Data.GetData(typeof(TabItem)) as TabItem;
				if (tabItem2 != null)
				{
					if (!tabItem.Equals(tabItem2))
					{
						TabControl tabControl = tabItem.Parent as TabControl;
						int insertIndex = tabControl.Items.IndexOf(tabItem2);
						int num = tabControl.Items.IndexOf(tabItem);
						tabControl.Items.Remove(tabItem2);
						tabControl.Items.Insert(num, tabItem2);
						tabControl.Items.Remove(tabItem);
						tabControl.Items.Insert(insertIndex, tabItem);
						tabControl.SelectedIndex = num;
					}
					return;
				}
			}
		}

		private void CloseWindows()
		{
			this.tabEditWindow.Close();
			this.watcher.Dispose();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002604 File Offset: 0x00000804
		private void MoveWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				base.ResizeMode = ResizeMode.NoResize;
				base.DragMove();
				base.ResizeMode = ResizeMode.CanResize;
			}
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Minimize_Click(object sender, RoutedEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		private void Execute_Click(object sender, RoutedEventArgs e)
		{
	//		Bridge.Sentinel.ExecuteScript(NewTab.Text);
		}

		private void Open_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Lua Files (*.lua)|*.lua|Txt Files (*.txt)|*.txt";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
		//		NewTab.Text = File.ReadAllText(ofd.FileName);
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Lua Files (*.lua)|*.lua|Txt Files (*.txt)|*.txt";
				if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
			//		File.WriteAllText(sfd.FileName, NewTab.Text);
				}
			}
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
	//		NewTab.Text = "";
		}



		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Authenticator.LoginViaHWIDAsync();
			// What directory are the files in?...

	///		NewTab.Text = "--Theme made by Hawk#4872,The Tabsystem should be done soon, Will be adding a way to change the color of everything this may take awhile";




	///		SearchPanel.Install(NewTab.TextArea);


			string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".LuaMode.xshd";
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			using (System.IO.Stream s = assembly.GetManifestResourceStream(name))
			{
				using (XmlTextReader reader = new XmlTextReader(s))
				{
					var xshd = HighlightingLoader.LoadXshd(reader);
			///		NewTab.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
				}
			}




			try
			{
				string[] luaf = Directory.GetFiles(Directory.GetParent(Directory.GetCurrentDirectory())?.ToString() + "/Scripts", "*.lua");
				string[] txtf = Directory.GetFiles(Directory.GetParent(Directory.GetCurrentDirectory())?.ToString() + "/Scripts", "*.txt");
				for (int j = 0; j < luaf.Length; j++)
				{
			//		scripts.Items.Add(Path.GetFileName(luaf[j]));
				}
				for (int i = 0; i < txtf.Length; i++)
				{
		//			scripts.Items.Add(Path.GetFileName(txtf[i]));
				}
				reset.Start();
			}
			catch

			{
			}
		}



		private void scripts_SelectedChanged(object sender, EventArgs e)
		{
		//	string fileName = ((FileInfo)scripts.SelectedItem).FullName;
		//	if (File.Exists(fileName))
			{
			///	NewTab.Text = File.ReadAllText(fileName);
			}
		}

		private void Attach_Click(object sender, RoutedEventArgs e)
		{

			InjectionResult val = Injector.Inject();
			if ((int)val.Result == 0)
			{
		//		scripts.Items.Add(">Connected and Injected");
				Bridge.Sentinel.Start();
			}
			if ((int)val.Result == 3)
			{
		//		scripts.Items.Add(">Injection Failed, Make sure you have all files");
			}
			if ((int)val.Result == 4)
			{
		//		scripts.Items.Add(">Injection failed, something interfered with injection.");
			}
			if ((int)val.Result == 2)
			{
		//		scripts.Items.Add(">Injection failed, multiple Roblox processes are running.");
			}
			if ((int)val.Result == 1)
			{
		//		scripts.Items.Add(">Injection failed, Roblox is not opened.");
			}
			if ((int)val.Result == 5)
			{
				//scripts.Items.Add(">Already Injected");
			}
			}


		public static TextEditor MakeEditor()
		{
			TextEditor textEditor = new TextEditor
			{
				ShowLineNumbers = true,
				Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)),
				Foreground = new SolidColorBrush(Color.FromRgb(247, 241, byte.MaxValue)),
				LineNumbersForeground = Brushes.LightGray,
				Margin = new Thickness(0.0, 0.0, 2.0, 2.0),
				FontFamily = new FontFamily("Consolas")
			};
			textEditor.Options.EnableEmailHyperlinks = false;
			textEditor.Options.EnableHyperlinks = false;
			textEditor.Options.AllowScrollBelowDocument = true;
			Line line = new Line
			{
				X1 = 0.0,
				Y1 = 0.0,
				X2 = 0.0,
				Y2 = 1.0,
				Stretch = Stretch.Fill,
				StrokeThickness = 1.0,
				Margin = new Thickness(2.0, 0.0, 6.0, 0.0),
				Tag = new object()
			};
			line.SetBinding(Shape.StrokeProperty, new System.Windows.Data.Binding("LineNumbersForeground")
			{
				Source = textEditor
			});
			textEditor.TextArea.LeftMargins.RemoveAt(1);
			textEditor.TextArea.LeftMargins.Insert(1, line);
			textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension("LuaMode.xshd");
			textEditor.TextArea.TextView.LineTransformers.Add(new FunctionColorizingTransformer());
			textEditor.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());
			return textEditor;
		}



		public TabItem MakeTab(string text = "", string title = "New Tab")
		{
			bool loaded = false;
			TextEditor textEditor = MakeEditor();
			textEditor.Text = text;
			TextBox tbox = new TextBox
			{
				Text = title,
				IsEnabled = false,
				TextWrapping = TextWrapping.NoWrap,
				IsHitTestVisible = false,
				Style = (TryFindResource("InvisibleTextBox") as Style)
			};
			TabItem tab = new TabItem
			{
				Content = textEditor,
				Style = (TryFindResource("Tab") as Style),
				Header = tbox,
				AllowDrop = true
			};
			tab.MouseWheel += ScrollTabs;
			tab.Loaded += delegate
			{
				if (!loaded)
				{
					tab.GetTemplateChild<Button>("CloseButton").Click += delegate
					{
						EditorTabs.Items.Remove(tab);
					};
					tabScroller.ScrollToRightEnd();
					loaded = true;
				}
			};
			tab.MouseDown += delegate (object sender, MouseButtonEventArgs e)
			{
				if (e.OriginalSource is Border)
				{
					if (e.MiddleButton == MouseButtonState.Pressed)
					{
						EditorTabs.Items.Remove(tab);
					}
					else if (e.RightButton == MouseButtonState.Pressed)
					{
						tabEditWindow.Left = e.GetPosition(null).X - 12.0 + base.Left;
						tabEditWindow.Top = e.GetPosition(null).Y - 12.0 + base.Top;
						tabEditWindow.Show(tab);
					}
				}
			};
			tab.MouseMove += this.MoveTab;
			tab.Drop += this.DropTab;
			tbox.GotFocus += delegate
			{
				title = tbox.Text;
				tbox.CaretIndex = tbox.Text.Length - 1;
			};
			tbox.KeyDown += delegate (object s, System.Windows.Input.KeyEventArgs e)
			{
				switch (e.Key)
				{
					default:
						return;
					case Key.Escape:
						tbox.Text = title;
						break;
					case Key.Return:
						break;
				}
				tbox.IsEnabled = false;
			};
			tbox.LostFocus += delegate
			{
				tbox.IsEnabled = false;
			};
			EditorTabs.SelectedIndex = EditorTabs.Items.Add(tab);
			return tab;
		}





		public TextEditor GetCurrent()
		{
			if (EditorTabs.SelectedIndex == 0)
			{
				return current = null;
			}
			return current = (EditorTabs.SelectedContent as TextEditor);
		}


		private void reset_Tick(object sender, EventArgs e)
		{
		//	string[] files = Directory.GetFiles(Directory.GetParent(Directory.GetCurrentDirectory())?.ToString() + "/Scripts", "*.lua");
		//	string[] files2 = Directory.GetFiles(Directory.GetParent(Directory.GetCurrentDirectory())?.ToString() + "/Scripts", "*.txt");
	//		if (scripts.Items.Count != files.Length + files2.Length)
			{
		//		scripts.Items.Clear();
			//	for (int i = 0; i < files.Length; i++)
				{
			//		scripts.Items.Add(System.IO.Path.GetFileName(files[i]));
				}
		//		for (int j = 0; j < files2.Length; j++)
				{
			//		scripts.Items.Add(Path.GetFileName(files2[j]));
				}
			}
		}


		private void Sentinel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				DragMove();
			}
			catch (Exception)
			{

			}
		}


		public void Show(string filePath)
		{
			this.filePath = filePath;
			base.Show();
		}


		private void Exec_Click(object sender, RoutedEventArgs e)
		{
	//		if (scripts.SelectedIndex != -1)
			{
	//			Sentinel.ExecuteScript(File.ReadAllText(this.filePath));
			}
		}

		private void Loads_Click(object sender, RoutedEventArgs e)
		{
			try
			{
		//		dynamic fullPath = scripts.SelectedNode.FullPath;
		//		dynamic val = File.ReadAllText(Settings.Default.ScriptPath + (dynamic)"//" + fullPath);
		//		customTabControl1.GetWorkingTextEditor().Text = val;
			}
			catch (Exception)
			{
			}
		}
		public CustomTabControl customTabControl1;
		private System.Windows.Forms.TreeView ScriptView;
		private string filePath;
		private string ClickedScriptFile;
		public static MainWindow Instance;
		private TextEditor current;
		private ScrollViewer tabScroller;
		private readonly TabEditWindow tabEditWindow;
		private readonly FileSystemWatcher watcher;
		private readonly ScriptsContextMenu scriptsContextMenu;
		private System.Timers.Timer reset;

		private void TabablzControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}//dont delete
}//dont delte





