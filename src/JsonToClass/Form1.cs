using System.Text;
using System.Text.Json;

namespace JsonToClass
{
    public partial class Form1 : Form
    {
        private static JsonSerializerOptions jsonOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new EnumStringConverter()
            }
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
            var factory = (comboBox_sourceType.SelectedItem as ComboBoxLangItem)?.ConverterFactory;
            if (factory == null)
            {
                return;
            }

            var item = (LangConfig)comboBox_lang.SelectedItem;
            if (item == null)
            {
                return;
            }
            var converter = factory(item);
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

            comboBox_sourceType.BeginUpdate();
            comboBox_sourceType.Items.Add(new ComboBoxLangItem() { Text = "Json", ConverterFactory = option => new LangJsonConverter(option) });
            comboBox_sourceType.Items.Add(new ComboBoxLangItem() { Text = "Line", ConverterFactory = option => new LangLineConverter(option) });
            comboBox_sourceType.EndUpdate();
            comboBox_sourceType.SelectedIndex = 0;

            comboBox_lang.BeginUpdate();
            foreach (var item in configs)
            {
                comboBox_lang.Items.Add(item);
            }
            comboBox_lang.EndUpdate();
            comboBox_lang.SelectedIndex = 0;
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

    internal class ComboBoxLangItem
    {
        public string? Text { get; set; }
        public Func<ClassOption, ILangConverter>? ConverterFactory { get; set; }

        public override string ToString()
        {
            return Text ?? "";
        }
    }
}