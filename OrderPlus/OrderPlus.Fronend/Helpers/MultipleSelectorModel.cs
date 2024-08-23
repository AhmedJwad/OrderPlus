namespace OrderPlus.Fronend.Helpers
{
    public class MultipleSelectorModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public MultipleSelectorModel(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
