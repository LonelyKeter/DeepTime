using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Simulator.ViewModels
{
    public class TaskGeneratorVM
    {
        private TaskGenParametersVM[] _parameters = { new(), new(), new(), new(), new() };

        public TaskGenParametersVM VeryHighPriority => _parameters[4];
        public TaskGenParametersVM HighPriority => _parameters[3];
        public TaskGenParametersVM MediumPriority => _parameters[2];
        public TaskGenParametersVM LowPriority => _parameters[1];
        public TaskGenParametersVM VeryLowPriority => _parameters[0];

        private TaskGenParametersVM[] Params => new[] { 
            VeryHighPriority, HighPriority, MediumPriority, LowPriority, VeryLowPriority };
    }
}
