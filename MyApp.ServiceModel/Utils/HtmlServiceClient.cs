using System;
using System.IO;
using ServiceStack;
using ServiceStack.Web;

namespace MyApp.ServiceModel.Utils
{
    /// <summary>
    /// 通用的 web 请求客户端
    /// </summary>
    public class HtmlServiceClient : ServiceClientBase
    {
        public override string Format => "html";

        public override string ContentType => MimeTypes.Html;

        public HtmlServiceClient()
        {
            DefaultUserAgent =
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.81 Safari/537.36";
        }

        public HtmlServiceClient(string baseUri)
            // Can't call SetBaseUri as that appends the format specific suffixes.
            : base(baseUri, baseUri)
        {
        }

        public override string Accept => MimeTypes.Html;

        public override void SerializeToStream(IRequest requestContext, object request, Stream stream)
        {
            var queryString = QueryStringSerializer.SerializeToString(request);
            stream.Write(queryString);
        }

        public override T DeserializeFromStream<T>(Stream stream)
        {
            return (T) DeserializeDtoFromHtml(typeof(T), stream);
        }

        public override StreamDeserializerDelegate StreamDeserializer => DeserializeDtoFromHtml;

        private object DeserializeDtoFromHtml(Type type, Stream fromStream)
        {
            // TODO: No tests currently use the response, but this could be something that will come in handy later.
            // It isn't trivial though, will have to parse the HTML content.
            return Activator.CreateInstance(type);
        }
    }
}