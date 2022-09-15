using System.Text;
using System.Text.Json;

namespace JsonToClass
{
    public partial class Form1 : Form
    {
        private static JsonSerializerOptions jsonOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        public Form1()
        {
            InitializeComponent();
            this.SuspendLayout();
            textBox_result.MaxLength = textBox_origin.MaxLength = int.MaxValue;   
            this.ResumeLayout();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var item = (LangConfig)comboBox1.SelectedItem;
            var converter = new LangConverter(item);
            textBox_result.Text = converter.Convert(textBox_origin.Text);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
            {
                return;
            }

            Initialize();
            var configs = ReadConfig();
            if (configs == default)
            {
                return;
            }

            comboBox1.BeginUpdate();

            foreach (var item in configs)
            {
                comboBox1.Items.Add(item);
            }

            comboBox1.EndUpdate();
            comboBox1.SelectedIndex = 0;
        }

        private void Initialize()
        {
            if (!File.Exists(configPath))
            {
                using (var fs = new FileStream(configPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    var defaultConfig = JsonSerializer.Serialize(LanguageDefaultConfig.DefaultConfigs, jsonOption);
                    var buffer = Encoding.UTF8.GetBytes(defaultConfig);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                }
            }
        }

        private ICollection<LangConfig>? ReadConfig()
        {
            if (!File.Exists(configPath))
            {
                return default;
            }

            using (var fs = new FileStream(configPath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    var configJson = sr.ReadToEnd();
                    return JsonSerializer.Deserialize<ICollection<LangConfig>>(configJson, jsonOption);
                }
            }
        }
    }

    internal class ComboBoxItem
    {
        public string? Text { get; set; }
        public object? Tag { get; set; }

        public override string ToString()
        {
            return Text ?? "";
        }
    }
}