using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate.Appenders;
// ReSharper disable Unity.IncorrectScriptableObjectInstantiation

namespace Tests.Editor
{
    public class TestSourceValidator
    {
        [Test]
        public void Test_Endpoint_Has_Null_Empty_Name_Throw_Exception()
        {
            Assert.Throws<Exception>(() =>
            {
                new Request
                {
                    EndpointName = null,
                    Url = "test"
                }.ValidateToGenerate();
            }, "test-request uses a null (or empty) generated name");
            Assert.Throws<Exception>(() =>
            {
                new Request
                {
                    EndpointName = string.Empty,
                    Url = "test"
                }.ValidateToGenerate();
            }, "test uses a null (or empty) generated name");
        }
        
        [Test]
        public void Test_Endpoint_Has_Same_Name_With_Parent_Throw_Exception()
        {
            Assert.Throws<Exception>(() =>
            {
                var request = new Request()
                {
                    EndpointName = "same-name",
                    Url = "test"
                };
                var domain = new Domain
                {
                    EndpointName = "Same name",
                    Url = "test",
                    Requests = new List<Request>
                    {
                        request
                    }
                };
                request.Parent = domain;
                domain.ValidateToGenerate();
            }, "same_name(test) uses the same generated class name with its parent (Same name): SameName");
        }
        
        internal class InvalidAppender
        {
        }
        [Test]
        public void Test_Auth_Container_Contains_Inaccessible_Script_Throw_Exception()
        {
            Assert.Throws<Exception>(() =>
            {
                new SummerRestConfiguration
                {
                    AuthContainers =
                    {
                        new AuthContainer
                        {
                            Appender = typeof(InvalidAppender)
                        }
                    }
                }.ValidateAuthContainers(Assembly.Load("SummerRest"));
            }, $"The target assembly SummerRest does not contain (or reference) appender type {typeof(InvalidAppender)}");
        }

        [Test]
        public void Test_Auth_Configures_Have_Duplicated_Keys_Throw_Exception()
        {
            Assert.Throws<Exception>(() =>
            {
                new SummerRestConfiguration
                {
                    AuthContainers =
                    {
                        new AuthContainer
                        {
                            AuthKey = "dup-key"
                        },
                        new AuthContainer
                        {
                            AuthKey = "dup-key"
                        },
                        new AuthContainer
                        {
                            AuthKey = "dup-key-1"
                        },
                        new AuthContainer
                        {
                            AuthKey = "dup-key-1"
                        }
                    }
                }.ValidateAuthContainers(Assembly.Load("SummerRest"));
            }, "The followings auth keys [dupkey,dupkey-1] are duplicated, please check your auth containers in the advanced settings section");
        }
        
        [Test]
        public void Test_Satisfied_Configures_Not_Throw()
        {
            Assert.DoesNotThrow(() =>
            {
                var serviceRequest = new Request()
                {
                    EndpointName = "Request"
                };
                var service = new Service
                {
                    EndpointName = "Service",
                    Requests = new List<Request>()
                    {
                        serviceRequest
                    }
                };
                var domainRequest = new Request()
                {
                    EndpointName = "Request"
                };
                var domain = new Domain
                {
                    EndpointName = "Domain",
                    Services = new List<Service>
                    {
                        service
                    },
                    Requests = new List<Request>
                    {
                        domainRequest
                    }
                };
                serviceRequest.Parent = service;
                service.Parent = domain;
                domainRequest.Parent = domain;
                new SummerRestConfiguration
                {
                    Domains = new List<Domain>
                    {
                       domain
                    },
                    AuthContainers =
                    {
                        new AuthContainer
                        {
                            AuthKey = "dup-key",
                            Appender = typeof(BearerTokenAuthAppender)
                        },
                    }
                }.ValidateToGenerate();
            });
        }
    }
}