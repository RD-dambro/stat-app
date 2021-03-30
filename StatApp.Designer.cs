using MyControls;

namespace stat_app
{
    partial class StatApp
    {

        private void InitializeComponent(int width, int height)
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(this.Width, this.Height);

            this.Text = "StatApp";

            this.Width = width;
            int sidebarWidth = (int)(this.Width*0.3);

            this.ImportDataSettings = new System.Windows.Forms.GroupBox();
            this.BrowseButton = new System.Windows.Forms.Button(); 
            this.Path = new System.Windows.Forms.RichTextBox(); 
            this.ClearButton = new System.Windows.Forms.Button(); 
            this.LoadButton = new System.Windows.Forms.Button(); 

            this.BrowseButton.Click += new System.EventHandler(this.onBrowseButtonClick);        
            this.ClearButton.Click += new System.EventHandler(this.onClearButtonClick); 
            this.LoadButton.Click += new System.EventHandler(this.onLoadButtonClick);
            this.Path.AllowDrop = true;            
            this.Path.DragDrop += new System.Windows.Forms.DragEventHandler(this.onPathDragDrop);

            MyImportCSV ctr = new MyImportCSV();
            this.Height = ctr.ImportCSV(0 ,0 ,sidebarWidth, this.ImportDataSettings, this.BrowseButton, this.ClearButton, this.Path, this.LoadButton);

            this.ImportSettingsSettings = new System.Windows.Forms.GroupBox();
            this.ImportLabels = new System.Windows.Forms.CheckBox();
            this.DelimiterLabel = new System.Windows.Forms.Label();
            this.Delimiter = new System.Windows.Forms.TextBox();
            this.DeleteSelectedButton = new System.Windows.Forms.Button();

            this.DeleteSelectedButton.Click += new System.EventHandler(this.onDeleteSelectedButtonClick);        
            this.ImportLabels.CheckedChanged += new System.EventHandler(this.LabelsChecked);

            this.Height += ctr.ImportSettings(0, this.Height, sidebarWidth, this.ImportSettingsSettings, this.DeleteSelectedButton, this.ImportLabels, this.DelimiterLabel, this.Delimiter);

            this.ChartSettings = new System.Windows.Forms.GroupBox();
            this.UnivariateButton = new System.Windows.Forms.Button();
            this.BivariateButton = new System.Windows.Forms.Button();

            this.UnivariateButton.Click += new System.EventHandler(this.onUnivariateAnalysisButtonClick);
            this.BivariateButton.Click += new System.EventHandler(this.onBivariateAnalysisButtonClick);

            MyButton btn = new MyButton();

            this.Height += btn.Init(0, this.Height, sidebarWidth, this.ChartSettings, this.BivariateButton,  "Chart Options","Bivariate Analysis");
            btn.Add(this.ChartSettings, this.UnivariateButton,"Riemann/Lebesgue Integration");


            this.DisplayTypesSettings = new System.Windows.Forms.GroupBox();

            this.TypesTreeView = new System.Windows.Forms.TreeView();

            this.TypesTreeView.CheckBoxes = true;
            this.TypesTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.AfterCheck);

            this.Height += ctr.DisplayVariableTypes(0, this.Height, sidebarWidth, this.DisplayTypesSettings, this.TypesTreeView);

            this.DisplayDataSettings = new System.Windows.Forms.GroupBox();
            this.DataTable = new System.Windows.Forms.ListView();

            this.DataTable.View = System.Windows.Forms.View.Details;
            this.DataTable.FullRowSelect = true;
            this.DataTable.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.onChangeSelect); 
            int temp = ctr.DisplayData(sidebarWidth, 0, width - sidebarWidth, 0, this.DisplayDataSettings, this.DataTable);

            this.Controls.Add(this.ImportDataSettings);
            this.Controls.Add(this.ImportSettingsSettings);
            this.Controls.Add(this.ChartSettings);
            this.Controls.Add(this.DisplayTypesSettings);
            if(temp > this.Height) 
            {
                this.Height = temp;
            }
            else
            {
                this.DisplayDataSettings.Height = this.Height - 30;
            }
            this.Controls.Add(this.DisplayDataSettings);

            this.ClientSize = new System.Drawing.Size(this.Width, this.Height);
        }


        private System.Windows.Forms.GroupBox ImportDataSettings;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.RichTextBox Path;
        private System.Windows.Forms.Button LoadButton;


        private System.Windows.Forms.GroupBox ImportSettingsSettings;
        private System.Windows.Forms.CheckBox ImportLabels;
        private System.Windows.Forms.Label DelimiterLabel;
        private System.Windows.Forms.TextBox Delimiter;
        private System.Windows.Forms.Button DeleteSelectedButton;

        private System.Windows.Forms.GroupBox ChartSettings;
        private System.Windows.Forms.Button UnivariateButton;
        private System.Windows.Forms.Button BivariateButton;

        private System.Windows.Forms.GroupBox DisplayTypesSettings;
        private System.Windows.Forms.TreeView TypesTreeView;

        private System.Windows.Forms.GroupBox DisplayDataSettings;
        private System.Windows.Forms.ListView DataTable;
    }
}

