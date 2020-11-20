using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Pollsar.Shared.Models
{
    public class UserViewModel : BaseViewModel
    {
        private string names;
        [Display(Name = "Names")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is requried")]
        public string Names
        {
            get => names;
            set {
                var temp = names;
                names = value;
                if (value == temp) return;
                OnPropertyChanged();
            }
        }

        private string email;

        public string Email
        {
            get => email;

            set
            {
                var temp = email;
                email = value;
                if (temp == value) return;

                OnPropertyChanged();
            }
        }


        private long id;
        [Editable(false)]
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
        [Url(ErrorMessage = "{0} is an invalid url")]
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
        [Editable(false)]
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
        [Editable(false)]
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
        private ObservableCollection<PollViewModel> pollsCreated;
        [Editable(false)]
        public ObservableCollection<PollViewModel> PollsCreated
        {
            get => pollsCreated;
            set
            {
                pollsCreated = value;
                pollsCreated.CollectionChanged += PollsCreated_CollectionChanged;
            }
        }

        private void PollsCreated_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PollsCreated));
        }
    }
}
