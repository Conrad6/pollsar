using System;
using System.Collections.ObjectModel;

namespace Pollsar.Shared.Models
{
    public class PollChoiceViewModel : BaseViewModel
    {
        private readonly ObservableCollection<long> votes;
        private long id;
        private string name;
        private long? pollId;
        private DateTime? lastUpdated;

        public PollChoiceViewModel ()
        {
            votes = new ObservableCollection<long>();
            votes.CollectionChanged += Votes_CollectionChanged;
        }

        private void Votes_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Votes));
        }

        public long Id
        {
            get => id; set
            {
                var temp = id;
                id = value;
                if (id == temp) return;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name; set
            {
                var temp = name;
                name = value;
                if (name == temp) return;
                OnPropertyChanged();
            }
        }
        public long? PollId
        {
            get => pollId; set
            {
                var temp = pollId;
                pollId = value;
                if (pollId == temp) return;
                OnPropertyChanged();
            }
        }
        public virtual ObservableCollection<long> Votes => votes;
        public DateTime? LastUpdated
        {
            get => lastUpdated; set
            {
                var temp = lastUpdated;
                lastUpdated = value;
                if (lastUpdated == temp) return;
                OnPropertyChanged();
            }
        }
    }
}
