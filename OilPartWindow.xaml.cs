using PartsManager.BaseHandlers;
using PartsManager.Model.Entities;
using PartsManager.Model.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PartsManager
{
    /// <summary>
    /// Interaction logic for OilPartWindow.xaml
    /// </summary>
    public partial class OilPartWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isPartSelected;
        private bool isInfoCreated;
        private AdditionalInfo localAdditionalInfo;
        private ICollection<PartApiStandard> partApiStandards;
        private ICollection<PartManufacturerStandard> partManufacturerStandards;

        public bool IsPartSelected
        {
            get { return isPartSelected; }
            set
            {
                isPartSelected = value;
                OnPropertyChanged("IsPartSelected");
            }
        }
        public bool IsInfoCreated
        {
            get { return isInfoCreated; }
            set
            {
                isInfoCreated = value;
                OnPropertyChanged("IsInfoCreated");
            }
        }
        public AdditionalInfo LocalAdditionalInfo
        {
            get { return localAdditionalInfo; }
            set
            {
                localAdditionalInfo = value;
                OnPropertyChanged("LocalAdditionalInfo");
            }
        }
        public ICollection<PartApiStandard> PartApiStandards
        {
            get { return partApiStandards; }
            set
            {
                partApiStandards = value;
                OnPropertyChanged("PartApiStandards");
            }
        }
        public ICollection<PartManufacturerStandard> PartManufacturerStandards
        {
            get { return partManufacturerStandards; }
            set
            {
                partManufacturerStandards = value;
                OnPropertyChanged("PartManufacturerStandards");
            }
        }
        private EFUnitOfWork unitOfWork = EFUnitOfWork.GetUnitOfWork("DataContext");

        public OilPartWindow()
        {
            InitializeComponent();
            LocalAdditionalInfo = new AdditionalInfo()
            {
                Part = new Part(),
            };
            DataContext = this;
            SetHandlers();
            IsPartSelected = false;
            IsInfoCreated = false;
        }

        public OilPartWindow(Part part)
        {
            InitializeComponent();
            LocalAdditionalInfo = new AdditionalInfo()
            {
                Part = new Part(),
            };
            DataContext = this;
            SetHandlers();
            IsPartSelected = true;
            PartApiStandards = unitOfWork.PartApiStandards.GetAll().Where(item => item.PartId == part.Id).ToList();
            PartManufacturerStandards = unitOfWork.PartManufacturerStandards.GetAll().Where(item => item.PartId == part.Id).ToList();
            var info = unitOfWork.AdditionalInfos.GetAll().Where(item => item.PartId == part.Id).ToList();
            if (info.Any())
            {
                LocalAdditionalInfo = info.First();
                ManufacturerBox.Text = info.First().Manufacturer.Name;
                SaeQualityStandardBox.Text = info.First().SaeQualityStandard.Name;
                IsInfoCreated = true;
            }
            else
            {
                LocalAdditionalInfo = new AdditionalInfo()
                {
                    Part = part,
                };
                IsInfoCreated = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void ChangeButtons(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInfoCreated")
            {
                if (IsInfoCreated)
                {
                    WorkButton.Content = "Редагувати стандарти оливи";
                }
                else
                {
                    WorkButton.Content = "Додати стандарти до оливи";
                }
            }

            if (e.PropertyName == "IsPartSelected")
            {
                if (IsPartSelected)
                {
                    PartNotificationBlock.Text = "Олива обрана";
                    PartNotificationBlock.Foreground = Brushes.DarkGreen;
                    WorkButton.IsEnabled = true;
                    AddManufacturerStandardButton.IsEnabled = true;
                    AddMultipleManufacturerStandardButton.IsEnabled = true;
                    AddApiStandardButton.IsEnabled = true;
                }
                else
                {
                    PartNotificationBlock.Text = "Для додавання стандартів спочатку необхідно обрати оливу";
                    PartNotificationBlock.Foreground = Brushes.DarkRed;
                    WorkButton.IsEnabled = false;
                    AddManufacturerStandardButton.IsEnabled = false;
                    AddMultipleManufacturerStandardButton.IsEnabled = false;
                    AddApiStandardButton.IsEnabled = false;
                }
            }
        }

        public void SetHandlers()
        {
            PropertyChanged += ChangeButtons;

            SelectPartButton.Click += delegate
            {
                PartSelectionWindow partSelectionWindow = new PartSelectionWindow("Олива");
                bool? dialogResult = partSelectionWindow.ShowDialog();
                if (dialogResult != true)
                    return;
                else
                {
                    IsPartSelected = true;
                    PartApiStandards = unitOfWork.PartApiStandards.GetAll().Where(item => item.PartId == partSelectionWindow.LocalPart.Id).ToList();
                    PartManufacturerStandards = unitOfWork.PartManufacturerStandards.GetAll().Where(item => item.PartId == partSelectionWindow.LocalPart.Id).ToList();
                    var info = unitOfWork.AdditionalInfos.GetAll().Where(item => item.PartId == partSelectionWindow.LocalPart.Id).ToList();
                    if (info.Any())
                    {
                        LocalAdditionalInfo = info.First();
                        //LocalAdditionalInfo = new AdditionalInfo()
                        //{
                        //    Id = info.First().Id,
                        //    Info = info.First().Info,
                        //    Part = partSelectionWindow.LocalPart,
                        //    PartId = partSelectionWindow.LocalPart.Id,
                        //};
                        ManufacturerBox.Text = info.First().Manufacturer.Name;
                        SaeQualityStandardBox.Text = info.First().SaeQualityStandard.Name;
                        IsInfoCreated = true;
                    }
                    else
                    {
                        LocalAdditionalInfo = new AdditionalInfo()
                        {
                            Part = partSelectionWindow.LocalPart,
                        };
                        IsInfoCreated = false;
                    }
                }
            };
            WorkButton.Click += delegate
            {
                if (ManufacturerBox.Text == string.Empty || SaeQualityStandardBox.Text == string.Empty)
                    return;

                var manufacturers = unitOfWork.Manufacturers.Find(item => item.Name == ManufacturerBox.Text).ToList();
                var saeQualityStandards = unitOfWork.SaeQualityStandards.Find(item => item.Name == SaeQualityStandardBox.Text).ToList();

                var IsNoManufacturer = manufacturers.Count == 0;
                var IsNoSaeQualityStandard = saeQualityStandards.Count == 0;
                string message = "Для додавання також треба створити";

                if (IsNoManufacturer)
                {
                    message += $" виробника \"{ManufacturerBox.Text}\"";
                }
                if (IsNoSaeQualityStandard)
                {
                    message += $" норму якості SAE \"{SaeQualityStandardBox.Text}\"";
                }
                if (IsNoSaeQualityStandard || IsNoManufacturer)
                {
                    message += "\nВи згодні з створенням?";
                    DialogWindow dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    if (IsNoManufacturer)
                    {
                        var manufacturer = new Manufacturer()
                        {
                            Name = ManufacturerBox.Text,
                        };

                        unitOfWork.Manufacturers.Create(manufacturer);
                        unitOfWork.Save();

                        LocalAdditionalInfo.Manufacturer = manufacturer;
                    }

                    if (IsNoSaeQualityStandard)
                    {
                        var saeQualityStandard = new SaeQualityStandard()
                        {
                            Name = SaeQualityStandardBox.Text,
                        };

                        unitOfWork.SaeQualityStandards.Create(saeQualityStandard);
                        unitOfWork.Save();

                        LocalAdditionalInfo.SaeQualityStandard = saeQualityStandard;
                    }
                }
                if (!IsNoManufacturer)
                {
                    if (manufacturers.Any())
                    {
                        LocalAdditionalInfo.Manufacturer = manufacturers.First();
                    }
                    else return;
                }
                if (!IsNoSaeQualityStandard)
                {
                    if (saeQualityStandards.Any())
                    {
                        LocalAdditionalInfo.SaeQualityStandard = saeQualityStandards.First();
                    }
                    else return;
                }

                if (IsInfoCreated)
                {
                    unitOfWork.AdditionalInfos.Update(LocalAdditionalInfo);
                    unitOfWork.Save();
                    PartNotificationBlock.Text = "Стандарти відредаговані";
                    PartNotificationBlock.Foreground = Brushes.DarkGreen;
                }
                else
                {
                    unitOfWork.AdditionalInfos.Create(LocalAdditionalInfo);
                    unitOfWork.Save();
                    PartNotificationBlock.Text = "Стандарти додані";
                    PartNotificationBlock.Foreground = Brushes.DarkGreen;
                }
            };
            AddMultipleManufacturerStandardButton.Click += delegate
            {
                if (MultipleManufacturerStandardBox.Text == string.Empty)
                    return;
                var manufacturerStandards = new List<string>(MultipleManufacturerStandardBox.Text.Split('\n'));
                manufacturerStandards = manufacturerStandards.Select(item => item.Trim()).ToList();

                var existingManufacturerStandards = unitOfWork.ManufacturerStandards.GetAll().Where(item => manufacturerStandards.Any(item2 => item2 == item.Name)).ToList();
                var allManufacturerStandards = unitOfWork.ManufacturerStandards.GetAll().ToList();
                var partManufacturerStandards = unitOfWork.PartManufacturerStandards.Find(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
                manufacturerStandards = manufacturerStandards.Where(item => allManufacturerStandards.All(item2 => item2.Name != item)).ToList();

                if (existingManufacturerStandards.Any())
                {
                    existingManufacturerStandards = existingManufacturerStandards.Where(item => partManufacturerStandards.All(item2 => item2.Id != item.Id)).ToList();

                    var partManufacturerStandardsToAdd = existingManufacturerStandards.Select(item => new PartManufacturerStandard()
                    {
                        ManufacturerStandard = item,
                        PartId = LocalAdditionalInfo.Part.Id,
                    });

                    foreach (var partManufacturerStandard in partManufacturerStandardsToAdd)
                    {
                        unitOfWork.PartManufacturerStandards.Create(partManufacturerStandard);
                    }
                    unitOfWork.Save();
                }

                if (manufacturerStandards.Any())
                {
                    string message = $"Для додавання також треба створити стандарти\n\"{string.Join("\",\n\"", manufacturerStandards)}\"";
                    DialogWindow dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var manufacturerStandartsToAdd = manufacturerStandards.Select(item => new ManufacturerStandard() { Name = item });

                    var partManufacturerStandardsToAdd = manufacturerStandartsToAdd.Select(item => new PartManufacturerStandard()
                    {
                        ManufacturerStandard = item,
                        PartId = LocalAdditionalInfo.Part.Id,
                    });

                    foreach (var partManufacturerStandard in partManufacturerStandardsToAdd)
                    {
                        unitOfWork.PartManufacturerStandards.Create(partManufacturerStandard);
                    }
                    unitOfWork.Save();
                }

                PartManufacturerStandards = unitOfWork.PartManufacturerStandards.GetAll().Where(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
            };
            AddManufacturerStandardButton.Click += delegate
            {
                if (ManufacturerStandardBox.Text == string.Empty)
                    return;
                var partManufacturerStandard = new PartManufacturerStandard()
                {
                    Part = LocalAdditionalInfo.Part,
                };

                var manufacturerStandards = unitOfWork.ManufacturerStandards.Find(item => item.Name == ManufacturerStandardBox.Text).ToList();
                if (manufacturerStandards.Any())
                {
                    if (unitOfWork.PartManufacturerStandards.Find(item => item.ManufacturerStandardId == manufacturerStandards.First().Id
                        && item.PartId == LocalAdditionalInfo.Part.Id).Any())
                    {
                        return;
                    }
                    partManufacturerStandard.ManufacturerStandard = manufacturerStandards.First();
                }
                else
                {
                    string message = $"Для додавання також треба створити стандарт \"{ManufacturerStandardBox.Text}\"";
                    DialogWindow dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var manufacturerStandard = new ManufacturerStandard()
                    {
                        Name = ManufacturerStandardBox.Text,
                    };

                    unitOfWork.ManufacturerStandards.Create(manufacturerStandard);
                    unitOfWork.Save();

                    partManufacturerStandard.ManufacturerStandard = manufacturerStandard;
                }

                unitOfWork.PartManufacturerStandards.Create(partManufacturerStandard);
                unitOfWork.Save();

                PartManufacturerStandards = unitOfWork.PartManufacturerStandards.GetAll().Where(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
            };
            AddMultipleApiStandardButton.Click += delegate
            {
                if (MultipleApiStandardBox.Text == string.Empty)
                    return;
                var apiStandards = new List<string>(MultipleApiStandardBox.Text.Split('\n'));
                apiStandards = apiStandards.Select(item => item.Trim()).ToList();

                var existingApiStandards = unitOfWork.ApiStandards.GetAll().Where(item => apiStandards.Any(item2 => item2 == item.Name)).ToList();
                var allApiStandards = unitOfWork.ApiStandards.GetAll().ToList();
                var partApiStandards = unitOfWork.PartApiStandards.Find(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
                apiStandards = apiStandards.Where(item => allApiStandards.All(item2 => item2.Name != item)).ToList();

                if (existingApiStandards.Any())
                {
                    existingApiStandards = existingApiStandards.Where(item => partApiStandards.All(item2 => item2.Id != item.Id)).ToList();

                    var partApiStandardsToAdd = existingApiStandards.Select(item => new PartApiStandard()
                    {
                        ApiStandard = item,
                        PartId = LocalAdditionalInfo.Part.Id,
                    });

                    foreach (var partApiStandard in partApiStandardsToAdd)
                    {
                        unitOfWork.PartApiStandards.Create(partApiStandard);
                    }
                    unitOfWork.Save();
                }

                if (apiStandards.Any())
                {
                    string message = $"Для додавання також треба створити стандарти\n\"{string.Join("\",\n\"", apiStandards)}\"";
                    DialogWindow dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var apiStandartsToAdd = apiStandards.Select(item => new ApiStandard() { Name = item });

                    var partApiStandardsToAdd = apiStandartsToAdd.Select(item => new PartApiStandard()
                    {
                        ApiStandard = item,
                        PartId = LocalAdditionalInfo.Part.Id,
                    });

                    foreach (var partApiStandard in partApiStandardsToAdd)
                    {
                        unitOfWork.PartApiStandards.Create(partApiStandard);
                    }
                    unitOfWork.Save();
                }

                PartApiStandards = unitOfWork.PartApiStandards.GetAll().Where(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
            };
            AddApiStandardButton.Click += delegate
            {
                if (ApiStandardBox.Text == string.Empty)
                    return;

                var partApiStandard = new PartApiStandard()
                {
                    Part = LocalAdditionalInfo.Part,
                };

                var apiStandards = unitOfWork.ApiStandards.Find(item => item.Name == ApiStandardBox.Text).ToList();
                if (apiStandards.Any())
                {
                    if (unitOfWork.PartApiStandards.Find(item => item.ApiStandardId == apiStandards.First().Id
                        && item.PartId == LocalAdditionalInfo.Part.Id).Any())
                    {
                        return;
                    }
                    partApiStandard.ApiStandard = apiStandards.First();
                }
                else
                {
                    string message = $"Для додавання також треба створити специфікацію API \"{ApiStandardBox.Text}\"";
                    DialogWindow dialogWindow = new DialogWindow(message);
                    bool? dialogResult = dialogWindow.ShowDialog();
                    if (dialogResult != true)
                        return;

                    var apiStandard = new ApiStandard()
                    {
                        Name = ApiStandardBox.Text,
                    };

                    unitOfWork.ApiStandards.Create(apiStandard);
                    unitOfWork.Save();

                    partApiStandard.ApiStandard = apiStandard;
                }

                unitOfWork.PartApiStandards.Create(partApiStandard);
                unitOfWork.Save();

                PartApiStandards = unitOfWork.PartApiStandards.GetAll().Where(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
            };

            ManufacturerBox.SetDropDownOpened(unitOfWork.Manufacturers.GetAll());
            SaeQualityStandardBox.SetDropDownOpened(unitOfWork.SaeQualityStandards.GetAll());
            ManufacturerStandardBox.SetDropDownOpened(unitOfWork.ManufacturerStandards.GetAll());
            ApiStandardBox.SetDropDownOpened(unitOfWork.ManufacturerStandards.GetAll());
            ManufacturerBox.SetTextChangedFirstCharToUpper();
            SaeQualityStandardBox.SetTextChangedFirstCharToUpper();
            ManufacturerStandardBox.SetTextChangedFirstCharToUpper();
            ApiStandardBox.SetTextChangedFirstCharToUpper();
        }

        public void DeleteApiStandardOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            var apiStandardToDelete = unitOfWork.PartApiStandards.Get((int)button.Tag);

            string message = $"Ви впевнені що хочете видалити специфікацію API \"{apiStandardToDelete.ApiStandard.Name}\" з оливи?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.PartApiStandards.Delete(apiStandardToDelete.Id);
            unitOfWork.Save();

            PartApiStandards = unitOfWork.PartApiStandards.GetAll().Where(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
        }
        public void DeleteManufacturerStandardOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            var manufacturerStandardToDelete = unitOfWork.PartManufacturerStandards.Get((int)button.Tag);

            string message = $"Ви впевнені що хочете видалити стандарт \"{manufacturerStandardToDelete.ManufacturerStandard.Name}\" з оливи?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            unitOfWork.PartManufacturerStandards.Delete(manufacturerStandardToDelete.Id);
            unitOfWork.Save();

            PartManufacturerStandards = unitOfWork.PartManufacturerStandards.GetAll().Where(item => item.PartId == LocalAdditionalInfo.Part.Id).ToList();
        }
    }
}
