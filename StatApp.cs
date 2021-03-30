using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyControls;
using Statistics2020Library;
using System.Reflection;
using first_app;

namespace stat_app
{
    public partial class StatApp : Form
    {
        public StatApp()
        {   
            this.Load += new EventHandler(StatApp_Load);
            InitializeComponent(800, 600);
        }

        private void StatApp_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        //---------------------------------------------------------------------------------------------------//
        private string[,] RawDataset;
        private string last_file = "";
        private int rows;
        private int cols;
        private List<AlphanumericDataset> Columns;

        private GenericDataset myds;

        private HashSet<Type> ObservableDataTypes = new HashSet<Type>
        {
            typeof(double),
            typeof(int),
            typeof(DateTime),
            
            typeof(string)
        };
        //---------------------------------------------------------------------------------------------------//
        
        //
        // InitializeTreeView
        //
        private void InitializeTreeView()
        {
            TypesTreeView.BeginUpdate();

            TypesTreeView.Nodes.Clear();

            int var_index = 0;
            foreach (var Variable in Columns)
            {
                TypesTreeView.Nodes.Add(Variable.Name);

                foreach(var type in Variable.ObservedDataTypes)
                {
                    TypesTreeView.Nodes[var_index].Nodes.Add(type.Name);
                }
                TypesTreeView.Nodes[var_index].Checked = true;
                TypesTreeView.Nodes[var_index].Nodes[0].Checked = true;
                var_index++;
            }

            TypesTreeView.EndUpdate();

            TypesTreeView.ExpandAll();

            //if(Columns.Count > 0) this.ConfirmButton.Enabled = true;
        }
        //
        // InferDataTypes
        //
        private void InferDataTypes()
        {
            foreach (var col in Columns)
            {
                col.InferTypes(ObservableDataTypes);
            }

            InitializeTreeView();
        }
        
        //
        // Initialize List View
        //
        private void InitializeListView()
        {
            this.DataTable.Clear();
            this.DataTable.BeginUpdate();

            foreach(var col in Columns)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = col.Name;
                header.TextAlign = HorizontalAlignment.Left;
                header.Width = (this.DataTable.Width / cols) - 1;

                this.DataTable.Columns.Add(header);
            }
            int r = 0;
            if(ImportLabels.Checked) r++;
            
            while(r < rows)
            {
                ListViewItem item = new ListViewItem(RawDataset[r, 0]);
                if( DataTable.Items.Count % 2 == 1) 
                {
                    //item.ForeColor = Color.White;
                    item.BackColor = Color.FromArgb(50, Color.Black);
                
                }

                for(int c = 1; c < cols; c++)
                {
                    var obs = RawDataset[r, c];
                    item.SubItems.Add(obs);
                }
                this.DataTable.Items.Add(item);
                r++;
            }
            
            this.DataTable.EndUpdate();

            InferDataTypes();
        }


        private void LoadFile()
        {
            if(this.last_file != "")
                LoadFile(this.last_file);
        }
        //
        // Load File
        //
        private void LoadFile(string path)
        {
            this.last_file = path;
            RawDataset = ParseCSV(path);
            this.rows = RawDataset.GetLength(0);
            this.cols = RawDataset.GetLength(1);
            
            
            this.Columns = new List<AlphanumericDataset>();
            for(int c = 0; c < cols; c++)
            {
                string name = "X_" + (c).ToString();

                int r = 0;
                var UD = new AlphanumericDataset();
                UD.ListOfObservations = new List<string>();
                if(ImportLabels.Checked) 
                {
                    UD.Name = RawDataset[0, c];
                    r++;
                }
                else UD.Name = name;


                while(r < rows)
                {
                    var obs = RawDataset[r, c];
                    UD.ListOfObservations.Add(obs);
                    r++;
                }

                //UD.Log();
                this.Columns.Add(UD);
            }

            InitializeListView();
            
        }

        //
        // Parse CSV
        // Courtesy of https://www.code4example.com/csharp/csharp-console-application/how-to-read-csv-file-in-c/
        private string[,] ParseCSV(string path)
        {
            // Get the file's text.
            string whole_file = System.IO.File.ReadAllText(path);

            // Split into lines.
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' });
            lines = lines.Where(o => o.Trim() != "").ToArray();

            var delimiter = ',';
            if(this.Delimiter.Text.Length > 0)
            {

                delimiter = this.Delimiter.Text[0];
                Console.WriteLine(this.Delimiter.Text[0]);
            }
            // See how many rows and columns there are.
            int num_rows = lines.Length;
            int num_cols = lines[0].Split(delimiter).Length;

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 0; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(delimiter);
                for (int c = 0; c < num_cols; c++)
                {
                    values[r, c] = line_r[c].Trim();
                }
            }

