using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pollsar.Shared.Models
{
    public class PollViewModel : BaseViewModel
    {
        private long id;
        private string title;
        private string description;
        private long? creatorId;
        private DateTime? dateCreated;
        private DateTime? lastUpdate;
        private ObservableCollection<PollChoiceViewModel> choices;
        private ObservableCollection<string> tags;
        private ObservableCollection<string> categories;
        private ObservableCollection<string> images;

        public PollViewModel ()
        {

        }

        private void General_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string propertyName;
            if (sender == choices) propertyName = nameof(Choices);
            else if (sender == tags) propertyName = nameof(tags);
            else if (sender == categories) propertyName = nameof(Categories);
            else if (sender == images) propertyName = nameof(Images);
            else throw new KeyNotFoundException();

            OnPropertyChanged(propertyName);
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
        public string Title
        {
            get => title; set
            {
                var temp = title;
                title = value;
                if (title == temp) return;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get => description; set
            {
                var temp = description;
                description = value;
                if (description == temp) return;
                OnPropertyChanged();
            }
        }
        public long? CreatorId
        {
            get => creatorId; set
            {
                var temp = creatorId;
                creatorId = value;
                if (creatorId == temp) return;
                OnPropertyChanged();
            }
        }
        public DateTime? DateCreated
        {
            get => dateCreated; set
            {
                var temp = dateCreated;
                dateCreated = value;
                if (dateCreated == temp) return;
                OnPropertyChanged();
            }
        }
        public DateTime? LastUpdate
        {
            get => lastUpdate; set
            {
                var temp = lastUpdate;
                lastUpdate = value;
                if (lastUpdate == temp) return;
                OnPropertyChanged();
            }
        }
        public virtual ObservableCollection<PollChoiceViewModel> Choices
        {
            get => choices;
            set
            {
                choices = value;
                choices.CollectionChanged += General_CollectionChanged;
            }
        }
        public virtual ObservableCollection<string> Tags
        {
            get => tags;
            set
            {
                tags = value;
                choices.CollectionChanged += General_CollectionChanged;
            }
        }
        public virtual ObservableCollection<string> Categories
        {
            get => categories; set
            {
                categories = value;
                choices.CollectionChanged += General_CollectionChanged;
            }
        }
        public virtual ObservableCollection<string> Images
        {
            get => images; set
            {
                images = value;
                choices.CollectionChanged += General_CollectionChanged;
            }
        }
    }
}
