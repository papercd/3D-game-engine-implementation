using GraphenEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GraphenEditor.GameProject
{

    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {

        public static string Extension { get; } = ".graphen";

        [DataMember]
        public string Name { get; private set; } = "New Project";

        [DataMember]
        public string Path { get; private set; }

        public string Fullpath => $"{Path}{Name}{Extension}";



        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene> ();
        public ReadOnlyObservableCollection<Scene> Scenes
        { get; private set; }

        private Scene _activeScene;

        
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        

        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();
        
        public ICommand Undo {  get; set; } 
        public ICommand Redo {  get; set; } 

        public ICommand AddScene {  get; set; } 

        public ICommand RemoveScene { get; set; }   


        private void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveSceneInteral(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public void Unload()
        {

        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.Fullpath);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);


            AddScene = new RelayCommand<object>(x =>
            {
                AddSceneInternal($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveSceneInteral(newScene),
                    () => _scenes.Insert(sceneIndex,newScene),
                    $"Add {newScene.Name}"));

            });

            RemoveScene = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveSceneInteral(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveSceneInteral(x),
                    $"Remove {x.Name}"));
            }, x=> !x.IsActive);
            Undo = new RelayCommand<object>(x => UndoRedo.Undo());
            Redo = new RelayCommand<object>(x => UndoRedo.Redo());



        }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;


            OnDeserialized(new StreamingContext());
        }

    }
}
