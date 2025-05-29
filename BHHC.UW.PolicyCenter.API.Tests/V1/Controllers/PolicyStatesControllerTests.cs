using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.API.V1.Controllers;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.Tests.V1.Controllers
{
    public class PolicyStatesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PolicyStatesController>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PolicyStatesController _controller;

        public PolicyStatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PolicyStatesController>>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new PolicyStatesController(
                _mediatorMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsOk_WithStates()
        {
            var policyId = "MGA1";
            var states = new List<UWStateDTO> { new UWStateDTO() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(states);

            var result = await _controller.GetPolicyStates(policyId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<UWStateDTO>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(states, response.Result);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsOk_WithError_WhenStatesNull()
        {
            var policyId = "MGA1";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<UWStateDTO>)null);

            var result = await _controller.GetPolicyStates(policyId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UWStateDTO>>(okResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        //[Fact]
        //public async Task GetPolicyStates_ReturnsInternalServerError_OnSqlException()
        //{
        //    var policyId = "MGA1";
        //    _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new SqlException());

        //    var result = await _controller.GetPolicyStates(policyId);

        //    var statusResult = Assert.IsType<ObjectResult>(result);
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, statusResult.StatusCode);
        //    var response = Assert.IsType<ApiResponse<bool>>(statusResult.Value);
        //    Assert.False(response.Success);
        //    Assert.NotNull(response.Exception);
        //}

        [Fact]
        public async Task GetPolicyStates_ReturnsInternalServerError_OnException()
        {
            var policyId = "MGA1";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error"));

            var result = await _controller.GetPolicyStates(policyId);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusResult.StatusCode);
            var response = Assert.IsType<ApiResponse<bool>>(statusResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsBadRequest_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("State", "Required");
            var request = new UpsertPolicyStateCommandRequest();

            var result = await _controller.UpsertPolicyState(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(badRequest.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsOk_WhenSuccess()
        {
            var request = new UpsertPolicyStateCommandRequest { MgaCode = "MGA1", State = "CA" };
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync("Success");

            var result = await _controller.UpsertPolicyState(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Success", response.Result);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsBadRequest_OnBusinessLogicException()
        {
            var request = new UpsertPolicyStateCommandRequest { MgaCode = "MGA1", State = "CA" };
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessLogicException("Business error"));

            var result = await _controller.UpsertPolicyState(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(badRequest.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        //[Fact]
        //public async Task UpsertPolicyState_ReturnsInternalServerError_OnSqlException()
        //{
        //    var request = new UpsertPolicyStateCommandRequest { MgaCode = "MGA1", State = "CA" };
        //    var command = new UpsertPolicyStateCommand();
        //    _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
        //    _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new SqlException());

        //    var result = await _controller.UpsertPolicyState(request);

        //    var statusResult = Assert.IsType<ObjectResult>(result);
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, statusResult.StatusCode);
        //    var response = Assert.IsType<ApiResponse<bool>>(statusResult.Value);
        //    Assert.False(response.Success);
        //    Assert.NotNull(response.Exception);
        //}

        [Fact]
        public async Task UpsertPolicyState_ReturnsInternalServerError_OnException()
        {
            var request = new UpsertPolicyStateCommandRequest { MgaCode = "MGA1", State = "CA" };
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error"));

            var result = await _controller.UpsertPolicyState(request);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusResult.StatusCode);
            var response = Assert.IsType<ApiResponse<bool>>(statusResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsBadRequest_WhenMgaCodeMissing()
        {
            var result = await _controller.GetAvailableStates("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(badRequest.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsOk_WithStates()
        {
            var mgacode = "MGA1";
            var states = new List<ReferenceStateDTO> { new ReferenceStateDTO() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(states);

            var result = await _controller.GetAvailableStates(mgacode);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ReferenceStateDTO>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(states, response.Result);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsOk_WithError_WhenStatesNull()
        {
            var mgacode = "MGA1";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<ReferenceStateDTO>)null);

            var result = await _controller.GetAvailableStates(mgacode);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ReferenceStateDTO>>(okResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        //[Fact]
        //public async Task GetAvailableStates_ReturnsInternalServerError_OnSqlException()
        //{
        //    var mgacode = "MGA1";
        //    _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new SqlException());

        //    var result = await _controller.GetAvailableStates(mgacode);

        //    var statusResult = Assert.IsType<ObjectResult>(result);
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, statusResult.StatusCode);
        //    var response = Assert.IsType<ApiResponse<bool>>(statusResult.Value);
        //    Assert.False(response.Success);
        //    Assert.NotNull(response.Exception);
        //}

        [Fact]
        public async Task GetAvailableStates_ReturnsInternalServerError_OnException()
        {
            var mgacode = "MGA1";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error"));

            var result = await _controller.GetAvailableStates(mgacode);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusResult.StatusCode);
            var response = Assert.IsType<ApiResponse<bool>>(statusResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }
    }
}
