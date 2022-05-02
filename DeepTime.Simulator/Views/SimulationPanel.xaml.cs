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

namespace DeepTime.Simulator.Views
{
    /// <summary>
    /// Логика взаимодействия для SimulationPanel.xaml
    /// </summary>
    public partial class SimulationPanel : UserControl
    {
        public SimulationPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => AddCommandBindinds();
        }

        private void AddCommandBindinds()
        {
            var taskVM = (ViewModels.SimulationVM)DataContext;
            CommandBindings.AddRange(taskVM.CreateCommandBindings());
        }
    }
}
