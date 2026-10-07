namespace SafeParentControl.App
{
    using System;
    using System.Windows.Forms;
    using SafeParentControl.Methods;
    using SafeParentControl.Queries;

    public partial class Form1 : Form
    {
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuClearLog;
        private ToolStripMenuItem menuExit;
        
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox txtUrl;
        private Button btnCheck;
        private Label lblResult;
        private ListBox lstThreats;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUI();

            // Инициализируем базу данных при запуске
            DatabaseManager.InitializeDatabase();
        }

        private void InitializeCustomUI()
        {
            this.Text = "Safe Parent Control - .NET 10";
            this.Width = 650;
            this.Height = 500;

            // 1. Создаем верхнее меню (MenuStrip)
            menuStrip1 = new MenuStrip();
            menuFile = new ToolStripMenuItem("Файл");
            
            menuClearLog = new ToolStripMenuItem("Очистить журнал");
            menuClearLog.Click += (s, e) => lstThreats.Items.Clear();

            menuExit = new ToolStripMenuItem("Выход");
            menuExit.Click += (s, e) => Application.Exit();

            menuFile.DropDownItems.Add(menuClearLog);
            menuFile.DropDownItems.Add(new ToolStripSeparator());
            menuFile.DropDownItems.Add(menuExit);
            menuStrip1.Items.Add(menuFile);

            this.MainMenuStrip = menuStrip1;
            this.Controls.Add(menuStrip1);

            // 2. Создаем вкладки (TabControl) под меню
            tabControl1 = new TabControl();
            tabControl1.Dock = DockStyle.Fill;
            // Делаем отступ сверху, чтобы вкладки не перекрывали меню
            tabControl1.Top = menuStrip1.Height;
            tabControl1.Height = this.ClientSize.Height - menuStrip1.Height;

            tabPage1 = new TabPage("Проверка сайтов");
            tabPage2 = new TabPage("Журнал угроз");

            // Элементы первой вкладки
            lblResult = new Label();
            lblResult.Text = "Введите URL для проверки:";
            lblResult.Top = 30;
            lblResult.Left = 30;
            lblResult.Width = 400;

            txtUrl = new TextBox();
            txtUrl.Top = 60;
            txtUrl.Left = 30;
            txtUrl.Width = 450;

            btnCheck = new Button();
            btnCheck.Text = "Проверить";
            btnCheck.Top = 100;
            btnCheck.Left = 30;
            btnCheck.Width = 120;
            btnCheck.Height = 35;
            btnCheck.Click += BtnCheck_Click;

            tabPage1.Controls.Add(lblResult);
            tabPage1.Controls.Add(txtUrl);
            tabPage1.Controls.Add(btnCheck);

            // Элементы второй вкладки
            lstThreats = new ListBox();
            lstThreats.Dock = DockStyle.Fill;
            tabPage2.Controls.Add(lstThreats);

            tabControl1.TabPages.Add(tabPage1);
            tabControl1.TabPages.Add(tabPage2);

            this.Controls.Add(tabControl1);
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                lblResult.Text = "Пожалуйста, введите URL!";
                return;
            }

            if (UrlChecker.IsUrlSafe(url))
            {
                lblResult.Text = "Статус: Сайт безопасен.";
            }
            else
            {
                lblResult.Text = "Статус: Обнаружена угроза!";
                // Сохраняем угрозу в базу данных SQLite через библиотеку запросов
                DatabaseManager.LogThreat(url, "Dangerous Content");
                lstThreats.Items.Add($"{DateTime.Now}: Заблокирован URL - {url}");
            }
        }
    }
}