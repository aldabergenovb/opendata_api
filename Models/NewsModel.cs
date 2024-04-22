using System.Xml.Serialization;

namespace opendata_api.Models;

public class NewsModel
{
    [XmlRoot("rss")]
    public class RssFeed
    {
        [XmlElement("channel")]
        public Channel Channel { get; set; }
    }

    public class Channel
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("language")]
        public string Language { get; set; }

        [XmlElement("item")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("enclosure")]
        public Enclosure Enclosure { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("guid")]
        public string newsUrl { get; set; }
    }

    public class Enclosure
    {
        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}