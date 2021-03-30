using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyControls;
using Statistics2020Library;

namespace first_app
{
    public partial class FormCT : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public FormCT(BivariateDataset ds)
        {
            InitializeComponent(ds,800, 400);
        }

        public FormCT(BivariateDataset ds, int w, int h)
        {
            InitializeComponent(ds, w, h);
        }

        private void InitializeComponent(BivariateDataset ds, int width, int height)
        {
            this.Dataset = ds;

            this.Width = width;
            int sidebarWidth = (int)(this.Width*0.3);
            MyPlotDataView view = new MyPlotDataView();

            this.DisplayAbsoluteButton = new System.Windows.Forms.Button();             
            
            this.ControlGroupBox = new System.Windows.Forms.GroupBox();
            this.DisplayAbsoluteButton = new System.Windows.Forms.Button();            
            this.DisplayRelativeButton = new System.Windows.Forms.Button();

            this.DisplayAbsoluteButton.Click += new System.EventHandler(this.onDisplayAbsoluteButtonClick);
            this.DisplayRelativeButton.Click += new System.EventHandler(this.onDisplayRelativeButtonClick);

            

            this.DisplayDataSettings = new System.Windows.Forms.GroupBox();
            this.DataTable = new System.Windows.Forms.ListView();

            this.DataTable.View = View.Details;

            MyImportCSV ctr = new MyImportCSV();
            var temp = ctr.DisplayData(0, 0, 600, 200, this.DisplayDataSettings, this.DataTable);
            
            // this.DisplayDataSettings.Anchor = (
            //     (System.Windows.Forms.AnchorStyles)(
            //         (System.Windows.Forms.AnchorStyles.Top) | (System.Windows.Forms.AnchorStyles.Left) 
            //     ));

            MyButton b = new MyButton();
            b.Init( DisplayDataSettings.Right,  temp , 200, ControlGroupBox, DisplayRelativeButton, "Display Options",  "Relative Values");
            b.Add(ControlGroupBox, DisplayAbsoluteButton, "Absolute Values");
            this.ControlGroupBox.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Right) | (System.Windows.Forms.AnchorStyles.Bottom) 
                ));
            


            this.ChartGroupBox1 = new System.Windows.Forms.GroupBox();
            this.Chart1 = new System.Windows.Forms.PictureBox();

            view.ChartView(DisplayDataSettings.Right,  0, 200, 200, this.ChartGroupBox1, this.Chart1);

            this.ChartGroupBox1.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Right) | (System.Windows.Forms.AnchorStyles.Top) | 
                    (System.Windows.Forms.AnchorStyles.Bottom) 
                ));

            this.ChartGroupBox2 = new System.Windows.Forms.GroupBox();
            this.Chart2 = new System.Windows.Forms.PictureBox();

            view.ChartView(15,  DisplayDataSettings.Bottom, 600, 200, this.ChartGroupBox2, this.Chart2);

            this.ChartGroupBox2.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Left) | (System.Windows.Forms.AnchorStyles.Bottom) | 
                    (System.Windows.Forms.AnchorStyles.Right) 
                ));

            this.Height = height; 
            DisplayDataSettings.Text = "Calculated Statistics";

            this.Controls.Add(this.DisplayDataSettings);
            this.Controls.Add(this.ControlGroupBox);
            this.Controls.Add(this.ChartGroupBox1);
            this.Controls.Add(this.ChartGroupBox2);

            this.ClientSize = new System.Drawing.Size(this.Width, this.Height);
            
            this.Text = "Contigency Table";

            InitializeListView();
        }
        //---------------------------------------------------------------------------------------------------//
        private BivariateDataset Dataset = new BivariateDataset();
        private bool abs = true;
        private Rectangle HistoX;
        private Rectangle HistoY;
        private List<Rectangle> HistoXRect;
        private List<Rectangle> HistoYRect;

        private Bitmap b1;
        private Bitmap b2;
        private Graphics g1;
        private Graphics g2;
        private Font SmallFont = new Font("Calibri", 10, FontStyle.Regular, GraphicsUnit.Pixel);
        SolidBrush myBrushG = new SolidBrush(Color.FromArgb(128, 50, 250, 50));
        SolidBrush myBrushB = new SolidBrush(Color.FromArgb(128, 50, 50, 250));
        //---------------------------------------------------------------------------------------------------//
        private void PopulateDistributions()
        {
            foreach(var point in Dataset.DataPoints)
            {
                point.X = (int) (HistoX.Left + HistoX.Width * (double.Parse(point.rawX) - double.Parse(Dataset.Min.Item1)) / double.Parse(Dataset.Range.Item1)); 
                point.Y = (int) (HistoY.Bottom - HistoY.Height * (double.Parse(point.rawY) - double.Parse(Dataset.Min.Item2)) / double.Parse(Dataset.Range.Item2));
                // point.X = X.X_viewport(point.rawX, Scatterplot.Left, Scatterplot.Width);
                // point.Y = Y.Y_viewport(point.rawY, Scatterplot.Bottom, Scatterplot.Height);
                // Dataset.DataPoints.Add(point);
            }
            int nx = Dataset.Distribution.Item1.Count;
            int ny = Dataset.Distribution.Item2.Count;
            Dataset.CalculateDistributions(HistoX.Left, HistoX.Width/nx + 1, HistoY.Top, HistoY.Height/ny +1);
        }
        
        private void draw()
        {
            InitializeGraphics();

            var myBrush = new SolidBrush(Color.FromArgb(128, 50, 250, 50));

            HistoXRect = new List<Rectangle>();
            HistoYRect = new List<Rectangle>();

            PopulateDistributions();
            
            if(Dataset.Distribution.Item1 != null)
            {
                var D1 =  Dataset.Distribution.Item1;
                foreach(var kvp in D1)
                {
                    if(kvp.Value.MeanValue <= HistoX.Right && kvp.Value.MeanValue >= HistoX.Left)
                    {
                        int adj = 80;
                        int X_device = (int)kvp.Key.getMin();
                        int Y_device = HistoX.Bottom - adj;
                        int width = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
                        //int height = (int)(HistoX.Height * (kvp.Value.Count /(double)Dataset.MaxCount.Item1));
                        int height = (int)((HistoX.Height - adj) * kvp.Value.RelativeFrequency);

                        if(X_device < HistoX.Left) 
                        {
                            width = width - (HistoX.Left - X_device);
                            X_device = HistoX.Left;
                        }
                        if(X_device + width > HistoX.Right) width = HistoX.Right - X_device;
                        Rectangle rect = new Rectangle(X_device, Y_device - height, width, height);
                        
                        g2.FillRectangle(myBrush, rect);

                        // rugplot
                        g2.DrawLine(Pens.Green, HistoX.Left - 5, Y_device + height, HistoX.Left, Y_device + height);

                        // mean
                        g2.DrawLine(Pens.Red, (int)kvp.Value.MeanValue, Y_device, (int)kvp.Value.MeanValue, Y_device - height );

                        HistoXRect.Add(rect);
                    }
                }
            }

            if(Dataset.Distribution.Item2 != null)
            {
                var D2 =  Dataset.Distribution.Item2;
                foreach(var kvp in D2)
                {
                    if(kvp.Value.MeanValue <= HistoY.Bottom && kvp.Value.MeanValue >= HistoY.Top)
                    {
                        int height = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
                        //int width = (int)(HistoY.Width * (kvp.Value.Count /(double)Dataset.MaxCount.Item2));
                        int width = (int)(HistoY.Width * kvp.Value.RelativeFrequency);
                        int Y_device = (int)(kvp.Key.getMax() - height);
                        int X_device = HistoY.Left;
                        // if(Y_device < HistoY.Top)
                        // { 
                        //     height = height - (HistoY.Top - Y_device);
                        //     Y_device = HistoY.Top;
                        // }
                        // if(Y_device + height > HistoY.Bottom) height = HistoY.Bottom - Y_device;
                        Rectangle rect = new Rectangle(X_device, Y_device, width, height);
                        g1.FillRectangle(myBrush, rect);

                        // rugplot
                        g1.DrawLine(Pens.Green, X_device, HistoY.Bottom + 5, X_device, HistoY.Bottom);

                        // // mean
                        g1.DrawLine(Pens.Red, X_device, (int) kvp.Value.MeanValue, X_device + rect.Width, (int) kvp.Value.MeanValue );
                        
                        HistoYRect.Add(rect);
                    }
                }
            }

            var orderedX = HistoXRect.OrderBy(rect => rect.Left).ToList();


            int _i = 0;
            foreach(var rect in orderedX)
            {
                //Console.WriteLine(orderedX.GetType());
                // CDF
                if(_i > 0) 
                {
                    int h = 0;
                    foreach (var r in orderedX)
                    {
                        if(r == orderedX[_i]) break;
                        h += r.Height;
                    }

                    var new_rect = new Rectangle(rect.Left, rect.Top - h, rect.Width, h);
                    g2.FillRectangle(myBrushB, new_rect);
                }
                _i++;
            }
            
            var orderedY = HistoYRect.OrderBy(rect => -rect.Top).ToList();

            _i = 0;
            foreach(var rect in orderedY)
            {
                // CDF
                if(_i > 0) 
                {
                    int w = 0;
                    foreach (var r in orderedY)
                    {
                        if(r == orderedY[_i]) break;
                        w += r.Width;
                    }

                    var new_rect = new Rectangle(HistoY.Left + orderedY[_i].Width, rect.Top, w, rect.Height);
                    g1.FillRectangle(myBrushB, new_rect);
                }
                _i++;
            }
            // Chart.Image = b;
            // Chart.Image = b;
        }
        
        //---------------------------------------------------------------------------------------------------//
        //
        // Init Graphics
        //
        private void InitializeGraphics()
        {            
            HistoX = new Rectangle(0, 0, Chart2.Width - 1, Chart2.Height - 1);
            HistoY = new Rectangle(0, 0, Chart1.Width - 1, Chart1.Height - 1);

            b1 = new Bitmap(Chart1.Width, Chart1.Height);
            g1 = Graphics.FromImage(b1);
            g1.SmoothingMode = SmoothingMode.AntiAlias;

            g1.Clear(Color.White);
            g1.DrawRectangle(Pens.Black, HistoY);
            
            Chart1.Image = b1;

            b2 = new Bitmap(Chart2.Width, Chart2.Height);
            g2 = Graphics.FromImage(b2);
            g2.SmoothingMode = SmoothingMode.AntiAlias;

            g2.Clear(Color.White);
            g2.DrawRectangle(Pens.Black, HistoX);
            
            Chart2.Image = b2;
        }
        
        //
        //
        //
        private void InitializeListView()
        {
            var orderedX = Dataset.Distribution.Item1.OrderBy(kvp => kvp.Key.getMin());
            var orderedY = Dataset.Distribution.Item2.OrderBy(kvp => kvp.Key.getMin());
            
            this.DataTable.Clear();
            this.DataTable.BeginUpdate();

            int Count = Dataset.Distribution.Item1.Count;

            ColumnHeader header = new ColumnHeader();
            header.Text = "Y \\ X";
            header.TextAlign = HorizontalAlignment.Left;
            header.Width = ((this.DataTable.Width) / (Count + 2)) - 2;

            this.DataTable.Columns.Add(header);

            foreach(var kvp in orderedX)
            {
                header = new ColumnHeader();
                //header.Text = kvp.Key.getMin().ToString() + " - " + kvp.Key.getMax().ToString();
                header.Text = kvp.Key.rawStart;
                header.TextAlign = HorizontalAlignment.Left;
                header.Width = ((this.DataTable.Width) / (Count + 2)) - 2;

                this.DataTable.Columns.Add(header);
            }

            header = new ColumnHeader();
            header.Text = "Dist Y";
            header.TextAlign = HorizontalAlignment.Left;
            header.Width = ((this.DataTable.Width) / (Count + 2)) - 2;

            this.DataTable.Columns.Add(header);

            foreach(var kvpY in orderedY)
            {

                // prima colonna
                string text = kvpY.Key.rawStart;
                ListViewItem item = new ListViewItem(text);
                this.DataTable.Items.Add(item);

                double sum = 0;
                foreach (var kvpX in orderedX)
                {
                    if(kvpX.Value.D.ContainsKey(kvpY.Key))
                    {
                        double val;
                        if(abs) val = kvpX.Value.D[kvpY.Key].Count;
                        else val = kvpX.Value.D[kvpY.Key].RelativeFrequency;
                        sum += val;
                        var text_val = val.ToString();
                        item.SubItems.Add(text_val); // item.SubItems.Add(kvpX.Value.D[kvpY.Key].Count.ToString());

                    }
                    else 
                        item.SubItems.Add("-");
                }

                // ultima colonna
                item.SubItems.Add(sum.ToString());
            }

            string text_last = "Dist X";
            ListViewItem item_last = new ListViewItem(text_last);
            this.DataTable.Items.Add(item_last);
            foreach (var kvpX in orderedX)
            {
                double sum = 0;
                foreach(var kvp in kvpX.Value.D)
                {
                    if(abs) sum += kvp.Value.Count;
                    else sum += kvp.Value.RelativeFrequency;
                }
                item_last.SubItems.Add(sum.ToString());
            }

            this.DataTable.EndUpdate();


            draw();

        }


        //
        // Display Absolute
        //
        private void onDisplayAbsoluteButtonClick(object sender, EventArgs e)
        {
            abs = true;
            InitializeListView();
            DisplayAbsoluteButton.Enabled = false;
            DisplayRelativeButton.Enabled = true;
        }

        //
        // Display Absolute
        //
        private void onDisplayRelativeButtonClick(object sender, EventArgs e)
        {
            abs = false;
            InitializeListView();

            DisplayAbsoluteButton.Enabled = true;
            DisplayRelativeButton.Enabled = false;
        }


        private System.Windows.Forms.GroupBox ControlGroupBox;
        private System.Windows.Forms.Button DisplayRelativeButton;
        private System.Windows.Forms.Button DisplayAbsoluteButton;

        private System.Windows.Forms.GroupBox DisplayDataSettings;
        private System.Windows.Forms.ListView DataTable;

        private System.Windows.Forms.GroupBox ChartGroupBox1;
        private System.Windows.Forms.PictureBox Chart1;

        private System.Windows.Forms.GroupBox ChartGroupBox2;
        private System.Windows.Forms.PictureBox Chart2;
    }
}