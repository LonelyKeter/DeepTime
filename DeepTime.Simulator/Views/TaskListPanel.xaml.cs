using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DeepTime.Simulator.ViewModels;

namespace DeepTime.Simulator.Views
{
    /// <summary>
    /// Логика взаимодействия для TaskListPanel.xaml
    /// </summary>
    public partial class TaskListPanel : UserControl
    {
        public TaskListPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => AddCommandBindinds();
        }

        private void AddCommandBindinds()
        {
            var taskVM = (TaskVM)DataContext;
            CommandBindings.AddRange(taskVM.CreateCommandBindings());
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }
}
