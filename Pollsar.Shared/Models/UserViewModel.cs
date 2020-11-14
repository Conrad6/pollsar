using System;
using System.Collections.ObjectModel;

namespace Pollsar.Shared.Models
{
    public class UserViewModel : BaseViewModel
    {
        private long id;

        public long Id
        {
            get => id;
            set {
                var temp = id;
                id = value;
                if (value == temp) return;
                OnPropertyChanged();
            }
        }

        private string avatar;

        public string Avatar
        {
            get => avatar;
            set {
                var temp = avatar;
                avatar = value;
                if (value == temp) return;
                OnPropertyChanged();
            }
        }
        private DateTime? dateAdded;

        public DateTime? DateAdded
        {
            get => dateAdded;
            set {
                var temp = dateAdded;
                dateAdded = value;
                if (value == temp) return;
                OnPropertyChanged();
            }
        }
        private DateTime? lastUpdated;

        public DateTime? LastUpdated
        {
            get => lastUpdated;
            set {
                var temp = lastUpdated;
                lastUpdated = value;
                if (value == temp) return;
                OnPropertyChanged();
            }
        }
        private readonly ObservableCollection<PollViewModel> pollsCreated;

        public ObservableCollection<PollViewModel> PollsCreated => pollsCreated;

        public UserViewModel ()
        {
            pollsCreated = new ObservableCollection<PollViewModel>();
            pollsCreated.CollectionChanged += PollsCreated_CollectionChanged;
        }

        private void PollsCreated_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PollsCreated));
        }
    }
}
