﻿using System;
using MyCouch.Testing;

namespace MyCouch.IntegrationTests
{
    public abstract class ClientTestsOf<T> :
        TestsOf<T>,
        IDisposable where T : class
    {
        protected readonly TestEnvironment Environment;
        protected IMyCouchClient Client { get; set; }

        protected ClientTestsOf() : this(IntegrationTestsRuntime.NormalEnvironment) { }

        protected ClientTestsOf(TestEnvironment environment)
        {
            Environment = environment;
            Client = IntegrationTestsRuntime.CreateDbClient(Environment);
            CleanDb();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposing)
                return;

            CleanDb();
            Client.Dispose();
            Client = null;

            var disposableSut = SUT as IDisposable;
            if(disposableSut == null)
                return;

            disposableSut.Dispose();
        }

        protected void CleanDb()
        {
            if (!(this is IPreserveStatePerFixture))
                Client.ClearAllDocuments();
        }
    }
}