﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.Core.Models;
using Microsoft.Health.Fhir.Tests.Common;
using Microsoft.Health.Fhir.Tests.Common.FixtureParameters;
using Microsoft.Health.Fhir.Tests.E2E.Common;
using Xunit;

namespace Microsoft.Health.Fhir.Tests.E2E.Rest
{
    /// <summary>
    /// NOTE: These tests will fail if security is disabled..
    /// </summary>
    [Trait(Traits.Category, Categories.Authorization)]
    [HttpIntegrationFixtureArgumentSets(DataStore.CosmosDb, Format.Json, FhirVersion.All)]
    public class BasicAuthTests : IClassFixture<HttpIntegrationTestFixture>
    {
        private const string ForbiddenMessage = "Forbidden: Authorization failed.";
        private const string UnauthorizedMessage = "Unauthorized: Authentication failed.";
        private const string Invalidtoken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImNmNWRmMGExNzY5ZWIzZTFkOGRiNWIxMGZiOWY3ZTk0IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NDQ2ODQ1NzEsImV4cCI6MTU0NDY4ODE3MSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNDgiLCJhdWQiOlsiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNDgvcmVzb3VyY2VzIiwiZmhpci1haSJdLCJjbGllbnRfaWQiOiJzZXJ2aWNlY2xpZW50Iiwicm9sZXMiOiJhZG1pbiIsImFwcGlkIjoic2VydmljZWNsaWVudCIsInNjb3BlIjpbImZoaXItYWkiXX0.SKSvy6Jxzwsv1ZSi0PO4Pdq6QDZ6mBJIRxUPgoPlz2JpiB6GMXu5u0n1IpS6zOXihGkGhegjtcqj-6TKE6Ou5uhQ0VTnmf-NxcYKFl48aDihcGem--qa2V8GC7na549Ctj1PLXoYUbovV4LB27Kj3X83sZVnWdHqg_G0AKo4xm7hr23VUvJ1D73lEcYaGd5K9GXHNgUrJO5v288y0uCXZ5ByNDJ-K6Xi7_68dLdshlIiHaeIBuC3rhchSf2hdglkQgOyo4g4gT_HfKjwdrrpGzepNXOPQEwtUs_o2uriXAd7FfbL_Q4ORiDWPXkmwBXqo7uUfg-2SnT3DApc3PuA0";
        private readonly Dictionary<string, string> _exportQueryParams = new Dictionary<string, string>()
            {
                { "_destinationType", "AzureBlockBlob" },
                { "_destinationConnectionSettings", "connectionString" },
            };

        public BasicAuthTests(HttpIntegrationTestFixture fixture)
        {
            Client = fixture.FhirClient;
        }

