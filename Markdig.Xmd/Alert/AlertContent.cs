namespace Markdig.Extensions.Xmd.Alert
{
    public class AlertContent
    {
        public AlertContent() { }
        public AlertContent(string content) : base()
        {
            Content = content;
        }

        public string Content { get; internal set; }
        public AlertContent Child { get; internal set; }
    }
}