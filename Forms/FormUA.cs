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
    public partial class FormUA : Form
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

        public FormUA(GenericDataset ds)
        {
            InitializeComponent(ds, 800, 600);
        }

        public FormUA(GenericDataset ds, int w, int h)
        {
            InitializeComponent(ds, w, h);
        }

        private void InitializeComponent(GenericDataset ds, int width, int height)
        {
            this.Dataset = ds;
            this.Width = width;
            int sidebarWidth = (int)(this.Width*0.3);

            this.PointSettings = new System.Windows.Forms.GroupBox();
            this.SelectLabel = new System.Windows.Forms.ComboBox();
            this.PickPointColor = new System.Windows.Forms.Button(); 
            this.DisplayPointColor = new System.Windows.Forms.RichTextBox(); 
            this.DisplayLabel = new System.Windows.Forms.CheckBox(); 
            this.PickLabelColor = new System.Windows.Forms.Button(); 
            this.DisplayLabelColor = new System.Windows.Forms.RichTextBox(); 

            this.DisplayLabel.CheckedChanged += new System.EventHandler(this.onDisplayLabelChange);
            MyPlotDataView view = new MyPlotDataView();
            //this.Height = view.PointSettings(0, 0, sidebarWidth, PointSettings, SelectLabel, PickPointColor, DisplayPointColor, DisplayLabel, PickLabelColor, DisplayLabelColor);

            this.VarXSettings = new System.Windows.Forms.GroupBox();
            this.SelectVar = new System.Windows.Forms.ComboBox();
            this.PickXColor = new System.Windows.Forms.Button();
            this.DisplayXColor = new System.Windows.Forms.RichTextBox();
            this.Interval = new System.Windows.Forms.NumericUpDown();

            this.SelectVar.SelectedIndexChanged += new System.EventHandler(this.onVarXChange);
            this.Interval.ValueChanged += this.onIntervalChange;

            this.Height = view.VariableSettings("", 0, 0, sidebarWidth, VarXSettings, SelectVar, Interval);

            this.ClearChartButton = new System.Windows.Forms.Button();             
            
            this.StatisticsGroupBox = new System.Windows.Forms.GroupBox();
            this.ContingencyTableButton = new System.Windows.Forms.Button();            
            this.PlotDataButton = new System.Windows.Forms.Button();


            this.ClearChartButton.Click += new System.EventHandler(this.onClearClick);
            this.PlotDataButton.Click += new System.EventHandler(this.onPlotClick);

            //this.Height += view.StatisticsControls(0, this.Height, sidebarWidth, StatisticsGroupBox, PlotDataButton, ContingencyTableButton);
            this.PlotDataButton.Text = "Refresh Chart";

            //
            // ClearChartButton Button
            //
            ClearChartButton.Text = "Clear Chart";
            ClearChartButton.Name = "ClearChartButton";

            int ClearChartButtonX = view.marginX;
            int ClearChartButtonY = this.StatisticsGroupBox.Height;

            ClearChartButton.Location = new System.Drawing.Point(ClearChartButtonX, ClearChartButtonY);

            ClearChartButton.Height = view.stdButtonHeight;
            ClearChartButton.Width = this.StatisticsGroupBox.Width - 2*view.marginX;
            StatisticsGroupBox.Height += ClearChartButton.Height + 2*view.marginY;

            this.Height += ClearChartButton.Height + 2*view.marginY;

            StatisticsGroupBox.Controls.Add(ClearChartButton);

            this.DisplayDataSettings = new System.Windows.Forms.GroupBox();
            this.DataTable = new System.Windows.Forms.ListView();

            this.DataTable.View = View.Details;

            MyImportCSV ctr = new MyImportCSV();
            this.Height += ctr.DisplayData(0, this.Height, sidebarWidth, 0, this.DisplayDataSettings, this.DataTable);

            DisplayDataSettings.Text = "Calculated Statistics";

            this.DisplayDataSettings.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Top) | (System.Windows.Forms.AnchorStyles.Bottom) | 
                    (System.Windows.Forms.AnchorStyles.Left)
                ));
            this.DataTable.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Top) | (System.Windows.Forms.AnchorStyles.Bottom) | 
                    (System.Windows.Forms.AnchorStyles.Left)
                ));

            this.ChartGroupBox = new System.Windows.Forms.GroupBox();
            this.Chart = new System.Windows.Forms.PictureBox();

            int temp = view.ChartView(sidebarWidth, 0, width - sidebarWidth, 0, this.ChartGroupBox, this.Chart);
            
            if(temp > this.Height) 
            {
                this.Height = temp;
            }
            else
            {
                this.ChartGroupBox.Height = this.Height - 30;
            }

            this.Controls.Add(this.ChartGroupBox);
            this.Controls.Add(this.DisplayDataSettings);

            //this.Controls.Add(this.StatisticsGroupBox);
            this.Controls.Add(this.VarXSettings);
            //this.Controls.Add(this.PointSettings);

            this.ClientSize = new System.Drawing.Size(this.Width, this.Height);
            
            this.Text = "Riemann/Lebesgue Integration";

            InitializeGraphics();
            InitializeVariables();
            InitializeListView();

            draw();

        }
        //---------------------------------------------------------------------------------------------------//
        private GenericDataset Dataset;
        private BivariateDataset Database;
        private Rectangle Viewport;
        private Rectangle Riemann;
        private Rectangle Lebesgue;
        private List<Rectangle> HistoRect;

        private bool populated = false;

        private Bitmap b;
        private Graphics g;
        private Font SmallFont = new Font("Calibri", 10, FontStyle.Regular, GraphicsUnit.Pixel);
        

        private string[] LVColumns = new string[3]{"Statistics", "X", "Y"};
        private string[] LVStatistics = new string[4]{"Minimum", "Maximum", "Range", "Arithmetic Mean"};

        //---------------------------------------------------------------------------------------------------//

        //
        //
        //
        private void PopulateUnivariateDatabase()
        {
               

            Database = new BivariateDataset();
            var X = new UnivariateDataset<int>();

            string variableName = (SelectVar.SelectedIndex < 0)? SelectVar.Items[0].ToString() :SelectVar.Text;
            int count = Dataset.GetVariableCount(variableName);

            HistoRect = new List<Rectangle>();

            // Min 
            X.Min = 1;
            Database.Min = new Tuple<string,string>(Dataset.GetMinAsString(variableName), X.Min.ToString());
            var item = this.DataTable.Items[0];
            item.SubItems.Clear();
            item.Text = LVStatistics[0];
            item.SubItems.Add(Database.Min.Item2);
            item.SubItems.Add(Database.Min.Item1);

            // Max
            X.Max = count;
            Database.Max = new Tuple<string,string>(Dataset.GetMaxAsString(variableName), X.Max.ToString());
            item = this.DataTable.Items[1];
            item.SubItems.Clear();
            item.Text = LVStatistics[1];
            item.SubItems.Add(Database.Max.Item2);
            item.SubItems.Add(Database.Max.Item1);

            
            // Range
            X.Range = X.Max - X.Min;
            Database.Range = new Tuple<string,string>(Dataset.GetRangeAsString(variableName), X.Range.ToString());
            item = this.DataTable.Items[2];
            item.SubItems.Clear();
            item.Text = LVStatistics[2];
            item.SubItems.Add(Database.Range.Item2);
            item.SubItems.Add(Database.Range.Item1);

            // arithmetic mean 
            X.ArithmeticMean = count/2;
            Tuple<string, string> tuple = new Tuple<string, string>(Dataset.GetAMAsString(variableName), X.ArithmeticMean.ToString());
            Database.AM = tuple;
            item = this.DataTable.Items[3];
            item.SubItems.Clear();
            item.Text = LVStatistics[3];
            item.SubItems.Add(Database.AM.Item2);
            item.SubItems.Add(Database.AM.Item1);

            
            
            var YR = Dataset.getY(variableName, Riemann.Bottom, Riemann.Height);
            
            // Points
            foreach(var i in Enumerable.Range(0, YR.Count))
            {
                var point = new MyPoint();
                point.X = X.X_viewport((i + 1).ToString(), Riemann.Left, Riemann.Width);
                point.Y = YR[i];
                Database.DataPoints.Add(point);
            }
            
            

            int nx =(int) Interval.Value;
            if(nx == 0) nx  = 1;
            int interval_x = (Riemann.Width/nx) +1;

            int ny =(int) Interval.Value;
            if(ny == 0) ny  = 1;
            int interval_y = (Lebesgue.Height/ny) +1;

            int start1 = Riemann.Left;
            int start2 = Lebesgue.Top - Riemann.Top +1;
            Database.CalculateDistributions(start1, interval_x, start2, interval_y);

            if(populated == false) populated = true;
            if(SelectVar.SelectedIndex < 0) SelectVar.SelectedIndex = 0;

        }
        //
        // Draw
        //
        private void draw()
        {
            
            InitializeGraphics();
            PopulateUnivariateDatabase();

            if(populated)
            {
                foreach(var point in Database.DataPoints)
                {
                    //Console.WriteLine("{0}, {1}", point.X, point.Y);
                    g.FillEllipse(Brushes.Blue, new Rectangle(new Point(point.X - 2, point.Y - 2), new Size(4, 4)));
                    g.FillEllipse(Brushes.Blue, new Rectangle(new Point(point.X - 2, Riemann.Bottom + point.Y - 2), new Size(4, 4)));
                }

                // Riemann Integration
                foreach(var kvp in Database.Distribution.Item1)
                {
                    int x = (int)kvp.Key.getMin();
                    int w = (int)(kvp.Key.getMax() - kvp.Key.getMin()) - 1;
                    int y = (int)kvp.Value.Max;
                    int h = (int)(Riemann.Bottom - y);
                    Rectangle rect = new Rectangle(x, y, w, h);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Green)), rect);
                    //Console.WriteLine("{0} - {1}: {2}", x, x+w, h);
                }

                // Lebesgue Integration
                foreach(var kvp in Database.Distribution.Item2)
                {
                    int y = (int) (Riemann.Bottom + kvp.Key.getMin());
                    int h = (int) (kvp.Key.getMax() - kvp.Key.getMin()) - 1;
                    

                    foreach(var kvp2 in Database.Distribution.Item1)
                    {
                        if(y > Riemann.Bottom + kvp2.Value.Min)
                        {
                            int x = (int)kvp2.Key.getMin();
                            int w =(int)(kvp2.Key.getMax() - kvp2.Key.getMin());
                            Rectangle rect = new Rectangle(x, y, w, h);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Green)), rect);
                        }
                    }
                    
                }

            }
            
            
            // if(SelectVar.SelectedIndex >= 0)
            // {
            //     PlotDataButton.Enabled = true;
            //     PopulateUnivariateDatabase();

            //     if(Database.Distribution.Item1 != null)
            //     {
            //         var D1 =  Database.Distribution.Item1;
            //         foreach(var kvp in D1)
            //         {
            //             int width = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
            //             int height =  Riemann.Bottom - (int)kvp.Value.Min;

            //             int X_device = (int)kvp.Key.getMin();
            //             int Y_device = Riemann.Bottom - height;
                        

            //             if(X_device < Riemann.Left) 
            //             {
            //                 width = width - (Riemann.Left - X_device);
            //                 X_device = Riemann.Left;
            //             }
            //             if(X_device + width > Riemann.Right) width = Riemann.Right - X_device;

            //             Rectangle rect = new Rectangle(X_device, Y_device, width, height);
            //             g.FillRectangle(Brushes.Green, rect);

            //             // rugplot
            //             g.DrawLine(Pens.Green, Riemann.Left - 5, Y_device + height, Riemann.Left, Y_device + height);

            //             // mean
            //             g.DrawLine(Pens.Red, (int)kvp.Value.MeanValue, Y_device, (int)kvp.Value.MeanValue, Y_device + height );


            //         }
            //     }
            //     if(Database.Distribution.Item1 != null)
            //     {
            //         var D2 =  Database.Distribution.Item2;
            //         foreach(var kvp in D2)
            //         {
            //             //int width = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
            //             // int height =  (int)(kvp.Value.RelativeFrequency * Riemann.Height);

            //             // int X_device = (int)kvp.Key.getMin();                        
            //             // int Y_device = Lebesgue.Bottom - height;

            //             int height = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
            //             int width =  (int)(kvp.Value.RelativeFrequency * Lebesgue.Width);

            //             int Y_device = Lebesgue.Top - Riemann.Top + (int)kvp.Key.getMin();
            //             int X_device = Lebesgue.Left;
                        

            //             // if(X_device < Riemann.Left) 
            //             // {
            //             //     width = width - (Riemann.Left - X_device);
            //             //     X_device = Riemann.Left;
            //             // }
            //             // if(X_device + width > Riemann.Right) width = Riemann.Right - X_device;

            //             Rectangle rect = new Rectangle(X_device, Y_device, width, height);
            //             g.FillRectangle(Brushes.Green, rect);
            //             HistoRect.Add(rect);
            //             // // rugplot
            //             // g.DrawLine(Pens.Green, Riemann.Left - 5, Y_device + height, Riemann.Left, Y_device + height);

            //             // // mean
            //             // g.DrawLine(Pens.Red, (int)kvp.Value.MeanValue, Y_device, (int)kvp.Value.MeanValue, Y_device + height );


            //         }
            //     }

            //     for(int i = 0; i< Database.DataPoints.Count; i++)
            //     {
            //         var point = Database.DataPoints[i];
            //         int X_device = point.X;
            //         int Y_device = point.Y;
                    
                    
            //         g.FillEllipse(Brushes.Blue, new Rectangle(new Point(X_device - 2, Y_device - 2), new Size(4, 4)));

            //         // g.FillEllipse(Brushes.Blue, new Rectangle(new Point(X_device - 2, Lebesgue.Top - Riemann.Top + Y_device - 2), new Size(4, 4)));

            //     }

            //     int q = 4;

            //     var ordered = HistoRect.OrderBy(rect => rect.Top);
            //     double q_quantile = 1/(double)q;

            //     double sum = 0;
            //     foreach(var rect in ordered)
            //     {
            //         sum += rect.Width/(double)Riemann.Width;

            //         //Console.WriteLine("Current: {0} / {1}", sum, q_quantile);
            //         if(sum >= q_quantile)
            //         {
            //             g.DrawLine(Pens.Black, Riemann.Left, rect.Top, Riemann.Right, rect.Top);
            //             sum = 0;
            //         }
            //     }
            // }

        }

        //
        //
        //
        private void InitializeListView()
        {
            this.DataTable.Clear();
            this.DataTable.BeginUpdate();

            
            foreach(var col in LVColumns)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = col;
                header.TextAlign = HorizontalAlignment.Left;
                header.Width = (this.DataTable.Width) / 3 - 2;

                this.DataTable.Columns.Add(header);
            }

            foreach(var stat in LVStatistics)
            {
                ListViewItem item = new ListViewItem(stat);
                this.DataTable.Items.Add(item);
            }

            this.DataTable.EndUpdate();
        }
    
        private void InitializeVariables()
        {
            var Variables = Dataset.GetNumericVariables();
            foreach(var Var in Variables)
            {
                SelectVar.Items.Add(Var);
            }
        }



        //
        // Init Graphics
        //
        private void InitializeGraphics()
        {            
            Viewport = new Rectangle(0, 0, Chart.Width - 1, Chart.Height - 1);
            
            int W = (int)(Viewport.Width*0.9);
            int marginX = (int)(Viewport.Width*0.05);

            int H = (int)(Viewport.Height*0.45);
            int marginY = (int)(Viewport.Height*0.03);
            
            Viewport = new Rectangle(0, 0, Chart.Width - 1, Chart.Height - 1);
            Riemann = new Rectangle(marginX, marginY, W, H);
            Lebesgue = new Rectangle(marginX, Riemann.Bottom + marginY, W, H);

            b = new Bitmap(Chart.Width, Chart.Height);
            g = Graphics.FromImage(b);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, Viewport);
            g.DrawRectangle(Pens.Blue, Riemann);
            g.DrawRectangle(Pens.Blue, Lebesgue);
            
            Chart.Image = b;
        }

        //---------------------------------------------------------------------------------------------------//
        //
        // CLEAR
        //
        private void onPlotClick(object sender, EventArgs e)
        {
            draw();
        }

        //
        //
        //
        private void onDisplayLabelChange(object sender, EventArgs e)
        {
            draw();
        }

        private void onIntervalChange(object sender, System.EventArgs e)
        {
            draw();
        }
        //
        //
        //
        private void onVarXChange(object sender, System.EventArgs e)
        {
            //Console.WriteLine(SelectVar.Text);
            draw();
        }

        //
        // CLEAR
        //
        private void onClearClick(object sender, EventArgs e)
        {
            InitializeGraphics();
        }
        
        //---------------------------------------------------------------------------------------------------//
        private System.Windows.Forms.GroupBox PointSettings;
        private System.Windows.Forms.ComboBox SelectLabel;
        private System.Windows.Forms.Button PickPointColor;
        private System.Windows.Forms.RichTextBox DisplayPointColor;
        private System.Windows.Forms.CheckBox DisplayLabel;
        private System.Windows.Forms.Button PickLabelColor;
        private System.Windows.Forms.RichTextBox DisplayLabelColor;

        private System.Windows.Forms.GroupBox VarXSettings;
        private System.Windows.Forms.ComboBox SelectVar;
        private System.Windows.Forms.Button PickXColor;
        private System.Windows.Forms.RichTextBox DisplayXColor;
        private System.Windows.Forms.NumericUpDown Interval;


        private System.Windows.Forms.Button ClearChartButton;

        private System.Windows.Forms.GroupBox StatisticsGroupBox;
        private System.Windows.Forms.Button PlotDataButton;
        private System.Windows.Forms.Button ContingencyTableButton;

        private System.Windows.Forms.GroupBox DisplayDataSettings;
        private System.Windows.Forms.ListView DataTable;

        private System.Windows.Forms.GroupBox ChartGroupBox;
        private System.Windows.Forms.PictureBox Chart;
    }
}