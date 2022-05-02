using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Simulator.ViewModels
{
    public class DoTaskVMFlyoutMenuItem
    {
        public DoTaskVMFlyoutMenuItem()
        {
            TargetType = typeof(DoTaskVMFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}