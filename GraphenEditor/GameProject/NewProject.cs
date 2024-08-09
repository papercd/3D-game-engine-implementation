using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphenEditor.GameProject
{
    public class ProjectTemplate
    {
        public string ProjectType { get; set; }

        public string ProjectFile { get; set; }




        class NewProject : ViewModelBase
    { 
        private string _name = "NewProject";
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        private string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\GraphenProjects\";
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

    }
    
}
