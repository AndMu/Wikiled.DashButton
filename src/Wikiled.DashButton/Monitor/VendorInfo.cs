namespace Wikiled.DashButton.Monitor
{
    public class VendorInfo
    {
        /// <summary>
        /// The MAC Address identifer
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The long name of the vendor / organization associated with the identifer mask
        /// </summary>
        public string Organization { get; }

        public VendorInfo(string identifier, string organization)
        {
            Identifier = identifier;
            Organization = organization;
        }

        public override string ToString()
        {
            return $"[MacVendorInfo: IdentiferString={Identifier}, Organization={Organization}]";
        }
    }
}
