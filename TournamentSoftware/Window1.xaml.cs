﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TournamentSoftware
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private ObservableCollection<NominationFormModel> nominationsList = new ObservableCollection<NominationFormModel>();
        private List<CheckBox> checkBoxes = new List<CheckBox>();
        ObservableCollection<DataGridColumn> mainWindowColumns = ((MainWindow)Application.Current.MainWindow).registrationTable.Columns;
        ObservableCollection<DataGridTemplateColumn> mainNominationsColumns = ((MainWindow)Application.Current.MainWindow).nominationsColumn;
        ObservableCollection<ParticipantFormModel> participants = MainWindow.participantsList;
        private List<NominationFormModel> nominationsForDelete = new List<NominationFormModel>();

        /// <summary>
        /// Закрываем окно настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeSettingsWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Применить настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setNewSettings(object sender, RoutedEventArgs e)
        {
            // скрываем ненужные столбцы или наоборот делаем их видимыми
            for (int i = 0; i < checkBoxes.Count; i++)
            {

                for (int j = 1; j < mainWindowColumns.Count; j++)
                {
                    if (mainWindowColumns[j].Header.Equals(checkBoxes[i].Content))
                    {
                        if (checkBoxes[i].IsChecked == false)
                        {
                            mainWindowColumns[j].Visibility = Visibility.Hidden;
                            break;
                        }
                        else
                        {
                            mainWindowColumns[j].Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }

            // добавляем новые номинации в таблицу регистрации
            for (int i = 0; i < nominationsList.Count; i++)
            {
                DataGridTemplateColumn nominationColumn = new DataGridTemplateColumn();
                string nominationName = nominationsList[i].Nomination.Name;
                if (checkNominationNameValid(nominationName))
                {
                    if (!checkNominationAlreadyExists(nominationName))
                    {
                        nominationColumn.Header = nominationName;
                        nominationColumn.CanUserResize = false;
                        nominationColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                        Binding bind = new Binding("Nominations[" + nominationName + "]");
                        bind.Mode = BindingMode.TwoWay;

                        var cellStyle = new Style(typeof(DataGridCell));
                        cellStyle.Setters.Add(new Setter()
                        {
                            Property = BackgroundProperty,
                            Value = (Brush)new BrushConverter().ConvertFrom("#F5F1DA")
                        });
                        nominationColumn.CellStyle = cellStyle;

                        FrameworkElementFactory checkBox = new FrameworkElementFactory(typeof(CheckBox));
                        checkBox.SetBinding(CheckBox.IsCheckedProperty, bind);
                        checkBox.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                        checkBox.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                        DataTemplate checkBoxTemplate = new DataTemplate();
                        checkBoxTemplate.VisualTree = checkBox;

                        nominationColumn.CellTemplate = checkBoxTemplate;

                        foreach (ParticipantFormModel p in participants)
                        {
                            p.Nominations.Add(nominationName, false);
                        }

                        mainNominationsColumns.Add(nominationColumn);
                        mainWindowColumns.Add(nominationColumn);
                        MainWindow.GetReagistrator.nominationsNames.Add(nominationName);
                    }

                }
                else
                {
                    MessageBox.Show("Недопустимое название номинации " + nominationName +
                        "!\nТакое название уже имеет существующий столбец, номинация " + nominationName +
                        " не будет добавлена в таблицу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // удаляем номинации из таблицы регистрации
            for (int i = 0; i < nominationsForDelete.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    string nominationName = nominationsForDelete[i].Nomination.Name;
                    // находим удаляемую колонку и удаляем из таблицы
                    if (nominationName.Equals(mainNominationsColumns[j].Header))
                    {
                        mainWindowColumns.Remove(mainNominationsColumns[j]);
                        // удаляем номинации у участников
                        foreach (ParticipantFormModel p in participants)
                        {
                            p.Nominations.Remove(nominationName);
                        }
                        MainWindow.GetReagistrator.nominationsNames.Remove(nominationName);
                    }
                }
            }

            for (int i = 0; i < nominationsForDelete.Count; i++)
            {
                for (int j = 0; j < mainNominationsColumns.Count; j++)
                {
                    if (nominationsForDelete[i].Nomination.Name.Equals(mainNominationsColumns[j].Header))
                    {
                        mainNominationsColumns.Remove(mainNominationsColumns[j]);
                    }
                }
            }

            nominationsList.Clear();
            nominationsForDelete.Clear();

            this.Close();
        }

        /// <summary>
        /// Проверка что такой номинации еще не существует
        /// </summary>
        /// <param name="nominationName"></param>
        /// <returns></returns>
        private bool checkNominationAlreadyExists(string nominationName)
        {
            for (int i = 0; i < mainNominationsColumns.Count; i++)
            {
                if (mainNominationsColumns[i].Header.Equals(nominationName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка что название номинации не совпадает с названием обязательных столбцов
        /// </summary>
        /// <param name="nominationName"></param>
        /// <returns></returns>
        private bool checkNominationNameValid(string nominationName)
        {
            if (nominationName.Equals("Имя") ||
                nominationName.Equals("Фамилия") ||
                nominationName.Equals("Отчество") ||
                nominationName.Equals("Посевной") ||
                nominationName.Equals("Пол") ||
                nominationName.Equals("Год рождения") ||
                nominationName.Equals("Клуб") ||
                nominationName.Equals("Город") ||
                nominationName.Equals("Рост") ||
                nominationName.Equals("Вес") ||
                nominationName.Equals("Рейтинг (общий)") ||
                nominationName.Equals("Рейтинг (клубный)") ||
                nominationName.Equals("Псевдоним") ||
                nominationName.Equals("Категория") ||
                nominationName.Equals("")
                )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Добавление новой номинации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNomination(object sender, RoutedEventArgs e)
        {
            NominationFormModel nomination = new NominationFormModel()
            {
                IsSelected = false,
            };

            nominationsList.Add(nomination);
            nominationsGrid.ItemsSource = nominationsList;
        }

        /// <summary>
        /// Удаление выбранных номинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSelectedNominations(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominationsList.Count;)
            {
                if (nominationsList[i].IsSelected)
                {
                    nominationsForDelete.Add(nominationsList[i]);
                    nominationsList.Remove(nominationsList[i]);
                }
                else
                {
                    i++;
                }
            }
            nominationsGrid.ItemsSource = nominationsList;
        }

        /// <summary>
        /// Выбираем все номинации на удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllForDelete_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominationsList.Count; i++)
            {
                nominationsList[i].IsSelected = true;
            }
        }

        /// <summary>
        /// Убираем все чеки у номинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllForDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nominationsList.Count; i++)
            {
                nominationsList[i].IsSelected = false;
            }
        }

        private void settingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // загружаем чекбоксы для скрытия столбцов
            for (int i = 1; i < mainWindowColumns.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.IsChecked = false;
                checkBox.Content = mainWindowColumns[i].Header;
                if (mainWindowColumns[i].Visibility == Visibility.Visible)
                {
                    checkBox.IsChecked = true;
                }
                checkBoxes.Add(checkBox);
                columnsNames.Items.Add(checkBox);
            }

            // загружаем столбцы номинаций
            for (int i = 0; i < mainNominationsColumns.Count; i++)
            {
                Nomination nomination = new Nomination()
                {
                    Name = mainNominationsColumns[i].Header.ToString(),
                };

                NominationFormModel nominationFormModel = new NominationFormModel()
                {
                    IsSelected = false,
                    Nomination = nomination,
                };

                nominationsList.Add(nominationFormModel);
                nominationsGrid.ItemsSource = nominationsList;
            }
        }

        /// <summary>
        /// Чекбокс у номинации установлен
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nominationSelected(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Чекбокс у номинации убран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nominationUnselected(object sender, RoutedEventArgs e)
        {

        }
    }

    public class NominationFormModel : INotifyPropertyChanged
    {
        private Nomination _nomination = new Nomination()
        {
            Name = "",
            Id = 0,
            //ParticipantId = 0,
        };
        private bool _isSelected;

        public Nomination Nomination
        {
            get { return _nomination; }
            set { _nomination = value; OnPropertyChanged("Nomination"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
