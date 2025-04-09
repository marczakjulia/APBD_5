namespace APBD5
{
    /// <summary>
    /// Interface for checking IDs of devices.
    /// </summary>
    public interface IIdValidator
    {
        bool IsValid(string id);

        /// <summary>
        /// Checks the ids and their validity.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Is thrown when an invalid id is found.
        /// </exception>
        void ValidateOrThrow(string id);
    }

    /// <summary>
    /// Checks if ID starts with P-.
    /// </summary>
    public class PersonalComputerIdValidator : IIdValidator
    {
        public bool IsValid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return id.StartsWith("P-");
        }

        public void ValidateOrThrow(string id)
        {
            if (!IsValid(id))
            {
                throw new ArgumentException("Invalid ID value. Required format: P-<something>", nameof(id));
            }
        }
    }

    /// <summary>
    /// Checks if ID starts with ED-.
    /// </summary>
    public class EmbeddedDeviceIdValidator : IIdValidator
    {
        public bool IsValid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return id.StartsWith("ED-");
        }

        public void ValidateOrThrow(string id)
        {
            if (!IsValid(id))
            {
                throw new ArgumentException("Invalid ID. Required format: ED-<something>", nameof(id));
            }
        }
    }

    /// <summary>
    /// Checks if ID starts with SW-.
    /// </summary>
    public class SmartWatchIdValidator : IIdValidator
    {
        public bool IsValid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return id.StartsWith("SW-");
        }

        public void ValidateOrThrow(string id)
        {
            if (!IsValid(id))
            {
                throw new ArgumentException("Invalid ID. Required format: SW-<something>", nameof(id));
            }
        }
    }
}
