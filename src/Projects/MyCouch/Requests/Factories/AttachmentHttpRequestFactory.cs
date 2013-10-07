﻿using MyCouch.Net;
using MyCouch.Requests;
using MyCouch.Requests.Factories;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MyCouch.Contexts
{
    public class AttachmentHttpRequestFactory : 
        HttpRequestFactoryBase,
        IHttpRequestFactory<GetAttachmentRequest>,
        IHttpRequestFactory<PutAttachmentRequest>,
        IHttpRequestFactory<DeleteAttachmentRequest>
    {
        public AttachmentHttpRequestFactory(IConnection connection) : base(connection) { }

        protected virtual string GenerateRequestUrl(string docId, string docRev, string attachmentName)
        {
            return string.Format("{0}/{1}/{2}{3}",
                Connection.Address,
                docId,
                attachmentName,
                docRev == null ? string.Empty : string.Concat("?rev=", docRev));
        }

        public virtual HttpRequest Create(GetAttachmentRequest request)
        {
            var httpRequest = new HttpRequest(HttpMethod.Get, GenerateRequestUrl(request.DocId, request.DocRev, request.Name));

            httpRequest.SetIfMatch(request.DocRev);

            return httpRequest;
        }

        public virtual HttpRequest Create(PutAttachmentRequest request)
        {
            var httpRequest = new HttpRequest(HttpMethod.Put, GenerateRequestUrl(request.DocId, request.DocRev, request.Name));

            httpRequest.SetIfMatch(request.DocRev);
            httpRequest.SetContent(request.Content, request.ContentType);

            return httpRequest;
        }

        public virtual HttpRequest Create(DeleteAttachmentRequest request)
        {
            var httpRequest = new HttpRequest(HttpMethod.Delete, GenerateRequestUrl(request.DocId, request.DocRev, request.Name));

            httpRequest.SetIfMatch(request.DocRev);

            return httpRequest;
        }
    }
}
