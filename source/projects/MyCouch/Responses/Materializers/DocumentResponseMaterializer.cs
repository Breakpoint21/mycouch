﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EnsureThat;
using MyCouch.Extensions;
using MyCouch.Serialization;
using Newtonsoft.Json.Linq;

namespace MyCouch.Responses.Materializers
{
    public class DocumentResponseMaterializer
    {
        protected readonly ISerializer Serializer;

        public DocumentResponseMaterializer(ISerializer serializer)
        {
            Ensure.That(serializer, "serializer").IsNotNull();

            Serializer = serializer;
        }

        public virtual async Task MaterializeAsync(DocumentResponse response, HttpResponseMessage httpResponse)
        {
            if (response.RequestMethod != HttpMethod.Get)
                throw new ArgumentException(GetType().Name + " only supports materializing GET responses for raw documents.");

            using (var content = await httpResponse.Content.ReadAsStreamAsync().ForAwait())
            {
                response.Content = content.ReadAsString();

                var jt = JObject.Parse(response.Content);

                response.Id = jt.Value<string>(JsonScheme._Id);
                response.Rev = jt.Value<string>(JsonScheme._Rev);
                response.Conflicts = jt.Values<string>(JsonScheme.Conflicts)?.ToArray();

                SetMissingIdFromRequestUri(response, httpResponse.RequestMessage);
                SetMissingRevFromResponseHeaders(response, httpResponse.Headers);
            }
        }

        protected virtual void SetMissingIdFromRequestUri(DocumentResponse response, HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(response.Id) && request.Method != HttpMethod.Post)
                response.Id = request.ExtractIdFromUri(false);
        }

        protected virtual void SetMissingRevFromResponseHeaders(DocumentResponse response, HttpResponseHeaders responseHeaders)
        {
            if (string.IsNullOrWhiteSpace(response.Rev))
                response.Rev = responseHeaders.GetETag();
        }
    }
}