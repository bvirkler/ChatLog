
namespace ChatLog
{
    class Chat
    {
        protected string _displayName;
        protected string _path;

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public bool IsMultiway { get; set; }

        public Chat(string displayName, string path)
            : this(displayName, path, false)
        {

        }

        public Chat(string displayName, string path, bool multiway)
        {
            _displayName = displayName;
            _path = path;
            IsMultiway = multiway;
        }
    }
}
