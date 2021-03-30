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
    public partial class PlotData : Form
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

        public PlotData(GenericDataset ds)
        {
            InitializeComponent(ds, 800, 600);
        }

        public PlotData(GenericDataset ds, int w, int h)
        {
            InitializeComponent(ds, w, h);
        }

        private void InitializeComponent(GenericDataset myds, int width, int height)
        {
            this.Dataset = myds;
            var DS = new UnivariateDataset<double>() as IUnivariateDataset;
            var col = new AlphanumericDataset();
            col.Name = "Default X";
            col.ListOfObservations = new List<string>();

            //Console.WriteLine(col.DataType);
            foreach(var i in Enumerable.Range(1, Dataset.GetVariableCount(Dataset.Labels[0])))
            {
                col.ListOfObservations.Add(i.ToString());
            }

            //DS.isNumeric = true;
            DS.Init(col);
            
            if(!Dataset.UnivariateDatasets.ContainsKey(DS.Name)) Dataset.UnivariateDatasets.Add(DS.Name, DS);
            else 
            {
                DS.Name  = DS.Name + "1";
                Dataset.UnivariateDatasets.Add(DS.Name, DS);
            }
            Dataset.Labels.Add(DS.Name);
            Dataset.Log();
            Console.WriteLine("Added {0}", DS.Name);
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
            this.SelectVarX = new System.Windows.Forms.ComboBox();
            this.PickXColor = new System.Windows.Forms.Button();
            this.DisplayXColor = new System.Windows.Forms.RichTextBox();
            this.IntervalX = new System.Windows.Forms.NumericUpDown();

            this.SelectVarX.SelectedIndexChanged += new System.EventHandler(this.onVarXChange);
            this.IntervalX.Value = 10;
            this.Height = view.VariableSettings("X", 0, 0, sidebarWidth, VarXSettings, SelectVarX, IntervalX);

            this.VarYSettings = new System.Windows.Forms.GroupBox();
            this.SelectVarY = new System.Windows.Forms.ComboBox();
            this.PickYColor = new System.Windows.Forms.Button();
            this.DisplayYColor = new System.Windows.Forms.RichTextBox();
            this.IntervalY = new System.Windows.Forms.NumericUpDown();

            this.SelectVarY.SelectedIndexChanged += new System.EventHandler(this.onVarYChange);
            this.IntervalY.Value = 10;
            this.Height += view.VariableSettings("Y", 0, this.Height, sidebarWidth, VarYSettings, SelectVarY, IntervalY);

            this.ClearChartButton = new System.Windows.Forms.Button();             
            
            this.StatisticsGroupBox = new System.Windows.Forms.GroupBox();
            this.ContingencyTableButton = new System.Windows.Forms.Button();            
            this.PlotDataButton = new System.Windows.Forms.Button();


            this.ClearChartButton.Click += new System.EventHandler(this.onClearClick);
            this.PlotDataButton.Click += new System.EventHandler(this.onPlotClick);
            this.ContingencyTableButton.Click += new System.EventHandler(this.onCTClick);

            this.Height += view.StatisticsControls(0, this.Height, sidebarWidth, StatisticsGroupBox, PlotDataButton, ContingencyTableButton);
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

            this.Controls.Add(this.StatisticsGroupBox);
            this.Controls.Add(this.VarYSettings);
            this.Controls.Add(this.VarXSettings);
            //this.Controls.Add(this.PointSettings);

            this.ClientSize = new System.Drawing.Size(this.Width, this.Height);
            
            this.Text = "Plot Data";

            InitializeGraphics();

            InitializeLabels();

            InitializeListView();

            InitializeVariables();

        }
        //---------------------------------------------------------------------------------------------------//
        private GenericDataset Dataset;
        private BivariateDataset Database;
        private Rectangle Viewport;
        private Rectangle Scatterplot;
        private Rectangle HistoX;
        private Rectangle HistoY;
        private Rectangle Legend;
        // private Median MedianX = new Median();
        // private Median MedianY = new Median();
        private List<string> ordX;
        private List<string> ordY;

        private List<Rectangle> HistoXRect;
        private List<Rectangle> HistoYRect;
        // private int[] quantiles;
        private Bitmap b;
        private Graphics g;
        private Font SmallFont = new Font("Calibri", 10, FontStyle.Regular, GraphicsUnit.Pixel);
        Pen myPen = new Pen(Color.FromArgb(100, Color.Black), 3);
        SolidBrush myBrushG = new SolidBrush(Color.FromArgb(128, 50, 250, 50));
        SolidBrush myBrushB = new SolidBrush(Color.FromArgb(128, 50, 50, 250));

        private string[] LVColumns = new string[3]{"Statistics", "X", "Y"};
        private string[] LVStatistics = new string[9]{
            "Minimum", 
            "Maximum", 
            "Range", 
            "Arithmetic Mean", 
            "Median", 
            "Variance", 
            "Covariance", 
            "Coeff. of Corr.", 
            "Coeff. of Det."
        };

        //---------------------------------------------------------------------------------------------------//

        //
        //
        //
        private void PopulateDatabase()
        {            
            Database = new BivariateDataset();
            int count = Dataset.GetVariableCount(SelectVarX.Text);
            HistoXRect = new List<Rectangle>();
            HistoYRect = new List<Rectangle>();

            // Min 
            Database.Min = new Tuple<string,string>(Dataset.GetMinAsString(SelectVarX.Text), Dataset.GetMinAsString(SelectVarY.Text));
            var item = this.DataTable.Items[0];
            item.SubItems.Clear();
            item.Text = LVStatistics[0];
            item.SubItems.Add(Database.Min.Item1);
            item.SubItems.Add(Database.Min.Item2);

            // Max
            Database.Max = new Tuple<string,string>(Dataset.GetMaxAsString(SelectVarX.Text), Dataset.GetMaxAsString(SelectVarY.Text));
            item = this.DataTable.Items[1];
            item.SubItems.Clear();
            item.Text = LVStatistics[1];
            item.SubItems.Add(Database.Max.Item1);
            item.SubItems.Add(Database.Max.Item2);
            
            // Range
            Database.Range = new Tuple<string,string>(Dataset.GetRangeAsString(SelectVarX.Text), Dataset.GetRangeAsString(SelectVarY.Text));
            item = this.DataTable.Items[2];
            item.SubItems.Clear();
            item.Text = LVStatistics[2];
            item.SubItems.Add(Database.Range.Item1);
            item.SubItems.Add(Database.Range.Item2);

            // arithmetic mean 
            Database.AM = new Tuple<string,string>(Dataset.GetAMAsString(SelectVarX.Text), Dataset.GetAMAsString(SelectVarY.Text));
            item = this.DataTable.Items[3];
            item.SubItems.Clear();
            item.Text = LVStatistics[3];
            item.SubItems.Add(Database.AM.Item1);
            item.SubItems.Add(Database.AM.Item2);

            // var tempX = Dataset.UnivariateDatasets[SelectVarX.Text];

            // foreach(var x in X.ANO)
            // {
            //     Values.Add(X.X_viewport(x, v_left, v_width));
            // }
            // // var X = Dataset.getX(SelectVarX.Text, Scatterplot.Left, Scatterplot.Width);
            // var X = Dataset.getStringValues(SelectVarX.Text);
            // // var Y = Dataset.getY(SelectVarY.Text, Scatterplot.Bottom, Scatterplot.Height);
            // var Y = Dataset.getStringValues(SelectVarY.Text);
            
            var X = Dataset.UnivariateDatasets[SelectVarX.Text];
            var Y = Dataset.UnivariateDatasets[SelectVarY.Text];
            var Label = fetchLabels();
            (double X, double Y) var_comp = (0, 0);
            double covar_comp = 0;

            // MedianX.Init();
            // MedianY.Init();
            ordX = new List<string>();
            ordY = new List<string>();
            // Points
            foreach(var i in Enumerable.Range(0, count))
            {
                var point = new MyPoint();
                point.rawX = X.ANO[i];
                point.rawY = Y.ANO[i];
                
                ordX.Add(point.rawX);
                ordY.Add(point.rawY);

                point.X = X.X_viewport(point.rawX, Scatterplot.Left, Scatterplot.Width);
                point.Y = Y.Y_viewport(point.rawY, Scatterplot.Bottom, Scatterplot.Height);
                point.Label = Label[i];
                Database.DataPoints.Add(point);

                var_comp.X += Math.Pow(double.Parse(point.rawX), 2) / (double) count;
                var_comp.Y += Math.Pow(double.Parse(point.rawY), 2) / (double) count;

                covar_comp += double.Parse(point.rawX) * double.Parse(point.rawY) / (double) count;
                // Console.WriteLine("{0}, {1}", point.X, point.Y);
            }

            ordX = ordX.OrderBy(x => double.Parse(x)).ToList();
            ordY = ordY.OrderBy(y => double.Parse(y)).ToList();

            Database.Median = new Tuple<string,string>(ordX[ordX.Count/2], ordY[ordY.Count/2]);
            item = this.DataTable.Items[4];
            item.SubItems.Clear();
            item.Text = LVStatistics[4];
            item.SubItems.Add(Database.Median.Item1);
            item.SubItems.Add(Database.Median.Item2);

            // var(x), var(y) 
            Database.Var = new Tuple<string, string>(
                (var_comp.X - Math.Pow(double.Parse(Database.AM.Item1), 2)).ToString(), 
                (var_comp.Y - Math.Pow(double.Parse(Database.AM.Item2), 2)).ToString()
            );

            item = this.DataTable.Items[5];
            item.SubItems.Clear();
            item.Text = LVStatistics[5];
            item.SubItems.Add(Database.Var.Item1);
            item.SubItems.Add(Database.Var.Item2);


            // covar 
            Database.Cov = new Tuple<string, string>(
                (covar_comp - double.Parse(Database.AM.Item1) * double.Parse(Database.AM.Item2)).ToString(), 
                (covar_comp - double.Parse(Database.AM.Item1) * double.Parse(Database.AM.Item2)).ToString()
            );
            

            item = this.DataTable.Items[6];
            item.SubItems.Clear();
            item.Text = LVStatistics[6];
            item.SubItems.Add(Database.Cov.Item1);
            item.SubItems.Add(Database.Cov.Item2);


            // -1 <= r <= 1
            var sigma_x = Math.Pow(double.Parse(Database.Var.Item1), 0.5);
            var sigma_y = Math.Pow(double.Parse(Database.Var.Item2), 0.5);
            var sigma_xy = double.Parse(Database.Cov.Item1);

            var r = ( sigma_xy / (sigma_x * sigma_y)).ToString();
            item = this.DataTable.Items[7];
            item.SubItems.Clear();
            item.Text = LVStatistics[7];
            item.SubItems.Add(r);
            item.SubItems.Add(r);

            // 0 <= R^2 <= 1
            var R_2 = Math.Pow(double.Parse(r), 2).ToString();
            item = this.DataTable.Items[8];
            item.SubItems.Clear();
            item.Text = LVStatistics[8];
            item.SubItems.Add(R_2);
            item.SubItems.Add(R_2);

            // Regression Line X 
            Database.RegrX = (
                (sigma_xy * (double.Parse(Database.Min.Item2) - double.Parse(Database.AM.Item2)) / Math.Pow(sigma_y, 2) + double.Parse(Database.AM.Item1)).ToString(),
                Database.Min.Item2, 
                (sigma_xy * (double.Parse(Database.Max.Item2) - double.Parse(Database.AM.Item2)) / Math.Pow(sigma_y, 2) + double.Parse(Database.AM.Item1)).ToString(),
                Database.Max.Item2
            );

            if((double.Parse(Database.RegrX.x0) < double.Parse(Database.Min.Item1))) {
                Database.RegrX.x0 = Database.Min.Item1;         
                Database.RegrX.y0 = ( 
                    double.Parse(Database.AM.Item2) + (double.Parse(Database.Min.Item1) - double.Parse(Database.AM.Item1)) * Math.Pow(sigma_y, 2) / sigma_xy
                ).ToString();
            }
            else if((double.Parse(Database.RegrX.x0) > double.Parse(Database.Max.Item1))) {
                Database.RegrX.x0 = Database.Max.Item1;         
                Database.RegrX.y0 = (
                    double.Parse(Database.AM.Item2) + (double.Parse(Database.Max.Item1) - double.Parse(Database.AM.Item1)) * Math.Pow(sigma_y, 2) / sigma_xy
                ).ToString();
            }

            if((double.Parse(Database.RegrX.x1) < double.Parse(Database.Min.Item1))) {
                Database.RegrX.x1 = Database.Min.Item1; 
                Database.RegrX.y1 = ( 
                    double.Parse(Database.AM.Item2) + (double.Parse(Database.Min.Item1) - double.Parse(Database.AM.Item1)) * Math.Pow(sigma_y, 2) / sigma_xy
                ).ToString();
            }
            else if((double.Parse(Database.RegrX.x1) > double.Parse(Database.Max.Item1))) {
                Database.RegrX.x1 = Database.Max.Item1; 
                Database.RegrX.y1 = (
                    double.Parse(Database.AM.Item2) + (double.Parse(Database.Max.Item1) - double.Parse(Database.AM.Item1)) * Math.Pow(sigma_y, 2) / sigma_xy
                ).ToString();
            }
            
            // Regression Line Y 
            Database.RegrY = (
                Database.Min.Item1,
                (sigma_xy * (double.Parse(Database.Min.Item1) - double.Parse(Database.AM.Item1)) / Math.Pow(sigma_x, 2) + double.Parse(Database.AM.Item2)).ToString(), 
                Database.Max.Item1, 
                (sigma_xy * (double.Parse(Database.Max.Item1) - double.Parse(Database.AM.Item1)) / Math.Pow(sigma_x, 2) + double.Parse(Database.AM.Item2)).ToString()
            );

            if((double.Parse(Database.RegrY.y0) < double.Parse(Database.Min.Item2))) {
                Database.RegrY.y0 = Database.Min.Item2;         
                Database.RegrY.x0 = ( 
                    double.Parse(Database.AM.Item1) + (double.Parse(Database.Min.Item2) - double.Parse(Database.AM.Item2)) * Math.Pow(sigma_x, 2) / sigma_xy
                ).ToString();
            }
            else if((double.Parse(Database.RegrY.y0) > double.Parse(Database.Max.Item2))) {
                Database.RegrY.y0 = Database.Max.Item2;         
                Database.RegrY.x0 = (
                    double.Parse(Database.AM.Item1) + (double.Parse(Database.Max.Item2) - double.Parse(Database.AM.Item2)) * Math.Pow(sigma_x, 2) / sigma_xy
                ).ToString();
            }

            if((double.Parse(Database.RegrY.y1) < double.Parse(Database.Min.Item2))) {
                Database.RegrY.y1 = Database.Min.Item2;         
                Database.RegrY.x1 = ( 
                    double.Parse(Database.AM.Item1) + (double.Parse(Database.Min.Item2) - double.Parse(Database.AM.Item2)) * Math.Pow(sigma_x, 2) / sigma_xy
                ).ToString();
            }
            else if((double.Parse(Database.RegrY.y1) > double.Parse(Database.Max.Item2))) {
                Database.RegrY.y1 = Database.Max.Item2;         
                Database.RegrY.x1 = (
                    double.Parse(Database.AM.Item1) + (double.Parse(Database.Max.Item2) - double.Parse(Database.AM.Item2)) * Math.Pow(sigma_x, 2) / sigma_xy
                ).ToString();
            }

            int nx =(int) IntervalX.Value;
            if(nx == 0) nx  = 1;
            int interval_x = (Scatterplot.Width/nx);

            int ny =(int) IntervalY.Value;
            if(ny == 0) ny  = 1;
            int interval_y = (Scatterplot.Height/ny);

            int start1 = Scatterplot.Left;
            int start2 = Scatterplot.Bottom;
            Database.CalculateDistributions(start1, interval_x + 1, start2, interval_y + 1);

        }
        private List<string> fetchLabels()
        {
            if(SelectLabel.SelectedIndex >= 0)
                return Dataset.getStringValues(SelectLabel.Text);
            else 
                return Enumerable.Range(1, Dataset.GetVariableCount(SelectVarX.Text)).Select(x => x.ToString()).ToList();
        }
        
        //
        // Draw
        //
        private void draw()
        {
            InitializeGraphics();

            if(SelectVarX.SelectedIndex  >= 0 && SelectVarY.SelectedIndex >= 0)
            {
                PlotDataButton.Enabled = true;
                ContingencyTableButton.Enabled = true;


                // Scatterplot  + rugplot Stuff
                PopulateDatabase();

                // Console.WriteLine("label# {0}, X# {1}, Y# {2}", Label.Count, X.Count, Y.Count);

                foreach(var point in Database.DataPoints)
                {
                    int X_device = point.X;
                    int Y_device = point.Y;
                    
                    g.FillEllipse(Brushes.Blue, new Rectangle(new Point(X_device - 2, Y_device - 2), new Size(4, 4)));
                    if(DisplayLabel.Checked)
                    {
                        string label = point.Label;
                        g.DrawString(label, SmallFont, Brushes.Red, new Point(X_device, Y_device));
                    }
                    
                    //rugplot
                    g.DrawLine(Pens.Blue, X_device, Scatterplot.Bottom + 5, X_device, Scatterplot.Bottom);
                    g.DrawLine(Pens.Blue,  Scatterplot.Left - 5,  Y_device, Scatterplot.Left, Y_device);

                }

                // mean point = (Mean(Y), mean(X))
                int AM_X = Dataset.GetAM_ScaledX(SelectVarX.Text, Scatterplot.Left, Scatterplot.Width);
                int AM_Y = Dataset.GetAM_ScaledY(SelectVarY.Text, Scatterplot.Bottom, Scatterplot.Height);

                

                (int x, int y) P0 = (
                    (int) (Scatterplot.Left + Scatterplot.Width * (double.Parse(Database.RegrX.x0) - double.Parse(Database.Min.Item1)) / double.Parse(Database.Range.Item1)), 
                    (int) (Scatterplot.Bottom - Scatterplot.Height * (double.Parse(Database.RegrX.y0) - double.Parse(Database.Min.Item2)) / double.Parse(Database.Range.Item2)) 
                );

                (int x, int y) P1 = (
                    (int) (Scatterplot.Left + Scatterplot.Width * (double.Parse(Database.RegrX.x1) - double.Parse(Database.Min.Item1)) / double.Parse(Database.Range.Item1)), 
                    (int) (Scatterplot.Bottom - Scatterplot.Height * (double.Parse(Database.RegrX.y1) - double.Parse(Database.Min.Item2)) / double.Parse(Database.Range.Item2)) 
                );


                g.DrawLine(myPen, P0.x, P0.y, P1.x, P1.y);

                P0 = (
                    (int) (Scatterplot.Left + Scatterplot.Width * (double.Parse(Database.RegrY.x0) - double.Parse(Database.Min.Item1)) / double.Parse(Database.Range.Item1)), 
                    (int) (Scatterplot.Bottom - Scatterplot.Height * (double.Parse(Database.RegrY.y0) - double.Parse(Database.Min.Item2)) / double.Parse(Database.Range.Item2)) 
                );

                P1 = (
                    (int) (Scatterplot.Left + Scatterplot.Width * (double.Parse(Database.RegrY.x1) - double.Parse(Database.Min.Item1)) / double.Parse(Database.Range.Item1)), 
                    (int) (Scatterplot.Bottom - Scatterplot.Height * (double.Parse(Database.RegrY.y1) - double.Parse(Database.Min.Item2)) / double.Parse(Database.Range.Item2))
                );
                
                g.DrawLine(myPen, P0.x, P0.y, P1.x, P1.y);

                g.FillEllipse(Brushes.Red, new Rectangle(new Point(AM_X - 3, AM_Y - 3), new Size(6, 6)));

                //histograms
                if(Database.Distribution.Item1 != null)
                {
                    var D1 =  Database.Distribution.Item1;
                    foreach(var kvp in D1)
                    {
                        if(kvp.Value.MeanValue <= HistoX.Right && kvp.Value.MeanValue >= HistoX.Left)
                        {
                            int X_device = (int)kvp.Key.getMin();
                            int Y_device = HistoX.Top;
                            int width = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
                            //int height = (int)(HistoX.Height * (kvp.Value.Count /(double)Database.MaxCount.Item1));
                            int height = (int)(HistoX.Height * kvp.Value.RelativeFrequency);
                            if(X_device < HistoX.Left) 
                            {
                                width = width - (HistoX.Left - X_device);
                                X_device = HistoX.Left;
                            }
                            if(X_device + width > HistoX.Right) width = HistoX.Right - X_device;
                            Rectangle rect = new Rectangle(X_device, Y_device, width, height);
                            
                            HistoXRect.Add(rect);
                            
                            
                            g.FillRectangle(myBrushG, rect);

                            // rugplot
                            g.DrawLine(Pens.Green, HistoX.Left - 5, Y_device + height, HistoX.Left, Y_device + height);

                            // mean
                            g.DrawLine(Pens.Red, (int)kvp.Value.MeanValue, Y_device, (int)kvp.Value.MeanValue, Y_device + height );

                        }
                    }
                }

                if(Database.Distribution.Item2 != null)
                {
                    var D2 =  Database.Distribution.Item2;
                    foreach(var kvp in D2)
                    {
                        if(kvp.Value.MeanValue <= HistoY.Bottom && kvp.Value.MeanValue >= HistoY.Top)
                        {
                            int height = (int)(kvp.Key.getMax() - kvp.Key.getMin() - 1);
                            //int width = (int)(HistoY.Width * (kvp.Value.Count /(double)Database.MaxCount.Item2));
                            int width = (int)(HistoY.Width * kvp.Value.RelativeFrequency);
                            int Y_device = (int)(kvp.Key.getMax() - height);
                            int X_device = HistoY.Right - width;
                            // if(Y_device < HistoY.Top)
                            // { 
                            //     height = height - (HistoY.Top - Y_device);
                            //     Y_device = HistoY.Top;
                            // }
                            // if(Y_device + height > HistoY.Bottom) height = HistoY.Bottom - Y_device;

                            Rectangle rect = new Rectangle(X_device, Y_device, width, height);
                            
                            // if(rect.Top < HistoY.Bottom) 
                            HistoYRect.Add(rect);

                            g.FillRectangle(myBrushG, rect);

                            // rugplot
                            g.DrawLine(Pens.Green, X_device, HistoY.Bottom + 5, X_device, HistoY.Bottom);

                            // // mean
                            g.DrawLine(Pens.Red, X_device, (int) kvp.Value.MeanValue, HistoY.Right, (int) kvp.Value.MeanValue );
                            
                        }
                    }
                }

                // MedianX.CalculateQuartiles();
                //MedianY.CalculateQuartiles();
                
                // quartiles 
                int offsetX = ordX.Count / 4;
                int offsetY = ordY.Count / 4;

                for(int i = 1; i <= 3; i++)
                {
                    int _x = (int)(HistoX.Left + HistoX.Width * (double.Parse(ordX[offsetX * i]) - double.Parse(Database.Min.Item1)) / double.Parse(Database.Range.Item1)); 
                    g.DrawLine(Pens.Black, _x, HistoX.Top, _x, HistoX.Bottom );

                    int _y = (int) (HistoY.Bottom - HistoY.Height * (double.Parse(ordY[offsetY * i]) - double.Parse(Database.Min.Item2)) / double.Parse(Database.Range.Item2));
                    g.DrawLine(Pens.Black, HistoY.Left, _y, HistoY.Right, _y );

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

                        var new_rect = new Rectangle(rect.Left, rect.Bottom, rect.Width, h);
                        g.FillRectangle(myBrushB, new_rect);
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

                        var new_rect = new Rectangle(HistoY.Right - w - orderedY[_i].Width, rect.Top, w, rect.Height);
                        g.FillRectangle(myBrushB, new_rect);
                    }
                    _i++;
                }
                Chart.Image = b;
            }
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

        //
        //
        //
        private void InitializeVariables()
        {
            var Variables = Dataset.GetNumericVariables();
            foreach(var Var in Variables)
            {
                Console.WriteLine(Var);
                SelectVarX.Items.Add(Var);
                SelectVarY.Items.Add(Var);
            }

            SelectVarX.SelectedIndex = SelectVarX.Items.Count - 1;
            SelectVarY.SelectedIndex = 0;
        }
        //
        // Init Labels
        //
        private void InitializeLabels()
        {
            foreach(var label in Dataset.Labels)
            {
                SelectLabel.Items.Add(label);
            }
        }

        private void InitializeLegend()
        {
            int alignX = Legend.Left + (int) (Legend.Width*0.1);
            int marginY = (int)(Legend.Height * 0.1);

            int X_device = alignX;
            int Y_device = Legend.Top + marginY;

            g.DrawString("Legend:", SmallFont, Brushes.Black, new Point(X_device, Y_device -8));

            Y_device += marginY - 2;

            g.FillEllipse(Brushes.Blue, new Rectangle(new Point(X_device - 4, Y_device - 4), new Size(8, 8)));
            g.DrawString("Sample point", SmallFont, Brushes.Black, new Point(X_device + 20, Y_device -8));
            
            Y_device += marginY - 2;
            g.FillEllipse(Brushes.Red, new Rectangle(new Point(X_device - 4, Y_device - 4), new Size(8, 8)));
            g.DrawString("Mean point", SmallFont, Brushes.Black, new Point(X_device + 20, Y_device - 8));

            Y_device += marginY - 2;

            g.DrawLine(Pens.Blue, X_device, Y_device - (int)(marginY*0.25),X_device,Y_device + (int)(marginY*0.25));
            g.DrawLine(Pens.Blue, X_device +10, Y_device - (int)(marginY*0.25),X_device +10,Y_device + (int)(marginY*0.25));
            g.DrawLine(Pens.Blue, X_device +20, Y_device - (int)(marginY*0.25),X_device +20,Y_device + (int)(marginY*0.25));
            g.DrawLine(Pens.Blue, X_device + 30, Y_device - (int)(marginY*0.25),X_device + 30,Y_device + (int)(marginY*0.25));
            
            g.DrawString("Sample rugplot", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));
            
            Y_device += marginY - 2;

            g.FillRectangle(myBrushG, X_device, Y_device - (int)(marginY*0.25),30,(int)(marginY*0.5));
            g.DrawString("Interval count", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));

            Y_device += marginY - 2;

            g.FillRectangle(myBrushB, X_device, Y_device - (int)(marginY*0.25),30,(int)(marginY*0.5));
            g.DrawString("CDF", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));

            Y_device += marginY - 2;

            g.DrawLine(Pens.Green, X_device, Y_device - (int)(marginY*0.25),X_device,Y_device + (int)(marginY*0.25));
            g.DrawLine(Pens.Green, X_device +10, Y_device - (int)(marginY*0.25),X_device +10,Y_device + (int)(marginY*0.25));
            g.DrawLine(Pens.Green, X_device +20, Y_device - (int)(marginY*0.25),X_device +20,Y_device + (int)(marginY*0.25));
            g.DrawLine(Pens.Green, X_device + 30, Y_device - (int)(marginY*0.25),X_device + 30,Y_device + (int)(marginY*0.25));
            g.DrawString("Count rugplot", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));

            Y_device += marginY - 2;

            g.DrawLine(Pens.Red, X_device, Y_device,X_device + 30,Y_device);
            g.DrawString("Intrainterval\nmean", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));

            Y_device += marginY;

            g.DrawLine(myPen, X_device, Y_device,X_device + 30,Y_device);
            g.DrawString("Linear Regression", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));

            Y_device += marginY;

            g.DrawLine(Pens.Black, X_device, Y_device,X_device + 30,Y_device);
            g.DrawString("Quartiles", SmallFont, Brushes.Black, new Point(X_device + 40, Y_device - (int)(marginY*0.25)));
        }

        //
        // Init Graphics
        //
        private void InitializeGraphics()
        {
            //if (pictureBox.Width < 2) pictureBox.Width = 2;
            //if (pictureBox.Height < 2) pictureBox.Height = 2;
            
            Viewport = new Rectangle(0, 0, Chart.Width - 1, Chart.Height - 1);
            int smallW = (int)(Viewport.Width*0.3);
            int bigW = (int)(Viewport.Width*0.6);
            int marginX = (int)(Viewport.Width*0.03);

            int smallH = (int)(Viewport.Height*0.3);
            int bigH = (int)(Viewport.Height*0.6);
            int marginY = (int)(Viewport.Height*0.03);

            HistoY = new Rectangle(marginX, marginY, smallW, bigH);
            Scatterplot = new Rectangle(smallW + 2*marginX, marginY, bigW, bigH);
            HistoX = new Rectangle(2*marginX + smallW, 2*marginY + bigH, bigW, smallH);

            Legend = new Rectangle(marginX, 2*marginY + bigH, 145, 200);

            b = new Bitmap(Chart.Width, Chart.Height);
            g = Graphics.FromImage(b);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, Viewport);
            g.DrawLine(Pens.Blue, Scatterplot.Left, Scatterplot.Bottom,Scatterplot.Right, Scatterplot.Bottom);
            g.DrawLine(Pens.Blue, Scatterplot.Left, Scatterplot.Bottom,Scatterplot.Left, Scatterplot.Top);

            g.DrawLine(Pens.Green, HistoX.Left, HistoX.Top,HistoX.Right, HistoX.Top);
            g.DrawLine(Pens.Green, HistoX.Left, HistoX.Top,HistoX.Left, HistoX.Bottom);

            g.DrawLine(Pens.Green, HistoY.Right, HistoY.Bottom,HistoY.Left, HistoY.Bottom);
            g.DrawLine(Pens.Green, HistoY.Right, HistoY.Bottom,HistoY.Right, HistoY.Top);
            
            g.DrawRectangle(Pens.Black, Legend);
            InitializeLegend();
            // g.DrawRectangle(Pens.Blue, Scatterplot);
            // g.DrawRectangle(Pens.Green, HistoY);
            // g.DrawRectangle(Pens.Green, HistoX);
            Chart.Image = b;
        }

        //---------------------------------------------------------------------------------------------------//
        //
        // PLOT
        //
        private void onCTClick(object sender, EventArgs e)
        {
            Console.WriteLine("Contingency Table");
            FormCT CT = new FormCT(Database);
            CT.Show();
            ContingencyTableButton.Enabled = false;
        }

        //---------------------------------------------------------------------------------------------------//
        //
        // PLOT
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

        //
        //
        //
        private void onVarXChange(object sender, System.EventArgs e)
        {
            Console.WriteLine(SelectVarX.Text);
            draw();
        }

        //
        //
        //
        private void onVarYChange(object sender, System.EventArgs e)
        {
            Console.WriteLine(SelectVarY.Text);
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
        private System.Windows.Forms.ComboBox SelectVarX;
        private System.Windows.Forms.Button PickXColor;
        private System.Windows.Forms.RichTextBox DisplayXColor;
        private System.Windows.Forms.NumericUpDown IntervalX;

        private System.Windows.Forms.GroupBox VarYSettings;
        private System.Windows.Forms.ComboBox SelectVarY;
        private System.Windows.Forms.Button PickYColor;
        private System.Windows.Forms.RichTextBox DisplayYColor;
        private System.Windows.Forms.NumericUpDown IntervalY;

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