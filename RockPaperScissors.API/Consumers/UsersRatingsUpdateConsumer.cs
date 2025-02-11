using api.Contracts;
using api.Repositories;
using MassTransit;

namespace api.Consumers;

public class UsersRatingsUpdateConsumer: IConsumer<UpdateUsersRatings>
{
    private readonly IUserRatingsRepository _ratingRepository;

    public UsersRatingsUpdateConsumer(IUserRatingsRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task Consume(ConsumeContext<UpdateUsersRatings> context)
    {
        var message = context.Message;
        var currentUser1 = await _ratingRepository.GetSingle(message.user1.UserId);
        var currentUser2 = await _ratingRepository.GetSingle(message.user2.UserId);
        await _ratingRepository.UpdateOneAsync(message.user1.UserId, currentUser1.Rating + message.user1.Delta);
        await _ratingRepository.UpdateOneAsync(message.user2.UserId, currentUser2.Rating + message.user2.Delta);
    }
}