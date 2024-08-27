using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using AutoMapper;
using Common.Cache;
using Common.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RazorLight;
using System.Text;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _emailSender;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ICacheService cacheService,
        UserManager<User> userManager,
        IEmailSender emailSender)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
    }

    public async Task<UserResponseDto> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null; // Consider throwing a custom exception
        }

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto)
    {
        if (userRequestDto == null)
        {
            throw new ArgumentNullException(nameof(userRequestDto));
        }

        var user = _mapper.Map<User>(userRequestDto);
        var result = await _userManager.CreateAsync(user, userRequestDto.Password);

        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var activationLink = $"http://localhost:5165/Auth/ConfirmEmail?userId={user.Id}&code={encodeToken}";

            // Kiểm tra giá trị email tại đây
            await SendWelcomeEmailAsync(user.Email, user.UserName, activationLink);
            return _mapper.Map<UserResponseDto>(user);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"User creation failed: {errors}");
    }

    public async Task<UserResponseDto> UpdateUserAsync(string id, UserRequestDto userRequestDto)
    {
        if (userRequestDto == null)
        {
            throw new ArgumentNullException(nameof(userRequestDto));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null; // Consider throwing a custom exception
        }

        _mapper.Map(userRequestDto, user);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            // Send email after user update
            var emailBody = "Your profile has been updated.";

            return _mapper.Map<UserResponseDto>(user);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"User update failed: {errors}");
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<string> GetUserTokenAsync(string userId)
    {
        return await _cacheService.GetCacheAsync(userId);
    }

    public async Task SetUserTokenAsync(string userId, string token, TimeSpan expiration)
    {
        await _cacheService.SetCacheAsync(userId, token, expiration);
    }

    public async Task<PagedDto<UserResponseDto>> GetPagedUsersAsync(UserFilter filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        var users = await _userManager.Users
            .Skip(filter.PageIndex * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var userResponseDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);

        var pagedResponse = new PagedDto<UserResponseDto>(
            userResponseDtos,
            await _userManager.Users.CountAsync(),
            filter.PageIndex,
            filter.PageSize
        );

        return pagedResponse;
    }
    public async Task SendWelcomeEmailAsync(string email, string userName, string confirmationLink)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "WelcomeEmail.html");
        var emailBody = await File.ReadAllTextAsync(templatePath);

        // Thay thế các biến
        emailBody = emailBody.Replace("{{UserName}}", userName);
        emailBody = emailBody.Replace("{{ConfirmationLink}}", confirmationLink);

        // Gửi email sử dụng IEmailSender
        await _emailSender.SendEmailAsync(email, "Welcome to Our Service", emailBody);
    }

}
