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
        await _ratingRepository.UpdateOneAsync(message.user1.UserId, message.user1.Delta);
        await _ratingRepository.UpdateOneAsync(message.user2.UserId, message.user2.Delta);
    }
}