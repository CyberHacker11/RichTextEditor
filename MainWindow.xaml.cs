using System;
using System.Collections.Generic;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RichTextEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int[] size = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            for (int i = 0; i < size.Length; i++) cbx_FontSize.Items.Add(size[i]);
            foreach (var font in Fonts.SystemFontFamilies) cbx_Fonts.Items.Add(font.ToString());
            cbx_FontSize.SelectedIndex = 2;
            cbx_Fonts.SelectedIndex = 0;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_Closed_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_NormalMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        private void btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
            else WindowState = WindowState.Minimized;
        }

        private void FontAndFontSize(object sender, SelectionChangedEventArgs e)
        {
            tbx_Text.FontFamily = new FontFamily(cbx_Fonts.SelectedItem.ToString());
        }

        private void cbx_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbx_Text.FontSize = double.Parse(cbx_FontSize.SelectedItem.ToString());
        }

        private void FuncFontStyle(object sender, RoutedEventArgs e)
        {
            tbx_Text.FontFamily = new FontFamily(cbx_Fonts.SelectedItem.ToString());
            tbx_Text.FontSize = double.Parse(cbx_FontSize.SelectedItem.ToString());
            if (tbtn_Bold.IsChecked == true) tbx_Text.FontWeight = FontWeights.Bold;
            else tbx_Text.FontWeight = default;
            if (tbtn_Italic.IsChecked == true) tbx_Text.FontStyle = FontStyles.Italic;
            else tbx_Text.FontStyle = default;
            if (tbtn_Underline.IsChecked == true) tbx_Text.TextDecorations = TextDecorations.Underline;
            else tbx_Text.TextDecorations = default;
        }

        private void MoveText(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn.Content.ToString() == "L") tbx_Text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            else if (btn.Content.ToString() == "C") tbx_Text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            else if (btn.Content.ToString() == "R") tbx_Text.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
        }

        private void ColorSelect(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            ColorDialog colors = new ColorDialog();
            if (colors.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (btn.Content.ToString())
                {
                    case "FontColor":
                        tbx_Text.Foreground = new SolidColorBrush(Color.FromArgb(colors.Color.A, colors.Color.R, colors.Color.G, colors.Color.B));
                        break;
                    case "Highlight":
                      brd_Text.Background = tbx_Text.Background = new SolidColorBrush(Color.FromArgb(colors.Color.A, colors.Color.R, colors.Color.G, colors.Color.B));
                        break;
                    default:
                        break;
                }
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "TXT Format(*.txt)|*.txt|PDF Format(*.pdf)|*.pdf", ValidateNames = true, })
            {
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (sfd.FileName.EndsWith(".txt")) File.WriteAllText(sfd.FileName, tbx_Text?.Text, Encoding.Unicode);
                    else
                    {
                        Document doc = new Document(PageSize.A4.Rotate());
                        try
                        {
                            PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.OpenOrCreate));
                            doc.Open();
                            doc.Add(new iTextSharp.text.Paragraph(tbx_Text?.Text));
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
            }
        }

        private void mbtn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mbtn_New_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show("Are you sure?", "Message", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if(result == MessageBoxResult.Yes) tbx_Text.Text = "";
        }

        private void mbtn_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TXT File | *txt";
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd.InitialDirectory = System.IO.Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..");
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) tbx_Text.Text = File.ReadAllText(ofd.FileName);
        }
    }
}
