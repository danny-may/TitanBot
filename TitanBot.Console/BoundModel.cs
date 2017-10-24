namespace TitanBot.Console
{
    internal class BoundModel : Core.Models.BoundModel
    {
        private string _name;
        private int _server;
        private ChildObj _child;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public int Server
        {
            get => _server;
            set => Set(ref _server, value);
        }

        public ChildObj Child
        {
            get => _child;
            set => Set(ref _child, value);
        }
    }

    internal class ChildObj : Core.Models.BoundModel
    {
        private string _name;
        private int _server;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public int Server
        {
            get => _server;
            set => Set(ref _server, value);
        }
    }
}