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
    /// Логика взаимодействия для TaskPanel.xaml
    /// </summary>
    public partial class TaskPanel : UserControl
    {
        public TaskPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => AddCommandBindinds();
        }

        private void AddCommandBindinds()
        {
            var taskVM = (TaskVM)DataContext;
            CommandBindings.AddRange(taskVM.CreateCommandBindings());
        }
    }
}
