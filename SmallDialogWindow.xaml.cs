using System.Windows;

namespace PartsManager
{
    public partial class SmallDialogWindow : Window
    {
        public SmallDialogWindow()
        {
            InitializeComponent();
            SetHandlers();
        }

        public SmallDialogWindow(string message)
        {
            InitializeComponent();
            SetHandlers();
            MessageBlock.Text = message;
        }

        public void SetHandlers()
        {
            OkButton.Click += delegate
            {
                DialogResult = true;
                Close();
            };
        }
    }
}