        protected ICustomFhirClient Client { get; set; }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenGettingAResource_GivenAUserWithNoReadPermissions_TheServerShouldReturnForbidden()
        {
            await Client.RunAsClientApplication(TestApplications.ServiceClient);
            FhirResponse<ResourceElement> createdResponse = await Client.CreateAsync(Client.GetDefaultObservation());
            var createdResource = createdResponse.Resource;

            await Client.RunAsUser(TestUsers.WriteOnlyUser, TestApplications.NativeClient);
            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.ReadAsync(KnownResourceTypes.Observation, createdResource.Id));
            Assert.Equal(ForbiddenMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Forbidden, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenCreatingAResource_GivenAUserWithNoCreatePermissions_TheServerShouldReturnForbidden()
        {
            await Client.RunAsUser(TestUsers.ReadOnlyUser, TestApplications.NativeClient);
            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.CreateAsync(Client.GetDefaultObservation()));
            Assert.Equal(ForbiddenMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Forbidden, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenUpdatingAResource_GivenAUserWithNoWritePermissions_TheServerShouldReturnForbidden()
        {
            await Client.RunAsUser(TestUsers.WriteOnlyUser, TestApplications.NativeClient);
            FhirResponse<ResourceElement> createdResponse = await Client.CreateAsync(Client.GetDefaultObservation());
            var createdResource = createdResponse.Resource;

            await Client.RunAsUser(TestUsers.ReadOnlyUser, TestApplications.NativeClient);
            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.UpdateAsync(createdResource));
            Assert.Equal(ForbiddenMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Forbidden, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenHardDeletingAResource_GivenAUserWithNoHardDeletePermissions_TheServerShouldReturnForbidden()
        {
            await Client.RunAsUser(TestUsers.WriteOnlyUser, TestApplications.NativeClient);
            FhirResponse<ResourceElement> createdResponse = await Client.CreateAsync(Client.GetDefaultObservation());
            var createdResource = createdResponse.Resource;

            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.HardDeleteAsync(createdResource));
            Assert.Equal(ForbiddenMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Forbidden, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenHardDeletingAResource_GivenAUserWithHardDeletePermissions_TheServerShouldReturnSuccess()
        {
            await Client.RunAsUser(TestUsers.WriteOnlyUser, TestApplications.NativeClient);
            FhirResponse<ResourceElement> createdResponse = await Client.CreateAsync(Client.GetDefaultObservation());
            var createdResource = createdResponse.Resource;

            await Client.RunAsUser(TestUsers.HardDeleteUser, TestApplications.NativeClient);

            // Hard-delete the resource.
            await Client.HardDeleteAsync(createdResource);

            await Client.RunAsUser(TestUsers.ReadOnlyUser, TestApplications.NativeClient);

            // Getting the resource should result in NotFound.
            await ExecuteAndValidateNotFoundStatus(() => Client.ReadAsync(KnownResourceTypes.Observation, createdResource.Id));

            async Task<FhirException> ExecuteAndValidateNotFoundStatus(Func<Task> action)
            {
                FhirException exception = await Assert.ThrowsAsync<FhirException>(action);
                Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
                return exception;
            }
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenUpdatingAResource_GivenAUserWithUpdatePermissions_TheServerShouldReturnSuccess()
        {
            await Client.RunAsUser(TestUsers.AdminUser, TestApplications.NativeClient);
            FhirResponse<ResourceElement> createdResponse = await Client.CreateAsync(Client.GetDefaultObservation());
            var createdResource = createdResponse.Resource;

            await Client.RunAsUser(TestUsers.ReadWriteUser, TestApplications.NativeClient);
            FhirResponse<ResourceElement> updateResponse = await Client.UpdateAsync(createdResource);

            Assert.Equal(System.Net.HttpStatusCode.OK, updateResponse.StatusCode);

            ResourceElement updatedResource = updateResponse.Resource;

            Assert.NotNull(updatedResource);
            Assert.Equal(createdResource.Id, updatedResource.Id);
            Assert.NotEqual(createdResource.VersionId, updatedResource.VersionId);
            Assert.NotEqual(createdResource.LastUpdated, updatedResource.LastUpdated);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenCreatingAResource_GivenAClientWithNoAuthToken_TheServerShouldReturnUnauthorized()
        {
            await Client.RunAsClientApplication(TestApplications.InvalidClient);

            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.CreateAsync(Client.GetDefaultObservation()));
            Assert.Equal(UnauthorizedMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Unauthorized, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenCreatingAResource_GivenAClientWithInvalidAuthToken_TheServerShouldReturnUnauthorized()
        {
            await Client.RunAsClientApplication(TestApplications.InvalidClient);
            Client.HttpClient.SetBearerToken(Invalidtoken);
            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.CreateAsync(Client.GetDefaultObservation()));
            Assert.Equal(UnauthorizedMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Unauthorized, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenCreatingAResource_GivenAClientWithWrongAudience_TheServerShouldReturnUnauthorized()
        {
            await Client.RunAsClientApplication(TestApplications.WrongAudienceClient);
            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.CreateAsync(Client.GetDefaultObservation()));
            Assert.Equal(UnauthorizedMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Unauthorized, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenGettingAResource_GivenAUserWithReadPermissions_TheServerShouldReturnSuccess()
        {
            await Client.RunAsClientApplication(TestApplications.ServiceClient);
            FhirResponse<ResourceElement> createdResponse = await Client.CreateAsync(Client.GetDefaultObservation());
            var createdResource = createdResponse.Resource;

            await Client.RunAsUser(TestUsers.ReadOnlyUser, TestApplications.NativeClient);
            FhirResponse<ResourceElement> readResponse = await Client.ReadAsync(KnownResourceTypes.Observation, createdResource.Id);

            ResourceElement readResource = readResponse.Resource;

            Assert.Equal(createdResource.Id, readResource.Id);
            Assert.Equal(createdResource.VersionId, readResource.VersionId);
            Assert.Equal(createdResource.LastUpdated, readResource.LastUpdated);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenExportResources_GivenAUserWithNoExportPermissions_TheServerShouldReturnForbidden()
        {
            await Client.RunAsUser(TestUsers.ReadOnlyUser, TestApplications.NativeClient);

            FhirException fhirException = await Assert.ThrowsAsync<FhirException>(async () => await Client.ExportAsync(_exportQueryParams));
            Assert.Equal(ForbiddenMessage, fhirException.Message);
            Assert.Equal(HttpStatusCode.Forbidden, fhirException.StatusCode);
        }

        [Fact]
        [Trait(Traits.Priority, Priority.One)]
        public async Task WhenExportResources_GivenAUserWithExportPermissions_TheServerShouldReturnSuccess()
        {
            await Client.RunAsUser(TestUsers.ExportUser, TestApplications.NativeClient);

            await Client.ExportAsync(_exportQueryParams);
        }
    }
}
