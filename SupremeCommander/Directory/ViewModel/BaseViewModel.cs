using PropertyChanged;
using System.ComponentModel;

namespace SupremeCommander
{
    /// <summary>
    /// A base class that fires property changed events as needed
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that is fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
