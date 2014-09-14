﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using Ficksworkshop.TimeTrackerAPI;

namespace Ficksworkshop.TimeTracker
{
    public class NotificationIconViewModel : ViewModelBase
    {
        #region Fields

        private SelectedProjectManager _selectedProjectManager;

        #endregion

        #region Properties

        private bool _isPunchedIn;

        public bool IsPunchedIn
        {
            get
            {
                return _isPunchedIn;
            }
            private set
            {
                if (value != _isPunchedIn)
                {
                    _isPunchedIn = value;
                    NotifyPropertyChanged("IsPunchedIn");
                }
            }
        }

        private ObservableCollection<IProject> _activeProjects = new ObservableCollection<IProject>();

        /// <summary>
        /// The list of projects that we can punch in or punch out of.
        /// </summary>
        public ObservableCollection<IProject> ActiveProjects
        {
            get
            {
                return _activeProjects;
            }
            set
            {
                if (_activeProjects != value)
                {
                    _activeProjects = value;
                    NotifyPropertyChanged("ActiveProjectsCollection");
                }
            }
        }

        private IProject _selectedProject;

        /// <summary>
        /// Gets or sets the selected project to quiickly punch in or out of a project.
        /// </summary>
        public IProject SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                // First try to change in the selected project manager, since the change might be rejected
                if (value != _selectedProject && _selectedProjectManager.SetSelectedProject(value))
                {
                    // Ok, it was changed, so actually update the property
                    _selectedProject = value;
                    NotifyPropertyChanged("SelectedProject");
                }
            }
        }

        #endregion

        #region Constructors

        public NotificationIconViewModel(SelectedProjectManager selectedProjectManager)
        {
            // When we are constructed, we need to listen to events coming from the data
            // set so that we can update our local view.
            TrackerInstance.DataSetChangedEvent += DataContextChangedEventHandler;
            if (TrackerInstance.DataSet != null)
            {
                TrackerInstance.DataSet.ProjectsChanged += ProjectsChangedEventHandler;
                TrackerInstance.DataSet.ProjectTimeChanged += ProjectTimeChangedEventHandler;
            }

            _selectedProjectManager = selectedProjectManager;
            _selectedProjectManager.SelectedProjectChanged += SelectedProjectChangedEventHandler;
        }

        #endregion

        #region Private Members

        private void ProjectsChangedEventHandler(object sender, object e)
        {
            // Refresh the list of active projects
            ActiveProjects.Clear();
            foreach (var project in TrackerInstance.DataSet.Projects)
            {
                if (project.Status != ProjectStatus.Closed)
                {
                    ActiveProjects.Add(project);
                }
            }
        }

        private void ProjectTimeChangedEventHandler(object sender, TimesChangedEventArgs eventArgs)
        {
            // Refresh the is punched in value
            IsPunchedIn = (eventArgs.DataSet.FirstOpenTime() != null);
        }

        private void DataContextChangedEventHandler(IProjectTimesData oldDataSet, IProjectTimesData newDataSet)
        {
            // If we completely change the data set, then unsubscribe from the old data set, subscribe to the new
            // one, and update our projects
            if (oldDataSet != null)
            {
                oldDataSet.ProjectsChanged -= ProjectsChangedEventHandler;
                oldDataSet.ProjectTimeChanged -= ProjectTimeChangedEventHandler;
            }
            if (newDataSet != null)
            {
                newDataSet.ProjectsChanged += ProjectsChangedEventHandler;
                newDataSet.ProjectTimeChanged += ProjectTimeChangedEventHandler;
            }

            ProjectsChangedEventHandler(null, null);
        }

        /// <summary>
        /// Handler when the selected project changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SelectedProjectChangedEventHandler(object sender, SelectedProjectChangedEventArgs eventArgs)
        {
            SelectedProject = eventArgs.NewProject;
        }

        #endregion
    }
}