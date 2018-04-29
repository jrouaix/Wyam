﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Testing;
using Wyam.Testing.Execution;
using Wyam.Testing.Documents;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi;
using Shouldly;
using Microsoft.OpenApi.Interfaces;
using System;

namespace Wyam.OpenAPI.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self | ParallelScope.Children)]
    public class OpenAPIFixture : BaseFixture
    {
        public class ExecuteTests : OpenAPIFixture
        {
            #region OPEN_API_3_DOCUMENT
            private const string OPEN_API_3_DOCUMENT =

@"openapi: ""3.0.0""
info:
  version: 1.0.0
  title: Swagger Petstore
  description: A sample API that uses a petstore as an example to demonstrate features in the OpenAPI 3.0 specification
  termsOfService: http://swagger.io/terms/
  contact:
    name: Swagger API Team
    email: apiteam @swagger.io
    url: http://swagger.io
  license:
    name: Apache 2.0
    url: https://www.apache.org/licenses/LICENSE-2.0.html
servers:
  - url: http://petstore.swagger.io/api
paths:
  /pets:
    get:
      description: |
        Returns all pets from the system that the user has access to
        Nam sed condimentum est. Maecenas tempor sagittis sapien, nec rhoncus sem sagittis sit amet. Aenean at gravida augue, ac iaculis sem.Curabitur odio lorem, ornare eget elementum nec, cursus id lectus.Duis mi turpis, pulvinar ac eros ac, tincidunt varius justo.In hac habitasse platea dictumst.Integer at adipiscing ante, a sagittis ligula.Aenean pharetra tempor ante molestie imperdiet. Vivamus id aliquam diam. Cras quis velit non tortor eleifend sagittis.Praesent at enim pharetra urna volutpat venenatis eget eget mauris. In eleifend fermentum facilisis. Praesent enim enim, gravida ac sodales sed, placerat id erat.Suspendisse lacus dolor, consectetur non augue vel, vehicula interdum libero.Morbi euismod sagittis libero sed lacinia.

        Sed tempus felis lobortis leo pulvinar rutrum.Nam mattis velit nisl, eu condimentum ligula luctus nec.Phasellus semper velit eget aliquet faucibus. In a mattis elit. Phasellus vel urna viverra, condimentum lorem id, rhoncus nibh. Ut pellentesque posuere elementum. Sed a varius odio. Morbi rhoncus ligula libero, vel eleifend nunc tristique vitae.Fusce et sem dui. Aenean nec scelerisque tortor. Fusce malesuada accumsan magna vel tempus. Quisque mollis felis eu dolor tristique, sit amet auctor felis gravida.Sed libero lorem, molestie sed nisl in, accumsan tempor nisi.Fusce sollicitudin massa ut lacinia mattis. Sed vel eleifend lorem. Pellentesque vitae felis pretium, pulvinar elit eu, euismod sapien.
      operationId: findPets
      parameters:
        - name: tags
          in: query
          description: tags to filter by
          required: false
          style: form
          schema:
            type: array
            items:
              type: string
        - name: limit
          in: query
          description: maximum number of results to return
          required: false
          schema:
            type: integer
            format: int32
      responses:
        '200':
          description: pet response
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Pet'
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Error'
    post:
      description: Creates a new pet in the store.Duplicates are allowed
      operationId: addPet
      requestBody:
        description: Pet to add to the store
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/NewPet'
      responses:
        '200':
          description: pet response
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Pet'
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Error'
  /pets/{ id}:
    get:
      description: Returns a user based on a single ID, if the user does not have access to the pet
      operationId: find pet by id
      parameters:
        - name: id
          in: path
          description: ID of pet to fetch
          required: true
          schema:
            type: integer
            format: int64
      responses:
        '200':
          description: pet response
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Pet'
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Error'
    delete:
      description: deletes a single pet based on the ID supplied
      operationId: deletePet
      parameters:
        - name: id
          in: path
          description: ID of pet to delete
          required: true
          schema:
            type: integer
            format: int64
      responses:
        '204':
          description: pet deleted
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Error'
components:
  schemas:
    Pet:
      allOf:
        - $ref: '#/components/schemas/NewPet'
        - required:
          - id
          properties:
            id:
              type: integer
              format: int64

    NewPet:
      required:
        - name
      properties:
        name:
          type: string
        tag:
          type: string

    Error:
      required:
        - code
        - message
      properties:
        code:
          type: integer
          format: int32
        message:
          type: string

";
            #endregion

            [Test]
            public void SetsMetadataKey()
            {
                //// Given
                IExecutionContext context = new TestExecutionContext();
                IDocument document = new TestDocument(OPEN_API_3_DOCUMENT);
                var openAPI = new OpenAPI("myOpenApi");

                // When
                IList<IDocument> documents = openAPI.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                documents.Count.ShouldBe(1);
                documents[0].Keys.ShouldBe(new[] { "myOpenApi" }, true);
            }

            [Test]
            public void GeneratesOpenApiDocumentObject()
            {
                //// Given
                IExecutionContext context = new TestExecutionContext();
                IDocument document = new TestDocument(OPEN_API_3_DOCUMENT);
                var module = new OpenAPI();

                // When
                IList<IDocument> documents = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                documents.Count.ShouldBe(1);
                documents[0].Keys.ShouldBe(new[] { OpenAPI.OpenAPI_DEFAULT_KEY }, true);
                documents[0][OpenAPI.OpenAPI_DEFAULT_KEY].ShouldBeOfType<OpenApiDocument>();
                var api = (OpenApiDocument)documents[0][OpenAPI.OpenAPI_DEFAULT_KEY];
                api.Paths.Count.ShouldBe(2);
            }

            [Test]
            public void ReturnsDocumentIfEmptyInput()
            {
                //// Given
                IExecutionContext context = new TestExecutionContext();
                IDocument document = new TestDocument(Environment.NewLine);
                var openAPI = new OpenAPI();

                // When
                IList<IDocument> documents = openAPI.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

                // Then
                documents.Count.ShouldBe(1);
                documents[0].Keys.ShouldBeEmpty();
            }
        }
    }
}