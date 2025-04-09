namespace APBD5
{
    /// <summary>
    /// Class that represents the device, using Open/Closed Principle.
    /// </summary>
    public abstract class Device
    {
        private string _id;
        private string _name;
        private bool _isEnabled;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        protected Device(string id, string name, bool isEnabled)
        {
            _id = id;
            _name = name;
            _isEnabled = isEnabled;
        }

        public virtual void TurnOn()
        {
            IsEnabled = true;
        }

        public virtual void TurnOff()
        {
            IsEnabled = false;
        } 

        /// <summary>
        /// Function to save to file later.
        /// </summary>
        public abstract string ToSavingString();
    }
}