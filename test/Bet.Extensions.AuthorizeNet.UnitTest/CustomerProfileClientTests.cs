using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using AuthorizeNet.Api.V1.Contracts;

using Bet.Extensions.AuthorizeNet.Api.V1.Clients;
using Bet.Extensions.AuthorizeNet.Api.V1.Contracts;
using Bet.Extensions.Testing.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace Bet.Extensions.AuthorizeNet.UnitTest
{
    public class CustomerProfileClientTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _configuration;

        public CustomerProfileClientTests(ITestOutputHelper output)
        {
            var inMemoryConfiguration = new Dictionary<string, string>
            {
                { "Test:SomeNode", "Hello World" },
            };

            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configBuilder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

            configBuilder.AddInMemoryCollection(inMemoryConfiguration);
            _output = output;

            _configuration = configBuilder.Build();
        }

        [Fact]
        public async Task Create_Get_Delete_Test()
        {
            var services = new ServiceCollection();

            services.AddSingleton(_configuration);

            services.AddLogging(b => b.AddXunit(_output));
            services.AddAuthorizeNet();

            var sp = services.BuildServiceProvider();

            var customerProfile = sp.GetRequiredService<CustomerProfileClient>();

            // customer payment profile
            var customerPaymentProfiles = new Collection<CustomerPaymentProfileType>
                {
                    new CustomerPaymentProfileType
                    {
                        CustomerType = CustomerTypeEnum.Business,
                        Payment = new PaymentType
                        {
                            CreditCard = new CreditCardType
                            {
                                CardNumber = "4111111111111111",
                                ExpirationDate = "2020-12"
                            }
                        }
                    }
                };

            // create request
            var createRequest = new CreateCustomerProfileRequest
            {
                Profile = new CustomerProfileType
                {
                    Description = "Test Customer Account",
                    Email = "email2@email.com",
                    MerchantCustomerId = "CustomerId-2",
                    ProfileType = CustomerProfileTypeEnum.Regular,
                    PaymentProfiles = customerPaymentProfiles,
                },

                ValidationMode = ValidationModeEnum.TestMode
            };

            var createResponse = await customerProfile.CreateAsync(createRequest);

            Assert.Equal(MessageTypeEnum.Ok, createResponse.Messages.ResultCode);

            var getResponse = await customerProfile.GetAsync(new GetCustomerProfileRequest { CustomerProfileId = createResponse.CustomerProfileId });

            Assert.Equal(MessageTypeEnum.Ok, getResponse.Messages.ResultCode);

            _output.WriteLine(getResponse.Profile.CustomerProfileId);

            var deleteRequest = new DeleteCustomerProfileRequest
            {
                CustomerProfileId = createResponse.CustomerProfileId
            };

            var deleteRespnse = await customerProfile.DeleteAsync(deleteRequest);

            Assert.Equal(MessageTypeEnum.Ok, deleteRespnse.Messages.ResultCode);
        }

        [Fact]
        public void Test_DirectResponse()
        {
            var directResponse = @"2,2,27,The transaction has been declined because of an AVS mismatch. The address provided does not match billing address of cardholder.,AE5V2W,N,40059863807,none,Test transaction for ValidateCustomerPaymentProfile.,0.00,CC,auth_only,customer-48,Verdie,Farrell,Schimmel, McGlynn and Kling,214 Lucinda Streets,West Barrett,Kansas,46201,TC,,,Verdie_Farrell@gmail.com,,,,,,,,,0.00,0.00,0.00,FALSE,none,,P,2,,,,,,,,,,,XXXX0015,MasterCard,,,,,,,0S8C3LQM3HC6DEDAFV94OTO,,,,,,,,,,";

            var parsed = new PaymentGatewayResponse(directResponse);

            Assert.Equal("2", parsed.ResponseCode);
        }
    }
}
