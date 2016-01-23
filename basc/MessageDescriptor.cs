namespace Basic
{
    public class MessageDescriptor
    {
        public MessageDescriptor()
        {
            Priority = 1;
        }

        public MessageType Type { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public int Priority { get; set; }

        public int NumberOfArguments { get; set; }

        public bool IsVisualizable { get; set; }
    }
}