            // Return the values.
            return values; 
        }

        //---------------------------------------------------------------------------------------------------//
        //---------------------------------------------------------------------------------------------------//
        //
        // Univariate Analysis Button Click
        //
        private void onUnivariateAnalysisButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("Univariate Analysis");
            Confirm();
            if(myds == null) return;
            
            FormUA UA = new FormUA(myds);
            UA.Show();
        }
        //---------------------------------------------------------------------------------------------------//
        //
        // Bivariate Analysis Button Click
        //
        private void onBivariateAnalysisButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("Bivariate Analysis");
            Confirm();
            if(myds == null) return;

            PlotData PD = new PlotData(myds);
            PD.Show();
        }

        //
        // Confirm Click
        //
        private void Confirm()
        {
            Console.WriteLine("confirm");

            if(Columns == null) return;
            
            myds = new GenericDataset();

            foreach(var col in Columns)
            {
                
                var v2 = col.GetType().GetProperty("DataType").GetValue(col, null);
                //Console.WriteLine(v2);


                if(col.DataType == typeof(int))
                {
                    var DS = new UnivariateDataset<int>() as IUnivariateDataset;
                    
                    DS.Init(col);
                    
                    if(!myds.UnivariateDatasets.ContainsKey(DS.Name)) myds.UnivariateDatasets.Add(DS.Name, DS);
                    else 
                    {
                        DS.Name  = DS.Name + "1";
                        myds.UnivariateDatasets.Add(DS.Name, DS);
                    }
                    myds.Labels.Add(DS.Name);
                }
                else if(col.DataType  == typeof(double))
                {
                    var DS = new UnivariateDataset<double>() as IUnivariateDataset;
                    
                    DS.Init(col);

                    if(!myds.UnivariateDatasets.ContainsKey(DS.Name)) myds.UnivariateDatasets.Add(DS.Name, DS);
                    else 
                    {
                        DS.Name  = DS.Name + "1";
                        myds.UnivariateDatasets.Add(DS.Name, DS);
                    }
                    myds.Labels.Add(DS.Name);
                }
                else if(col.DataType  == typeof(DateTime))
                {
                    
                    var DS = new UnivariateDataset<DateTime>() as IUnivariateDataset;
                    
                    DS.Init(col);
                    
                    if(!myds.UnivariateDatasets.ContainsKey(DS.Name)) myds.UnivariateDatasets.Add(DS.Name, DS);
                    else 
                    {
                        DS.Name  = DS.Name + "1";
                        myds.UnivariateDatasets.Add(DS.Name, DS);
                    }
                    
                    myds.Labels.Add(DS.Name);
                }
                else if(col.DataType  == typeof(string))
                {
                    
                    var DS = new UnivariateDataset<string>() as IUnivariateDataset;
                    
                    DS.Name = col.Name;
                    
                    DS.Init(col);

                    //DS.Log();
                    if(!myds.UnivariateDatasets.ContainsKey(DS.Name)) myds.UnivariateDatasets.Add(DS.Name, DS);
                    else 
                    {
                        DS.Name  = DS.Name + "1";
                        myds.UnivariateDatasets.Add(DS.Name, DS);
                    }
                    myds.Labels.Add(DS.Name);
                }      
                
            }

            //myds.Log();
            //StatApp parent = (StatApp)this.Owner;
            //parent.Confirm(myds);
            //this.Close();
        }

        //
        // Delete Selected Click
        //
        private void onDeleteSelectedButtonClick(object sender, EventArgs e)
        {
            this.DataTable.BeginUpdate();
            var Indices = DataTable.SelectedIndices;
            int Number = Indices.Count;
            
            for(int i = Number - 1 ; i >= 0; i--)
            {
                int index = Indices[i];
                foreach(var col in Columns)
                {
                    col.ListOfObservations.RemoveAt(index);
                }
                DataTable.Items.RemoveAt(index);

            }
            this.DataTable.EndUpdate();

            InferDataTypes();
        }

        //
        // Select Change
        //
        private void onChangeSelect(Object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int number = this.DataTable.SelectedItems.Count;
            if(number > 0) this.DeleteSelectedButton.Enabled = true;
            else this.DeleteSelectedButton.Enabled = false;
        }
        private void LabelsChecked(object sender, EventArgs e)
        {

            LoadFile();
            // Console.WriteLine("checked");
            
        }
        
        //
        // After Check
        //
        private void AfterCheck(object sender, TreeViewEventArgs e)
        {
            
            // (DATA TYPE) 
            if (e.Node.Nodes.Count == 0 && e.Node.Checked)
            {

                var Parent = e.Node.Parent;
                var Current = e.Node;
                // uncheck all siblings
                foreach (TreeNode node in Parent.Nodes)
                {
                    if (node != Current) node.Checked = false;
                }

                var query = Columns[Parent.Index].ObservedDataTypes.Where((type) => type.Name == Current.Text);
                foreach(var q in query)
                {
                    //Console.WriteLine(q);
                    Columns[Parent.Index].DataType = q;
                }
            }
            
        }

        //
        // Path MouseEnter
        //
        private void onMouseEnter(object sender, EventArgs e)
        {
            // Console.WriteLine("enter");
            // this.Path.Focus();
        }

        //
        // Path DragDrop
        //
        private void onPathDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Console.WriteLine("drop");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string dataPath = files[0];

                this.Path.Text = dataPath;

                Console.WriteLine(dataPath.Split('/'));
            }
        }
        
        //
        // Browse Button Click
        //
        private void onBrowseButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("Browse");
            using(var o = new OpenFileDialog())
            {
                o.ShowDialog();
                if(o.FileName != "") 
                {
                    var name = o.FileName.Split('/');
                    this.Path.Text = name[name.Length-1];
                    LoadFile(o.FileName);
                }
            }
        }

        //
        // Load Button Click
        //
        private void onLoadButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("Load");
            if(this.Path.Text.Length > 0 )
            {
                LoadFile(this.last_file);
            }
        }
        
        //
        // Clear Button Click
        //
        private void onClearButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("Clear");
            this.Path.Clear();
            this.DataTable.Clear();
            this.TypesTreeView.Nodes.Clear();
            this.last_file = "";
        }
    }
}