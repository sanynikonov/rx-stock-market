using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Business.Users;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Api.Services;

public class UserService : Api.UserService.UserServiceBase
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;

    public UserService(IUserService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _service.Register(request.UserName, request.Password);
        return !result.Succeeded
            ? new RegisterResponse { Error = new Error { Message = JsonSerializer.Serialize(result.Errors) } }
            : new RegisterResponse();
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var token = await _service.Login(request.UserName, request.Password);
        return new LoginResponse { Token = token };
    }

    [Authorize]
    public override async Task<UpdatePreferencesResponse> UpdatePreferences(UpdatePreferencesRequest request, ServerCallContext context)
    {
        try
        {
            var username = context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _service.UpdateUserPreferences(_mapper.Map<UserPreferences>(request), username);
            return new UpdatePreferencesResponse();
        }
        catch (Exception e)
        {
            return new UpdatePreferencesResponse { Error = new Error { Message = e.Message } };
        }
    }
}