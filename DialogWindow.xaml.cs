using System.Windows;

namespace PartsManager
{
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
            SetHandlers();
        }

        public DialogWindow(string message)
        {
            InitializeComponent();
            SetHandlers();
            MessageBlock.Text = message;
        }

        public void SetHandlers()
        {
            CancelButton.Click += delegate
            {
                DialogResult = false;
                Close();
            };

            OkButton.Click += delegate
            {
                DialogResult = true;
                Close();
            };
        }
    }
}
