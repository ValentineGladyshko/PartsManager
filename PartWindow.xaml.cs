﻿using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System.Linq;
using System.Windows;

namespace PartsManager
{
    public partial class PartWindow : Window
    {
        private Part LocalPart { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public PartWindow()
        {
            InitializeComponent();

            LocalPart = new Part()
            {
                PartType = new PartType()
            };

            DataContext = LocalPart;
            Action = ActionType.Create;

            SetContent();
            SetHandlers();
        }

        public PartWindow(Part part, ActionType action)
        {
            InitializeComponent();

            LocalPart = part;
            Action = action;
            DataContext = LocalPart;

            SetContent();
            SetHandlers();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування запчастини";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення запчастини";
                WorkButton.Content = "Створити";
            }
            PartTypeNameBox.Text = LocalPart.PartType.Name;
        }

        public void SetHandlers()
        {
            CancelButton.Click += delegate { Close(); };

            WorkButton.Click += delegate
            {
                if (PartTypeNameBox.Text == string.Empty)
                    return;

                var partTypes = unitOfWork.PartTypes.Find(item => item.Name == PartTypeNameBox.Text).ToList();

                if (partTypes.Count == 0)
                {
                    string message = "Для " + TextBoxHelper.ActionText(Action) + " даної запчастини також треба створити тип \""
                            + PartTypeNameBox.Text + "\"\nВи згодні з створенням типу запчастини?";
                    var dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var partType = new PartType()
                    {
                        Name = PartTypeNameBox.Text,
                    };

                    unitOfWork.PartTypes.Create(partType);
                    unitOfWork.Save();
                    LocalPart.PartType = partType;
                }
                else
                {
                    LocalPart.PartType = partTypes.First();
                }

                if (Action == ActionType.Edit)
                {
                    unitOfWork.Parts.Update(LocalPart);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.Parts.Create(LocalPart);
                    unitOfWork.Save();
                    Close();
                }
            };

            PartTypeNameBox.SetDropDownOpened(unitOfWork.PartTypes.GetAll());
            PartTypeNameBox.SetTextChangedFirstCharToUpper();
            NameBox.SetTextChangedFirstCharToUpper();
            FullNameBox.SetTextChangedFirstCharToUpper();
            ArticleBox.SetTextChangedToUpper();
        }
    }
}
