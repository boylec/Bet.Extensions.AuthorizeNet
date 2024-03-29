﻿using System.Threading;
using System.Threading.Tasks;

using AuthorizeNet.Api.V1.Contracts;

namespace Bet.Extensions.AuthorizeNet.Api.V1.Clients
{
    public class TransactionClient : ITransactionClient
    {
        private readonly IAuthorizeNetClient<CreateTransactionRequest, CreateTransactionResponse> _create;
        private readonly IAuthorizeNetClient<GetTransactionDetailsRequest, GetTransactionDetailsResponse> _get;
        private readonly IAuthorizeNetClient<GetSettledBatchListRequest, GetSettledBatchListResponse> _getBatchList;
        private readonly IAuthorizeNetClient<GetTransactionListRequest, GetTransactionListResponse> _getList;
        private readonly IAuthorizeNetClient<GetUnsettledTransactionListRequest, GetUnsettledTransactionListResponse> _getUnsettledList;

        public TransactionClient(
            IAuthorizeNetClient<CreateTransactionRequest, CreateTransactionResponse> create,
            IAuthorizeNetClient<GetTransactionDetailsRequest, GetTransactionDetailsResponse> get,
            IAuthorizeNetClient<GetSettledBatchListRequest, GetSettledBatchListResponse> getBatchList,
            IAuthorizeNetClient<GetTransactionListRequest, GetTransactionListResponse> getList,
            IAuthorizeNetClient<GetUnsettledTransactionListRequest, GetUnsettledTransactionListResponse> getUnsettledList)
        {
            _create = create ?? throw new ArgumentNullException(nameof(create));
            _get = get ?? throw new ArgumentNullException(nameof(get));
            _getBatchList = getBatchList ?? throw new ArgumentNullException(nameof(getBatchList));
            _getList = getList ?? throw new ArgumentNullException(nameof(getList));
            _getUnsettledList = getUnsettledList ?? throw new ArgumentNullException(nameof(getUnsettledList));
        }

        public Task<CreateTransactionResponse> CreateAsync(
            CreateTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _create.PostAsync(request, cancellationToken);
        }

        public Task<GetTransactionDetailsResponse> GetAsync(
              GetTransactionDetailsRequest request,
              CancellationToken cancellationToken = default)
        {
            return _get.PostAsync(request, cancellationToken);
        }

        public Task<GetSettledBatchListResponse> GetBatchListAsync(
              GetSettledBatchListRequest request,
              CancellationToken cancellationToken = default)
        {
            return _getBatchList.PostAsync(request, cancellationToken);
        }

        public Task<GetTransactionListResponse> GetListAsync(
             GetTransactionListRequest request,
             CancellationToken cancellationToken = default)
        {
            return _getList.PostAsync(request, cancellationToken);
        }

        public Task<GetUnsettledTransactionListResponse> GetUnsettledListAsync(
                 GetUnsettledTransactionListRequest request,
                 CancellationToken cancellationToken = default)
        {
            return _getUnsettledList.PostAsync(request, cancellationToken);
        }
    }
}
